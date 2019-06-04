﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models.Enums
{
    public enum AuctionIncomeVerificationStatus
    {
        NotVerified = 1,
        VerifiedByPhone = 2,
        VerifiedByOtherDocument = 3,
        VerifiedByBankStatement = 4
    }
}
