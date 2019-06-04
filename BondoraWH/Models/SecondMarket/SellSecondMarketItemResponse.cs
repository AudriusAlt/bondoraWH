using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class SellSecondMarketItemResponse
    {
        public bool Success { get; set; }

        public List<ApiError> Errors { get; set; }
    }
}
