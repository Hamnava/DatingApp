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
        private readonly PresenceTracker _tracker;
        private readonly IHubContext<PresenceHub> _presence;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper, UserInterface userInterface
                          , PresenceTracker tracker, IHubContext<PresenceHub> presence)
        {
            _messageRepository = messageRepository;
            _userRepository = userInterface;
            _mapper = mapper;
            _tracker = tracker;
            _presence = presence;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddTGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var message = await _messageRepository
                .GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
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

            if (group.Connections.Any(x => x.Username == recipeint.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await _tracker.GetConnectionsForUser(recipeint.UserName);
                if(connections != null)
                {
                    await _presence.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }

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

        private async Task<Group> AddTGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if(await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Faild to add group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            if( await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group!");
        }
    }
}
