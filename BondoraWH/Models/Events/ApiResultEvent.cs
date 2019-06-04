using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class ApiResultEvent
    {
        public string EventId { get; set; }

        public string EventType { get; set; }

        public SecondMarketItem Payload { get; set; }

    }
}
