using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{ 
    public class LoanPartDetails
    {
            public string LoanPartId { get; set; }
            public float Amount { get; set; }
            public string AuctionId { get; set; }
            public string AuctionName { get; set; }
            public int AuctionNumber { get; set; }
            public int AuctionBidNumber { get; set; }
            public string InvestmentNumber { get; set; }
            public string Country { get; set; }
            public string CreditScore { get; set; }
            public string CreditScoreEsMicroL { get; set; }
            public string CreditScoreEsEquifaxRisk { get; set; }
            public string CreditScoreFiAsiakasTietoRiskGrade { get; set; }
            public string CreditScoreEeMini { get; set; }
            public string Rating { get; set; }
            public float InitialInterest { get; set; }
            public float Interest { get; set; }
            public int UseOfLoan { get; set; }
            public int IncomeVerificationStatus { get; set; }
            public string LoanId { get; set; }
            public int LoanStatusCode { get; set; }
            public string UserName { get; set; }
            public int Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public DateTime SignedDate { get; set; }
            public string ReScheduledOn { get; set; }
            public DateTime DebtOccuredOn { get; set; }
            public DateTime DebtOccuredOnForSecondary { get; set; }
            public int LoanDuration { get; set; }
            public int NextPaymentNr { get; set; }
            public string NextPaymentDate { get; set; }
            public string NextPaymentSum { get; set; }
            public int NrOfScheduledPayments { get; set; }
            public DateTime LastPaymentDate { get { return LastPaymentDate; } set { if (value == null) value = new DateTime(); } }
            public float PrincipalRepaid { get; set; }
            public float InterestRepaid { get; set; }
            public float LateAmountPaid { get; set; }
            public float PrincipalRemaining { get; set; }
            public float PrincipalLateAmount { get; set; }
            public float InterestLateAmount { get; set; }
            public float PenaltyLateAmount { get; set; }
            public float LateAmountTotal { get; set; }
            public float PrincipalWriteOffAmount { get; set; }
            public float InterestWriteOffAmount { get; set; }
            public float PenaltyWriteOffAmount { get; set; }
            public float WriteOffTotal { get; set; }
            public float DebtServicingCostMainAmount { get; set; }
            public float DebtServicingCostInterestAmount { get; set; }
            public float DebtServicingCostPenaltyAmount { get; set; }
            public float DebtServicingCostTotal { get; set; }
            public float RepaidPrincipalCurrentOwner { get; set; }
            public float RepaidInterestsCurrentOwner { get; set; }
            public float LateChargesPaidCurrentOwner { get; set; }
            public float RepaidTotalCurrentOwner { get; set; }
            public float TotalRepaid { get; set; }
            public Debtmanagmentevent[] DebtManagmentEvents { get; set; }
            public Loantransfer[] LoanTransfers { get; set; }
            public Scheduledpayment[] ScheduledPayments { get; set; }
 
  

        public class Debtmanagmentevent
        {
            public DateTime CreatedOn { get; set; }
            public int EventType { get; set; }
            public string Description { get; set; }
        }

        public class Loantransfer
        {
            public DateTime Date { get; set; }
            public float PrincipalAmount { get; set; }
            public float InterestAmount { get; set; }
            public float InterestAmountCarriedOver { get; set; }
            public float PenaltyAmountCarriedOver { get; set; }
            public float TotalAmount { get; set; }
        }

        public class Scheduledpayment
        {
            public DateTime ScheduledDate { get; set; }
            public float PrincipalAmount { get; set; }
            public float PrincipalAmountLeft { get; set; }
            public float InterestAmount { get; set; }
            public float IntrestAmountCarriedOver { get; set; }
            public float PenaltyAmountCarriedOver { get; set; }
            public float TotalAmount { get; set; }
        }

    }
}
