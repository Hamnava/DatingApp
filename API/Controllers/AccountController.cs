using AutoMapper;
using Business.Models;
using Business.Repository.Interface;
using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly ApplicationContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(ApplicationContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(UserRegisterDTO registerDTO)
        {
            if (await UserExist(registerDTO.Username))
            {
                return BadRequest("Username already exists!");
            }
            var user = _mapper.Map<ApplicationUser>(registerDTO);
            using var hmac = new HMACSHA512();

            user.UserName = registerDTO.Username;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            user.PasswordSalt = hmac.Key;
            

             _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Username = registerDTO.Username,
                Token = _tokenService.GetToken(user),
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.Include(x=> x.Photos).SingleOrDefaultAsync(u=> u.UserName == loginDTO.Username);
            if (user == null) return Unauthorized("Invalid Username!");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var hashpassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < hashpassword.Length; i++)
            {
                if (hashpassword[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password!");
            }
            return new UserDTO
            {
                Username = loginDTO.Username,
                Token = _tokenService.GetToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(u=> u.UserName == username.ToLower());
        }
    }
}
