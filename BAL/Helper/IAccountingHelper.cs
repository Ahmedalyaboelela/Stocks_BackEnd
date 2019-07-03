using BAL.Model;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public interface IAccountingHelper
    {
        void TransferToAccounts(List<EntryDetail> EntryList);
        void CancelTransferToAccounts(List<EntryDetail> EntryList);
    }
}
