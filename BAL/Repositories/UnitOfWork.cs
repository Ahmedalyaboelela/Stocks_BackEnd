﻿using BAL.Interfaces;
using DAL.Context;
using DAL.Entities;
using System;
using System.Collections.Generic;
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


        public virtual bool Save()
        {
            bool returnValue = true;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
                //  {
                try
                {
                    Context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    //Log Exception Handling message                      
                    returnValue = false;
                    dbContextTransaction.Rollback();
                }
            //    }

            return returnValue;
        }

        public virtual async Task<bool> SaveAsync()
        {
            bool returnValue = true;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    returnValue = false;
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
