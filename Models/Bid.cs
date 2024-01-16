using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCSampleApp.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public string BidderName { get; set; }
        public int BidAmount { get; set; }
        public DateTime Timestamp { get; set; }

        

        public int AuctionItemId { get; set; }
        public AuctionItem AuctionItem { get; set; }
    }
}
