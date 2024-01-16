using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RPCSampleApp.Models;
using System.Threading.Tasks;
using static RPCSampleApp.Services.AuctionService;


namespace RPCSampleApp.Services
{
    public class AuctionServiceCore : AuctionServiceBase
    {
        private readonly ILogger<AuctionServiceCore> _logger;
        private readonly AuctionDbContext _dbContext;

        public AuctionServiceCore(ILogger<AuctionServiceCore> logger, AuctionDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override async Task<BidReply> PlaceBid(BidRequest request, ServerCallContext context)
        {
            try
            {
                // Validate the bid request (you may add more validation logic)
                if (request.BidAmount <= 0)
                {
                    return new BidReply { Message = "Invalid bid amount. Bid amount must be greater than zero." };
                }

                // Find the auction item in the database based on the auction_item_id
                AuctionItem auctionItem = await _dbContext.AuctionItems.FindAsync(request.AuctionItemId);

                if (auctionItem == null)
                {
                    return new BidReply { Message = "Auction item not found." };
                }

                // Create a new bid
                Bid newBid = new Bid
                {
                    BidderName = request.BidderName,
                    BidAmount = request.BidAmount,
                    Timestamp = DateTime.UtcNow
                };

                // Add the bid to the auction item
                auctionItem.Bids.Add(newBid);

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                return new BidReply { Message = "Bid placed successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error placing bid: {ex.Message}");
                return new BidReply { Message = "An error occurred while placing the bid." };
            }
        }



        public override async Task<AuctionInitializationReply> InitializeAuction(AuctionInitializationRequest request, ServerCallContext context)
        {
            try
            {
                // Validate the auction initialization request (you may add more validation logic)
                if (request.InitialPrice <= 0)
                {
                    return new AuctionInitializationReply { Message = "Invalid initial price. Initial price must be greater than zero." };
                }

                // Check if the auction item with the provided ID already exists
                if (_dbContext.AuctionItems.Any(a => a.Id == request.AuctionItemId))
                {
                    return new AuctionInitializationReply { Message = "Auction item with the provided ID already exists." };
                }

                // Create a new auction item
                AuctionItem newAuctionItem = new AuctionItem
                {
                    ItemName = request.AuctionItemName, // You may want to use a more descriptive property
                    InitialPrice = request.InitialPrice,
                    InitiatorName = request.ParticipantName
                };

                // Add the new auction item to the database
                _dbContext.AuctionItems.Add(newAuctionItem);
                await _dbContext.SaveChangesAsync();

                return new AuctionInitializationReply { Message = "Auction initialized successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initializing auction: {ex.Message}");
                return new AuctionInitializationReply { Message = "An error occurred while initializing the auction." };
            }
        }
        public override Task<NotifyBidReply> NotifyBid(NotifyBidRequest request, ServerCallContext context)
        {
            try
            {
                // Perform actions when a bid is received
                // You may add more logic here based on your requirements

                _logger.LogInformation($"Bid notification received: Bidder Name - {request.BidderName}, Bid Amount - {request.BidAmount}");

                // Additional logic as needed...

                return Task.FromResult(new NotifyBidReply { Message = "Bid notification received successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing bid notification: {ex.Message}");
                return Task.FromResult(new NotifyBidReply { Message = "An error occurred while processing bid notification." });
            }
        }

        public override Task<NotifyClosureReply> NotifyClosure(NotifyClosureRequest request, ServerCallContext context)
        {
            try
            {
                // Perform actions when an auction is closed
                // You may add more logic here based on your requirements

                _logger.LogInformation($"Auction closure notification received: Auction ID - {request.AuctionId}, Closing Value - {request.ClosingValue}");

                // Additional logic as needed...

                return Task.FromResult(new NotifyClosureReply { Message = "Auction closure notification received successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing auction closure notification: {ex.Message}");
                return Task.FromResult(new NotifyClosureReply { Message = "An error occurred while processing auction closure notification." });
            }
        }
    }

}
