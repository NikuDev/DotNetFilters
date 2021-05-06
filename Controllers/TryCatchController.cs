using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DotNetFilters.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TryCatchController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IEmployeeService _employeeService;
        private readonly IAuthorizationRequirement _updateAuthorizationRequirement = new OperationAuthorizationRequirement { Name = "Update" };
        private readonly IMapper _mapper;

        public TryCatchController(IAuthorizationService authorizationService, IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        [HttpPut]
        public ActionResult UpdateEmployee([FromBody] UpdateEmployeeModel updateEmployeeModel)
        {
            var employee = _employeeService.Get(updateEmployeeModel.EmployeeId);

            if (employee == null)
            {
                return NotFound();
            }

            try
            {
                _employeeService.UpdateEmployee(employee, updateEmployeeModel);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

            var updatedEmployee = _mapper.Map<Employee, EmployeeModel>(employee);

            return Ok(updatedEmployee);
        }

        //[HttpPut]
        //public ActionResult UpdateEmployee([FromBody] UpdateEmployeeModel updateEmployeeModel)
        //{
        //    var employee = _employeeService.Get(updateEmployeeModel.EmployeeId);
        //    _employeeService.UpdateEmployee(employee, updateEmployeeModel);
        //    var updatedEmployee = _mapper.Map<Employee, EmployeeModel>(employee);

        //    return Ok(updatedEmployee);
        //}


        //[HttpPut]
        //public ActionResult UpdateEmployee([FromBody] UpdateEmployeeModel updateEmployeeModel)
        //{
        //    var employee = _employeeService.Get(updateEmployeeModel.EmployeeId);

        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    _employeeService.UpdateEmployee(employee, updateEmployeeModel);
        //    var updatedEmployee = _mapper.Map<Employee, EmployeeModel>(employee);

        //    return Ok(updatedEmployee);
        //}



        [HttpPut]
        public async Task<ActionResult> UpdateEmployeeAsync([FromBody] UpdateEmployeeModel updateEmployeeModel)
        {
            var employee = _employeeService.Get(updateEmployeeModel.EmployeeId);

            if (employee == null)
            {
                return NotFound();
            }

            if (!(await _authorizationService.AuthorizeAsync(User, employee, _updateAuthorizationRequirement)).Succeeded)
            {
                return Forbid();
            }

            try
            {
                _employeeService.UpdateEmployee(employee, updateEmployeeModel);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }

            var updatedEmployee = _mapper.Map<Employee, EmployeeModel>(employee);

            return Ok(updatedEmployee);
        }
    }


    public interface IEmployeeService
    {
        Employee Get(Guid employeeId);
        Employee UpdateEmployee(Employee employee, UpdateEmployeeModel updateModel);
    }

    internal class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public Employee Get(Guid employeeId)
        {
            return _employeeRepository.GetAsync(employeeId);
        }

        public Employee UpdateEmployee(Employee employee, UpdateEmployeeModel updateModel)
        {
            employee.Name = updateModel.Name;
            employee.Age = updateModel.Age;
            _employeeRepository.Update(employee);
            return employee;
        }
    }

    internal interface IEmployeeRepository
    {
        Task<Employee> GetAsync(Guid employeeId);
        Employee Update(Employee employee);
    }

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DatabaseContext _databaseContext;

        public EmployeeRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<Employee> GetAsync(Guid employeeId)
        {
            // Throws an InvalidOperationException if no result is found
            var employee = await _databaseContext.Employees.SingleAsync(e => e.Id == employeeId);

            // or a bit more explicit..
            employee = await _databaseContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                throw new InvalidOperationException();
            }

            return employee;
        }



        public Employee Update(Employee employee)
        {
            throw new NotImplementedException();
        }
    }

    public class Employee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
    internal class EmployeeModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class UpdateEmployeeModel
    {
        public Guid EmployeeId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
