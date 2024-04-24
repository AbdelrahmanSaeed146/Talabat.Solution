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
                 (!Params.brandId.HasValue || p.BrandId == Params.brandId)
                 &&
                 (!Params.cateId.HasValue || p.CategoryId == Params.cateId)
            )
        {
            
        }
    }
}
