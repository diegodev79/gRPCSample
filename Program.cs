using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPCSampleApp.Models;
using RPCSampleApp.Services;
using System.Net;

namespace RPCSampleApp
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var notificationService = new NotificationService();

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole().SetMinimumLevel(LogLevel.Error);
                })
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<NotificationService>(notificationService)
                .AddDbContext<AuctionDbContext>((serviceProvider, options) =>
                {
                    var config = serviceProvider.GetRequiredService<IConfiguration>(); // Get IConfiguration
                    string dbPath = config["DatabaseSettings:DatabasePath"];
                    options.UseSqlite($"Data Source={dbPath}");
                    options.LogTo(Console.WriteLine, LogLevel.Error);
                })
                .AddSingleton<AuctionServiceCore>()
                .BuildServiceProvider();

            Console.WriteLine("Hello, Welcome to gRPC Auction Manager Sample by Diego Barrantes!");

            string participantName = GetUniqueParticipantName(serviceProvider);           

            var auctionService = serviceProvider.GetRequiredService<AuctionServiceCore>();
            auctionService.AddParticipant(participantName);
            Console.WriteLine($"Welcome, {participantName}!");


            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();

                while (true)
                {
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("1. Create Auction");
                    Console.WriteLine("2. List Current Auctions");
                    Console.WriteLine("3. Exit");
                    Console.Write("Choose an option (1-3): ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                CreateAuction(dbContext, participantName, configuration, auctionService);
                                break;
                            case 2:
                                ListCurrentAuctions(dbContext, participantName, auctionService);
                                break;
                            case 3:
                                Console.WriteLine("Exiting... Goodbye!");
                                return;
                            default:
                                Console.WriteLine("Invalid choice. Please choose a valid option.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                    }
                }
            }
        }
        static string GetUniqueParticipantName(IServiceProvider serviceProvider)
        {
            string participantName = string.Empty;

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();

                do
                {
                    Console.Write("Enter your name to participate: ");
                    participantName = Console.ReadLine();

                    if (dbContext.AuctionNodes.Any(node => node.NodeName == participantName))
                    {
                        Console.WriteLine($"Participant '{participantName}' already exists. Please choose a different name.");
                    }
                    else
                    {
                        string ipAddress = GetLocalIPAddress();

                        // Add AuctionNode with the obtained IP address
                        dbContext.AuctionNodes.Add(new AuctionNode { NodeName = participantName, IPAddress = ipAddress });
                        dbContext.SaveChanges();
                        Console.WriteLine($"Participant '{participantName}' has been added to the database.");
                    }
                } while (string.IsNullOrWhiteSpace(participantName));
            }

            return participantName;
        }
        static string GetLocalIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            return addresses.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "Unknown";
        }

        static void CreateAuction(AuctionDbContext dbContext, string participantName, IConfiguration configuration, AuctionServiceCore auctionService)
        {
            Console.Write("Enter the item name for the auction: ");
            string itemName = Console.ReadLine();
            var currentTime = DateTime.Now;
            var auctionDurationInMinutes = int.Parse(configuration["AuctionSettings:AuctionDuration"]);
            var endTime = currentTime.AddMinutes(auctionDurationInMinutes);

            Console.Write("Enter the initial price for the auction: ");
            if (int.TryParse(Console.ReadLine(), out int initialPrice))
            {
                // Add a new auction to the database
                var newAuction = new AuctionItem
                {
                    ItemName = itemName,
                    InitialPrice = initialPrice,
                    InitiatorName = participantName,
                    EndTime = endTime
                };

                dbContext.AuctionItems.Add(newAuction);
                dbContext.SaveChanges();

                // Retrieve the generated Id after saving changes
                var auctionItemId = newAuction.Id;

                // Call the AuctionService method to initialize the auction
                var initializationRequest = new AuctionInitializationRequest
                {
                    ParticipantName = participantName,
                    AuctionItemId = auctionItemId,
                    InitialPrice = initialPrice,
                    InitializerId = 0, 
                };

                // Pass null for the context
                var initializationReply = auctionService.InitializeAuction(initializationRequest, null).Result;

                if (initializationReply != null && initializationReply.Message.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Auction '{itemName}' created successfully!");

                    
                }
                else
                {
                    Console.WriteLine($"Error initializing auction: {initializationReply?.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Initial price must be a valid number.");
            }
        }



        static void ListCurrentAuctions(AuctionDbContext dbContext, string participantName, AuctionServiceCore auctionService)
        {
            var currentTime = DateTime.Now; // just in case some logic changes later

            var openAuctions = dbContext.AuctionItems
                .Where(a => !a.IsClosed && a.EndTime > DateTime.Now)
                .Select(a => new
                {
                    a.Id,
                    a.ItemName,
                    a.InitialPrice,
                    HighestBidAmount = a.Bids.OrderByDescending(b => b.BidAmount).Select(b => (int?)b.BidAmount).FirstOrDefault()
                })
                .ToList();

            Console.WriteLine("\nCurrent Auctions:");

            foreach (var auction in openAuctions)
            {
                Console.WriteLine($"Auction ID: {auction.Id}, Item: {auction.ItemName}, Initial Price: {auction.InitialPrice}, Highest Bid: {auction.HighestBidAmount ?? 0}");
            }

            Console.Write("Enter the ID of the auction to place a bid (0 to go back): ");
            if (int.TryParse(Console.ReadLine(), out int auctionId))
            {
                if (auctionId == 0)
                {
                    return; // Go back to the main menu
                }

                var selectedAuction = openAuctions.FirstOrDefault(a => a.Id == auctionId);

                if (selectedAuction != null)
                {
                    Console.Write($"Enter your bid for '{selectedAuction.ItemName}': ");
                    if (int.TryParse(Console.ReadLine(), out int bidAmount))
                    {
                        // Place a bid
                        var bidRequest = new BidRequest
                        {
                            BidderName = participantName,
                            AuctionItemId = auctionId,
                            BidAmount = bidAmount
                        };

                        var bidReply = auctionService.PlaceBid(bidRequest, null); // Pass null for ServerCallContext

                        Console.WriteLine(bidReply.Result.Message);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Bid amount must be a valid number.");
                    }
                }
                else
                {
                    Console.WriteLine($"Auction with ID {auctionId} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Auction ID must be a valid number.");
            }
        }



    }
}
