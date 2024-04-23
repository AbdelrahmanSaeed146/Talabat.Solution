using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {

        public ProductWithBrandAndCategorySpecifications(string? sort, int? brandId, int? cateid) 
            :base
            (
                 p=> 
                 (!brandId.HasValue || p.BrandId == brandId)
                 &&
                 (!cateid.HasValue || p.CategoryId == cateid)
            )
        {
            AddIncludes();

            if (!string.IsNullOrEmpty(sort))
            {
                switch(sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;  

                }
            }

            else
                AddOrderBy(p=>p.Name);
        }

     

        public ProductWithBrandAndCategorySpecifications(int id):base(p => p.Id ==id )
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }

    }
}
