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
using API.ViewModels;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class ComplainsController : ControllerBase
    {
        private readonly IRepository<Complain> _repo;

        public ComplainsController(IRepository<Complain> repo)
        {
            _repo = repo;
        }

        // GET: api/Complains
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplainVM>>> GetComplains()
        {
            var list = await _repo.GetAsync(null, "Customer", null);
            return list.Select(a => new ComplainVM()
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer.Name,
                Createddate = a.Createddate,
                Description = a.Description,
                IsCompleted = a.IsCompleted,
                Title   =a.Title
            }).ToList();
        }

        // GET: api/Complains/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Complain>> GetComplain(int id)
        {

            var Complain = await _repo.GetByIdAsync(id);

            if (Complain == null)
            {
                return NotFound();
            }

            return Complain;
        }

        // PUT: api/Complains/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComplain(int id, Complain Complain)
        {
            if (id != Complain.Id)
            {
                return BadRequest();
            }

            await _repo.UpdateAsync(Complain);

            return NoContent();
        }

        // POST: api/Complains
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Complain>> PostComplain(Complain Complain)
        {
            Complain.Createddate = DateTime.Now;
            await _repo.InsertAsync(Complain);

            return CreatedAtAction("GetComplain", new { id = Complain.Id }, Complain);
        }

        // DELETE: api/Complains/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplain(int id)
        {
            var Complain = await _repo.GetByIdAsync(id);
            if (Complain == null)
            {
                return NotFound();
            }

            await _repo.DeleteAsync(id);

            return NoContent();
        }
    }
}
