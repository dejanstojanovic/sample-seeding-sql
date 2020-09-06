using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.Seeding.Data.Infrastructure;
using Sample.Seeding.Domain;

namespace Sample.Seeding.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        readonly EmployeesDatabaseContext _employeesDatabaseContext;
        public EmployeesController(EmployeesDatabaseContext employeesDatabaseContext)
        {
            _employeesDatabaseContext = employeesDatabaseContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Employee>> Get()
        {
            return await _employeesDatabaseContext.Employees.ToListAsync();
        }
    }
}
