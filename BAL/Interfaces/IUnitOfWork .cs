using BAL.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        bool Save();

        GenericRepository<T> Repository<T>() where T : class, new();

        Task<bool> SaveAsync();

    }
}
