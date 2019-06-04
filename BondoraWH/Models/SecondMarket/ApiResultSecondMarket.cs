﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class ApiResultSecondMarket : ApiResult<IList<SecondMarketItem>>
    {
        /// <summary>
        /// Total number of SecondaryMarket items found
        /// </summary>
        public int TotalCount { get; set; }
    }
}
