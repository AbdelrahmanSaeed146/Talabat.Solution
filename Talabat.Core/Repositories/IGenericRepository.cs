using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);   
        Task<IReadOnlyList<T?>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec);
        Task<T> GetByIdWithSpecAsync(ISpecifications<T> Spec);
        public Task<int> GetCountWithSpecAsync(ISpecifications<T> spec);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
