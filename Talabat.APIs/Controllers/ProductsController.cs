using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
 
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _genericRepository;

        public ProductsController(IGenericRepository<Product> genericRepository)
        {
            _genericRepository = genericRepository;
        }
    }
}
