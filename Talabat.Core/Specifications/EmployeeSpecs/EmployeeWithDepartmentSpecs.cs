using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.EmployeeSpecs
{
    public class EmployeeWithDepartmentSpecs : BaseSpecifications<Employee>
    {

        public EmployeeWithDepartmentSpecs():base()
        {
            Includes.Add(e=>e.Department);
        }

        public EmployeeWithDepartmentSpecs(int id):base(e => e.Id == id)
        {

            Includes.Add(e => e.Department);

        }
    }
}
