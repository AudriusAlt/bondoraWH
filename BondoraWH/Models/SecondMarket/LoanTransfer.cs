using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
   public class LoanTransfer
    {
        public DateTime Date { get; set; }

        public decimal PrincipalAmount { get; set; }

        public decimal InterestAmount { get; set; }

        public decimal IneterstAmountCarriedOver { get; set; }

        public decimal PenaltyAmountCarriedOver { get; set; }

        public decimal TotalAmount { get; set; }

    }
}
