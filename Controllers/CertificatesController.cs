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

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class CertificatesController : ControllerBase
    {
        private readonly IRepository<Certificate> _repo;

        public CertificatesController(IRepository<Certificate> repo)
        {
            _repo = repo;
        }

        // GET: api/Certificates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        {

            return await _repo.GetAsync();
        }

        // GET: api/Certificates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Certificate>> GetCertificate(int id)
        {

            var certificate = await _repo.GetByIdAsync(id);

            if (certificate == null)
            {
                return NotFound();
            }

            return certificate;
        }

        // PUT: api/Certificates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertificate(int id, Certificate certificate)
        {
            if (id != certificate.Id)
            {
                return BadRequest();
            }

            await _repo.UpdateAsync(certificate);

            return NoContent();
        }

        // POST: api/Certificates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Certificate>> PostCertificate(Certificate certificate)
        {

            await _repo.InsertAsync(certificate);

            return CreatedAtAction("GetCertificate", new { id = certificate.Id }, certificate);
        }

        // DELETE: api/Certificates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = await _repo.GetByIdAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            await _repo.DeleteAsync(id);

            return NoContent();
        }
    }
}
