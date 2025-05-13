using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Interfaces
{
    public interface IBaseRepository<T> : IDisposable
    {

        public Task<IEnumerable<T>> GetAllAsync();

        public Task<T> GetByIdAsync(int id);

        public Task<T> CreateAsync(T entity);

        public Task<bool> UpdateAsync(T entity);

        public Task<bool> DeleteAsync(T entity);
    }
}