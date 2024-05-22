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

        public ProductWithBrandAndCategorySpecifications(ProductSpecParams Params) 
            :base
            (
                 p=>
                (string.IsNullOrEmpty(Params.Search) || p.Name.ToLower().Contains(Params.Search))
                 &&
                 (!Params.brandId.HasValue || p.BrandId == Params.brandId)
                 &&
                 (!Params.categoryId.HasValue || p.CategoryId == Params.categoryId)
            )
        {
            AddIncludes();

            if (!string.IsNullOrEmpty(Params.Sort))
            {
                switch(Params.Sort)
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


            ApplyPagination(Params.PageSize * (Params.PageIndex - 1) , Params.PageSize);
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
