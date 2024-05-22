using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Services;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.Service
{

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams Params)
        {
            var Spec = new ProductWithBrandAndCategorySpecifications(Params);

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
            return products;
        }

        public async Task<Product?> GetProductAsync(int productId)
        {

            var spec = new ProductWithBrandAndCategorySpecifications(productId);
            var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);

            return product;
        }

        public async Task<int> GetCountAsync(ProductSpecParams Params)
        {
            var CountSpec = new ProductWithFiltrationForCountAsync(Params);

            var Count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);

            return Count;

        }





        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
      => await _unitOfWork.Repository<ProductBrand>().GetAllAsync();



        public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
       => await _unitOfWork.Repository<ProductCategory>().GetAllAsync();



    }
}
