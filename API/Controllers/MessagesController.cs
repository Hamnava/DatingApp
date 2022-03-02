using AutoMapper;
using Business.Models;
using Business.PublicClasses;
using Business.Repository.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseAPIController
    {
        private readonly UserInterface _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessagesController(UserInterface userInterface, IMessageRepository messageRepository, 
                                  IMapper mapper)
        {
            _userRepository = userInterface;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto messageDto)
        {
            var username = User.GetUsername();
            if (username == messageDto.RecipeintUsername) return BadRequest("You can not send message to yourself!");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipeint = await _userRepository.GetUserByUsernameAsync(messageDto.RecipeintUsername);

            if (recipeint == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipeint = recipeint,
                SenderUsername = sender.UserName,
                RecepeintUsername = recipeint.UserName,
                Content = messageDto.Content
            };

            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Faild to send message!");
        }
        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages);

            return messages;
            
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("id")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await _messageRepository.GetMessageById(id);

            if (message.Sender.UserName != username && message.Recipeint.UserName != username) return Unauthorized();

            if (message.Sender.UserName == username)  message.SenderDeleted = true;
            if (message.Recipeint.UserName == username)  message.RecipeintDeleted = true;

            if (message.SenderDeleted && message.RecipeintDeleted) _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();
            
            return BadRequest("Problem deleting the message");

        }

    }
}
