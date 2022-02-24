using AutoMapper;
using Business.Models;
using Business.Repository.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
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

        
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUserByUsername(string username)
        {
            return await _context.GetMemberByUsernameAsync(username);
           
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(UpdateMemberDto memberDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.GetUserByUsernameAsync(username);
            _mapper.Map(memberDto, user);

             _context.UpdateUser(user);
             //await _context.SaveAllAsync();

            if (await _context.SaveAllAsync()) return NoContent();

            return BadRequest("Faild to Update user!");
        }
    }
}
