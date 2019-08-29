using BAL.Interfaces;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StocksContext Context;


        public UnitOfWork(StocksContext dbContext)
        {
            Context = dbContext;
        }

        public GenericRepository<T> Repository<T>() where T : class, new()
        {
            return new GenericRepository<T>(Context);
        }

        #region Account repository
        //Account
        private GenericRepository<Account> accountRepository;
        public GenericRepository<Account> AccountRepository
        {
            get
            {

                if (this.accountRepository == null)
                {
                    this.accountRepository = new GenericRepository<Account>(Context);
                }
                return accountRepository;
            }
        }
        #endregion

        #region Partner repository
       
        private GenericRepository<Partner> partnerRepository;
        public GenericRepository<Partner> PartnerRepository
        {
            get
            {

                if (this.partnerRepository == null)
                {
                    this.partnerRepository = new GenericRepository<Partner>(Context);
                }
                return partnerRepository;
            }
        }
        #endregion

        #region Country repository

        private GenericRepository<Country> countryRepository;
        public GenericRepository<Country> CountryRepository
        {
            get
            {

                if (this.countryRepository == null)
                {
                    this.countryRepository = new GenericRepository<Country>(Context);
                }
                return countryRepository;
            }
        }
        #endregion

        #region Employee Repository
        private GenericRepository<Employee> employeeRepository;
        public GenericRepository<Employee> EmployeeRepository
        {
            get
            {
                if (this.employeeRepository == null)
                {
                    this.employeeRepository = new GenericRepository<Employee>(Context);
                }
                return employeeRepository;
            }
        }

        private GenericRepository<EmployeeCard> employeeCardRepository;
        public GenericRepository<EmployeeCard> EmployeeCardRepository
        {
            get
            {
                if (this.employeeCardRepository == null)
                {
                    this.employeeCardRepository = new GenericRepository<EmployeeCard>(Context);
                }
                return employeeCardRepository;
            }
        }
        #endregion

        #region Portfolio Repository
        private GenericRepository<Portfolio> portfolioRepository;
        public GenericRepository<Portfolio> PortfolioRepository
        {
            get
            {
                if (this.portfolioRepository == null)
                {
                    this.portfolioRepository = new GenericRepository<Portfolio>(Context);
                }
                return portfolioRepository;
            }
        }

        private GenericRepository<PortfolioAccount> portfolioAccountRepository;
        public GenericRepository<PortfolioAccount> PortfolioAccountRepository
        {
            get
            {
                if (this.portfolioAccountRepository == null)
                {
                    this.portfolioAccountRepository = new GenericRepository<PortfolioAccount>(Context);
                }
                return portfolioAccountRepository;
            }
        }

        private GenericRepository<PortfolioOpeningStocks> portfolioOpeningStocksRepository;
        public GenericRepository<PortfolioOpeningStocks> PortfolioOpeningStocksRepository
        {
            get
            {
                if (this.portfolioOpeningStocksRepository == null)
                {
                    this.portfolioOpeningStocksRepository = new GenericRepository<PortfolioOpeningStocks>(Context);
                }
                return portfolioOpeningStocksRepository;
            }
        }

        private GenericRepository<SellingOrder> sellingOrderRepository;
        public GenericRepository<SellingOrder> SellingOrderReposetory
        {
            get
            {
                if (this.sellingOrderRepository == null)
                {
                    this.sellingOrderRepository = new GenericRepository<SellingOrder>(Context);
                }
                return sellingOrderRepository;
            }
        }

        private GenericRepository<SellingOrderDetail> sellingOrderDetailRepository;
        public GenericRepository<SellingOrderDetail> SellingOrderDetailRepository
        {
            get
            {
                if (this.sellingOrderDetailRepository == null)
                {
                    this.sellingOrderDetailRepository = new GenericRepository<SellingOrderDetail>(Context);
                }
                return sellingOrderDetailRepository;
            }
        }




        #endregion

        #region Setting Repository
        private GenericRepository<Setting> settingRepository;
        public GenericRepository<Setting> SettingRepository
        {
            get
            {
                if (this.settingRepository == null)
                {
                    this.settingRepository = new GenericRepository<Setting>(Context);
                }
                return settingRepository;
            }
        }

        private GenericRepository<SettingAccount> settingAccountRepository;
        public GenericRepository<SettingAccount> SettingAccountRepository
        {
            get
            {
                if (this.settingAccountRepository == null)
                {
                    this.settingAccountRepository = new GenericRepository<SettingAccount>(Context);
                }
                return settingAccountRepository;
            }
        }



        private GenericRepository<PurchaseOrder> purchaseOrderRepository;
        public GenericRepository<PurchaseOrder> PurchaseOrderRepository
        {
            get
            {
                if (this.purchaseOrderRepository == null)
                {
                    this.purchaseOrderRepository = new GenericRepository<PurchaseOrder>(Context);
                }
                return purchaseOrderRepository;
            }
        }

        private GenericRepository<PurchaseOrderDetail> purchaseOrderDetailRepository;
        public GenericRepository<PurchaseOrderDetail> PurchaseOrderDetailRepository
        {
            get
            {
                if (this.purchaseOrderDetailRepository == null)
                {
                    this.purchaseOrderDetailRepository = new GenericRepository<PurchaseOrderDetail>(Context);
                }
                return purchaseOrderDetailRepository;
            }
        }

        #endregion

        #region ReceiptExchange Repository
        private GenericRepository<ReceiptExchange> receiptExchangeRepository;
        public GenericRepository<ReceiptExchange> ReceiptExchangeRepository
        {
            get
            {
                if (this.receiptExchangeRepository == null)
                {
                    this.receiptExchangeRepository = new GenericRepository<ReceiptExchange>(Context);
                }
                return receiptExchangeRepository;
            }
        }

        private GenericRepository<ReceiptExchangeDetail> receiptExchangeDetailRepository;
        public GenericRepository<ReceiptExchangeDetail> ReceiptExchangeDetailRepository
        {
            get
            {
                if (this.receiptExchangeDetailRepository == null)
                {
                    this.receiptExchangeDetailRepository = new GenericRepository<ReceiptExchangeDetail>(Context);
                }
                return receiptExchangeDetailRepository;
            }
        }


        #endregion

        #region Notice Repository
        private GenericRepository<Notice> noticeRepository;
        public GenericRepository<Notice> NoticeRepository
        {
            get
            {
                if (this.noticeRepository == null)
                {
                    this.noticeRepository = new GenericRepository<Notice>(Context);
                }
                return noticeRepository;
            }
        }

        private GenericRepository<NoticeDetail> noticeDetailRepository;
        public GenericRepository<NoticeDetail> NoticeDetailRepository
        {
            get
            {
                if (this.noticeDetailRepository == null)
                {
                    this.noticeDetailRepository = new GenericRepository<NoticeDetail>(Context);
                }
                return noticeDetailRepository;
            }
        }


        #endregion


        #region Entry Repository
        private GenericRepository<Entry> entryRepository;
        public GenericRepository<Entry> EntryRepository
        {
            get
            {
                if (this.entryRepository == null)
                {
                    this.entryRepository = new GenericRepository<Entry>(Context);
                }
                return entryRepository;
            }
        }

        private GenericRepository<EntryDetail> entryDetailRepository;
        public GenericRepository<EntryDetail> EntryDetailRepository
        {
            get
            {
                if (this.entryDetailRepository == null)
                {
                    this.entryDetailRepository = new GenericRepository<EntryDetail>(Context);
                }
                return entryDetailRepository;
            }
        }

        private GenericRepository<Currency> currencyRepository;
        public GenericRepository<Currency> CurrencyRepository
        {
            get
            {
                if (this.currencyRepository == null)
                {
                    this.currencyRepository = new GenericRepository<Currency>(Context);
                }
                return currencyRepository;
            }
        }



        #endregion



        #region PortfolioTransactions Repository
        private GenericRepository<PortfolioTransaction> portfolioTransactionsRepository;
        public GenericRepository<PortfolioTransaction> PortfolioTransactionsRepository
        {
            get
            {
                if (this.portfolioTransactionsRepository == null)
                {
                    this.portfolioTransactionsRepository = new GenericRepository<PortfolioTransaction>(Context);
                }
                return portfolioTransactionsRepository;
            }
        }
        #endregion

       
        #region ReportSetting Repository
        private GenericRepository<ReportSetting> reportSettingRepository;
        public GenericRepository<ReportSetting> ReportSettingRepository
        {
            get
            {
                if (this.reportSettingRepository == null)
                {
                    this.reportSettingRepository = new GenericRepository<ReportSetting>(Context);
                }
                return reportSettingRepository;
            }
        }
        #endregion


        public virtual int Save()
        {
            int returnValue = 200;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
                //  {
                try
                {
                    Context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null)
                    {
                        var number = sqlException.Number;

                        if (number == 547)
                        {
                            returnValue = 501;

                        }
                        else
                            returnValue = 500;
                    }
                }
                catch (Exception ex)
                {
                    //Log Exception Handling message                      
                    returnValue = 500;
                    dbContextTransaction.Rollback();
                }
            //    }

            return returnValue;
        }

        public virtual async Task<int> SaveAsync()
        {
            int returnValue = 200;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null)
                    {
                        var number = sqlException.Number;

                        if (number == 547)
                        {
                            returnValue = 501;

                        }
                        else
                            returnValue = 500;
                    }
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    returnValue = 500;
                    dbContextTransaction.Rollback();
                }
            }

            return returnValue;
        }

        private bool disposed = false;



        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
