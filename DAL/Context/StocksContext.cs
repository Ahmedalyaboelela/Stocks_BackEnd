using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Context
{
   public class StocksContext : DbContext
    {
        public StocksContext(DbContextOptions<StocksContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Partner> Partners { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeCard> EmployeeCards { get; set; }

        public DbSet<Portfolio> Portfolios { get; set; }

        public DbSet<Portfolioshareholder> Portfolioshareholders { get; set; }

        public DbSet<PortfolioAccount> PortfolioAccounts { get; set; }
    }
}
