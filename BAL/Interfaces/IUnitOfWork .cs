using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        bool Save();

        Task<bool> SaveAsync();

    }
}
