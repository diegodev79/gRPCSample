using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RPCSampleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCSampleApp.Services
{
    public class AuctionDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Bid> Bids { get; set; }
        public DbSet<AuctionNode> AuctionNodes { get; set; }
        public DbSet<AuctionItem> AuctionItems { get; set; }

        public AuctionDbContext(DbContextOptions<AuctionDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = _configuration["DatabaseSettings:DatabasePath"];
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            Console.WriteLine($"Database path: {dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bid>()
            .HasOne(b => b.AuctionItem)
            .WithMany(a => a.Bids)
            .HasForeignKey(b => b.AuctionItemId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
