using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
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

        //Transfer Entry To Accounts
        public void TransferToAccounts(List<EntryDetail> EntryList)
        {
            foreach (var item in EntryList)
            {
                var account = unitOfWork.AccountRepository.GetByID(item.AccountID);
                if(item.Credit!=null)
                {
                    if(account.Credit==null)
                    {
                        account.Credit = 0;
                    }
                    account.Credit += item.Credit;
                    if(item.StocksCredit!=null)
                    {
                        if (account.StocksCredit == null)
                        {
                            account.StocksCredit = 0;
                        }
                        account.StocksCredit += item.StocksCredit;
                    }
                }
                if(item.Debit !=null)
                {
                    if (account.Debit == null)
                    {
                        account.Debit = 0;
                    }
                    account.Debit += item.Debit;
                    if (item.StocksDebit != null)
                    {
                        if (account.StocksDebit == null)
                        {
                            account.StocksDebit = 0;
                        }
                        account.StocksDebit += item.StocksDebit;
                    }

                }
                unitOfWork.AccountRepository.Update(account);
            }
        }

        //Cancel Transfer Entry From Accounts
        public void CancelTransferToAccounts(List<EntryDetail> EntryList)
        {
            foreach (var item in EntryList)
            {
                var account = unitOfWork.AccountRepository.GetByID(item.AccountID);
                if (item.Credit != 0)
                {
                    if (account.Credit == null)
                    {
                        account.Credit = 0;
                    }
                    account.Credit -= item.Credit;
                }
                if (item.Debit != 0)
                {
                    if (account.Debit == null)
                    {
                        account.Debit = 0;
                    }
                    account.Debit -= item.Debit;
                }
                unitOfWork.AccountRepository.Update(account);
            }
        }


    }
}
