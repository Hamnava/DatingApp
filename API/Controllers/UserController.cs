using API.Extentions;
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
        private readonly IPhotoService _photoService;
        public UserController(UserInterface context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> UserList()
        {
            var users = await _context.GetMembersAsync();
            return Ok(users);
            
        }

        
        [HttpGet("{username}" , Name ="GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUserByUsername(string username)
        {
            return await _context.GetMemberByUsernameAsync(username);
           
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(UpdateMemberDto memberDto)
        {
           
            var user = await _context.GetUserByUsernameAsync(User.GetUsername());
            _mapper.Map(memberDto, user);

             _context.UpdateUser(user);

            if (await _context.SaveAllAsync()) return NoContent();

            return BadRequest("Faild to Update user!");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await _context.GetUserByUsernameAsync(User.GetUsername());
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count  == 0)
            {
                photo.IsMain = true;
            };

            user.Photos.Add(photo);
            if (await _context.SaveAllAsync()) 
            {
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Problem during adding photo");
        }
    }
}
