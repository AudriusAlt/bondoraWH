using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class AccountBalance
    {
        public decimal Balance { get; set; }

        public decimal Reserved { get; set; }

        public decimal BidRequestAmount { get; set; }

        public decimal TotalAvailable { get; set; }
    }
}
