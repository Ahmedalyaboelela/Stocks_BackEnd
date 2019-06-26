using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public class AccountingHelper : IAccountingHelper
    {

        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public AccountingHelper(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }

        public void TransferToAccounts(List<EntryDetailModel> EntryList)
        {
            foreach (var item in EntryList)
            {
                var account = unitOfWork.AccountRepository.GetByID(item.AccountID);
                if(item.Credit!=0)
                {
                    if(account.Credit==null)
                    {
                        account.Credit = 0;
                    }
                    account.Credit += item.Credit;
                }
                if(item.Debit !=0)
                {
                    if (account.Debit == null)
                    {
                        account.Debit = 0;
                    }
                    account.Debit += item.Debit;
                }
                unitOfWork.AccountRepository.Update(account);
            }
        }
            
    }
}
