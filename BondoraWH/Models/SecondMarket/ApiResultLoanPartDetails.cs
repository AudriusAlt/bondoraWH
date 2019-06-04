using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class ApiResultLoanPartDetails : ApiResult<IList<LoanPartDetails>>

    {
        public int TotalCount { get; set; }
    }
}
