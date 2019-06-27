using BAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public interface IAccountingHelper
    {
        void TransferToAccounts(List<EntryDetailModel> EntryList);
    }
}
