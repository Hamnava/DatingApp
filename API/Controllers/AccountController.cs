using Business.Models;
using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly ApplicationContext _context;
        public AccountController(ApplicationContext context)
        {
            _context = context;
        }


        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> Register(UserRegisterDTO registerDTO)
        {
            if (await UserExist(registerDTO.Username))
            {
                return BadRequest("Username already exists!");
            }
            using var hmac = new HMACSHA512();
            var user = new ApplicationUser
            {
                UserName = registerDTO.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

             _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUser>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u=> u.UserName == loginDTO.Username);
            if (user == null) return Unauthorized("Invalid Username!");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var hashpassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < hashpassword.Length; i++)
            {
                if (hashpassword[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password!");
            }
            return user;
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(u=> u.UserName == username.ToLower());
        }
    }
}
