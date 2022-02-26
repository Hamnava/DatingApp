using API.Extentions;
using AutoMapper;
using Business.Models;
using Business.PublicClasses;
using Business.Repository.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery]UserParams userParams)
        {
            var users = await _context.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, 
                                         users.TotalCount, users.TotalPages);
            return Ok(users);

        }


        [HttpGet("{username}", Name = "GetUser")]
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
            //var result = await _photoService.AddPhotoAsync(file);

            //if (result.Error != null) return BadRequest(result.Error.Message);

            //var photo = new Photo
            //{
            //    Url = result.SecureUrl.AbsoluteUri,
            //    PublicId = result.PublicId
            //};
            //if (user.Photos.Count  == 0)
            //{
            //    photo.IsMain = true;
            //};
            if (file != null)
            {
                string imgname = PhotoUpload.CreateImg(file);
                if (imgname == "false")
                {
                    return BadRequest("problem during storing images");
                }

                var photo = new Photo
                {
                    Url = "https://localhost:44372/images/"+imgname,

                };
                if (user.Photos.Count == 0)
                {
                    photo.IsMain = true;
                };
                user.Photos.Add(photo);

                if (await _context.SaveAllAsync())
                {
                    return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
                }
            }
            return BadRequest("Problem during adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _context.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This photo is already your main photo!");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if (await _context.SaveAllAsync()) return NoContent();

            return BadRequest("Faild to update the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _context.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x=> x.Id == photoId);
            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("The main photo can not be deleted!");
            var deletUrl = photo.Url.Replace("https://localhost:44372/images/", "");
            var result = PhotoUpload.DeleteImg(deletUrl);

            user.Photos.Remove(photo);

            if (await _context.SaveAllAsync()) return Ok();

            return BadRequest("Faild delete photo!");
        }
    }
}
