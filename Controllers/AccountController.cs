using API.DB.Repositories;
using API.Models;
using API.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class AccountController : ControllerBase
    {
        private readonly IRepository<Customer> _repo;

        public AccountController(IRepository<Customer> repo)
        {
            _repo = repo;
        }
        [HttpPost]
        public async Task<ActionResult<AccountVM>> Login(AccountVM user)
        {
           
            var old = await _repo.GetAsync(x => x.Mobile == user.Mobile);
            if (old.Any())
            {
                var currentUser = old.FirstOrDefault();
                user.Name = currentUser.Name;
                user.Id = currentUser.Id;
                user.Email = currentUser.Email;
                return user;
            }

            ModelState.AddModelError("Mobile", "this number not exist");
            return BadRequest(ModelState);

        }
    }
}
