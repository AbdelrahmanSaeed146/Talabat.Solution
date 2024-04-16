using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }


           #region Without Specifications
        //public async Task<IEnumerable<T>> GetAllAsync()
        //{
        //    if (typeof(T) == typeof(Product))
        //        return (IEnumerable<T>)await _dbContext.Set<Product>().Include(p => p.Brand).Include(p => p.Category).ToListAsync();
        //    return await _dbContext.Set<T>().ToListAsync();

        //}

        //public async Task<T?> GetAsync(int id)
        //{
        //    return await _dbContext.Set<T>().FindAsync(id);

        //} 
        #endregion

        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecification(Spec).ToListAsync();
        }

  

        public async Task<T> GetByIdWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecification(Spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecifications<T> spec)
        {
            return SpecificationEvalutor<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}
