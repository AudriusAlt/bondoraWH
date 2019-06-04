using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class ApiResultAccountBalance :ApiResult<AccountBalance>
    {
        public int TotalCount { get; set; }
    }
}
