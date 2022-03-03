using AutoMapper;
using Business.Models;
using Business.PublicClasses;
using Business.Repository.Interface;
using Data.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly UserInterface _userRepository;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper, UserInterface userInterface)
        {
            _messageRepository = messageRepository;
            _userRepository = userInterface;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddTGroup(groupName);

            var message = await _messageRepository
                .GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(CreateMessageDto messageDto)
        {
            var username = Context.User.GetUsername();
            if (username == messageDto.RecipeintUsername) throw new HubException("You can not send message to yourself!");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipeint = await _userRepository.GetUserByUsernameAsync(messageDto.RecipeintUsername);

            if (recipeint == null) throw new HubException("Not Found User");

            var message = new Message
            {
                Sender = sender,
                Recipeint = recipeint,
                SenderUsername = sender.UserName,
                RecepeintUsername = recipeint.UserName,
                Content = messageDto.Content
            };

            // update the Date Read of a Message

            var groupName = GetGroupName(sender.UserName, recipeint.UserName);
            var group = await _messageRepository.GetMessageGroup(groupName);

            if(group.Connections.Any(x=> x.Username == recipeint.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }


            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }

        }
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<bool> AddTGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await _messageRepository.SaveAllAsync();
        }

        private async Task RemoveFromMessageGroup()
        {
            var connection = await _messageRepository.GetConnectionAsync(Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            await _messageRepository.SaveAllAsync();
        }
    }
}
