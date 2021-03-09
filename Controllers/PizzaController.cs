using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetFilters.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetException()
        {
            throw new Exception();
            return Ok();
        }

    }
}
