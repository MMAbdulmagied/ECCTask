using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DB;
using API.Models;
using Microsoft.AspNetCore.Cors;
using API.DB.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<Customer> _repo;

        public CustomersController(IRepository<Customer> repo)
        {
            _repo = repo;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {

            return await _repo.GetAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {

            var Customer = await _repo.GetByIdAsync(id);

            if (Customer == null)
            {
                return NotFound();
            }

            return Customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer Customer)
        {
            if (id != Customer.Id)
            {
                return BadRequest();
            }

            await _repo.UpdateAsync(Customer);

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer Customer)
        {
            var old = await _repo.GetAsync(x => x.Mobile == Customer.Mobile);
            if (old.Any())
            {
                ModelState.AddModelError("Mobile", "this number already exist");
                return BadRequest(ModelState);
            }
            else
                await _repo.InsertAsync(Customer);

            return CreatedAtAction("GetCustomer", new { id = Customer.Id }, Customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var Customer = await _repo.GetByIdAsync(id);
            if (Customer == null)
            {
                return NotFound();
            }

            await _repo.DeleteAsync(id);

            return NoContent();
        }
    }
}
