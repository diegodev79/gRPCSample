using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCSampleApp.Models
{
    public class AuctionNode
    {
        [Key]
        public int NodeId { get; set; }
        public string NodeName { get; set; }
        public int Port { get; set; }

        public string IPAddress { get; set; } // Add IP address property if needed
    }
}
