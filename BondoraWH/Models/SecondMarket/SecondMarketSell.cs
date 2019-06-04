using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class SecondMarketSell
    {
        public Guid LoanPartId { get; set; }

        public int DesiredDiscountRate { get; set; }
    }
}
