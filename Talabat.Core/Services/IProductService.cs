using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Services
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams Params);

        Task<int> GetCountAsync(ProductSpecParams Params);  

        Task<Product?> GetProductAsync(int productId);
        
        Task<IReadOnlyList<ProductBrand>> GetBrandsAsync();

        Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync();
    }
}
