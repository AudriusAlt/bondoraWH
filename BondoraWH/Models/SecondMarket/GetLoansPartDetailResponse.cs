using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{ 

   public class GetLoansPartDetailResponse
    {
        public List<LoanPartDetails> Payload { get; set; }

        public bool Success { get; set; }

        public List<ApiError> Errors { get; set; }
    }
}
