using AutoMapper;
using Business.Models;
using Business.Repository.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class UserController : BaseAPIController
    {
        private readonly UserInterface _context;
        private readonly IMapper _mapper;
        public UserController(UserInterface context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> UserList()
        {
            var users = await _context.GetMembersAsync();
            return Ok(users);
            
        }

        //[HttpGet]
        //[Route("{id}")]
        //public async Task<ApplicationUser> GetUser(int id)
        //{
        //    return await _context.GetUserByIdAsync(id);
        //}
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUserByUsername(string username)
        {
            return await _context.GetMemberByUsernameAsync(username);
           
        }
    }
}
