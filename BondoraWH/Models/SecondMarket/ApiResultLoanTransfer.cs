using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public class ApiResultLoanTransfer : ApiResult<IList<LoanTransfer>>
    {
        public int TotalCount { get; set; }
    }
}
