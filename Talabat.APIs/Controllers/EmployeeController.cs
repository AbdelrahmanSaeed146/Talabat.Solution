using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications.EmployeeSpecs;

namespace Talabat.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : BaseApiController
    {
        private readonly IGenericRepository<Employee> _empReo;

        public EmployeeController(IGenericRepository<Employee> EmpReo )
        {
            _empReo = EmpReo;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var spec = new EmployeeWithDepartmentSpecs();
            var employees = await _empReo.GetAllWithSpecAsync(spec);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var spec = new EmployeeWithDepartmentSpecs(id);
            var employee = await _empReo.GetByIdWithSpecAsync(spec);
            if (employee is null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

    }
}
