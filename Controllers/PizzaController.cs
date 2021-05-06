using DotNetFilters.Resources;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DotNetFilters.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaController : ControllerBase
    {
        private readonly IPizzaService _pizzaService;

        public PizzaController(IPizzaService pizzaService)
        {
            _pizzaService = pizzaService;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Pizza>> GetPizzas()
        {
            return Ok(new List<Pizza>
            {
                new Pizza { Id = Guid.NewGuid(), Name = "Pepperoni", Price = 5.50f },
                new Pizza { Id = Guid.NewGuid(), Name = "Funghi", Price = 7.50f }
            });
        }

        [HttpPost("pizzaId:guid")]
        public ActionResult PlaceOrder(Guid pizzaId, [FromBody] Address address)
        {
            _pizzaService.PlacePizzaOrder(pizzaId, address);
            return Ok();
        }


    }





















    public interface IPizzaService
    {
        void PlacePizzaOrder(Guid pizzaId, Address address);
    }
    public class PizzaService : IPizzaService
    {
        private readonly int[] _allowedPostalCodeRange = new int[] { 100, 200 };
        private readonly string[] _allowedCities = new string[] { "New York", "Chicago" };
        private readonly IPizzaRepository _pizzaRepository;

        public PizzaService(IPizzaRepository pizzaRepository)
        {
            _pizzaRepository = pizzaRepository;
        }

        public void PlacePizzaOrder(Guid pizzaId, Address address)
        {
            // apply rules
            ValidateAddress(address);

            // persist order
            _pizzaRepository.AddOrder(pizzaId, address);
        }

        private void ValidateAddress(Address address)
        {
            if (!_allowedPostalCodeRange.Contains(address.PostalCode))
            {
                throw new PostalCodeNotAllowedException(address.PostalCode.ToString());
            }
            if (!_allowedCities.Contains(address.City))
            {
                throw new CityNotAllowedException(address.City);
            }
        }
    }




    public interface IPizzaRepository
    {
        void AddOrder(Guid pizzaId, Address address);
    }
    public class PizzaRepository : IPizzaRepository
    {
        public void AddOrder(Guid pizzaId, Address address)
        {
            // 
        }
    }










    [Serializable]
    public class PostalCodeNotAllowedException : Exception
    {
        public PostalCodeNotAllowedException(string message)
            : base(message)
        {
        }

        protected PostalCodeNotAllowedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }













    [Serializable]
    public class CityNotAllowedException : Exception
    {
        public CityNotAllowedException(string message)
            : base(message)
        {
        }

        protected CityNotAllowedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }



    [Serializable]
    public class OrderException : Exception
    {
        public OrderException(string message)
            : base(message)
        {
        }

        protected OrderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
