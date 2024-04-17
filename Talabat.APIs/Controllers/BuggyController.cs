using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _dbcontext;

        public BuggyController(StoreContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbcontext.Products.Find(100);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var product = _dbcontext.Products.Find(100);
            var ProductToReturn = product.ToString();
       
            return Ok(ProductToReturn);
        }



        [HttpGet("badrequset")]
        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }



        [HttpGet("badrequset/{id}")]
        public ActionResult GetBadRequestById(int id ) // Validation Error
        {
            return Ok();
        }

    }
}
