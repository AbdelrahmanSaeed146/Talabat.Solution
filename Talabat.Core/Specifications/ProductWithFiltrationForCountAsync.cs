using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithFiltrationForCountAsync :BaseSpecifications<Product>
    {
        public ProductWithFiltrationForCountAsync(ProductSpecParams Params)
              : base
            (
                 p =>
                 (string.IsNullOrEmpty(Params.Search) || p.Name.ToLower().Contains(Params.Search))
                 &&
                 (!Params.brandId.HasValue || p.BrandId == Params.brandId)
                 &&
                 (!Params.categoryId.HasValue || p.CategoryId == Params.categoryId)
            )
        {
            
        }
    }
}
