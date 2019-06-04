using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class SellSecondMarketItemRequest
    {
        public List<SecondMarketSell> Items { get; set; }

        public bool CancelItemOnPaymentReceived { get; set; }

        public bool CancelItemOnReschedule { get; set; }
    }
}
