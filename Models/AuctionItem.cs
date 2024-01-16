using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCSampleApp.Models
{
    public class AuctionItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int InitialPrice { get; set; }
        public string InitiatorName { get; set; }
        public bool IsClosed { get; set; }
        public DateTime EndTime { get; set; }

        // Navigation property for bids associated with this auction item
        public List<Bid> Bids { get; set; }
    }
}
