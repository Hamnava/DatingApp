using Business.Repository.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Threading.Tasks;

namespace API.Controllers
{
  
    public class UserController : BaseAPIController
    {
        private readonly UserInterface _context;
        public UserController(UserInterface context)
        {
            _context = context;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable> UserList()
        {
            return await _context.GetUsers();
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<ApplicationUser> GetUser(int id)
        {
            return await _context.GetUserById(id);
        }
    }
}
