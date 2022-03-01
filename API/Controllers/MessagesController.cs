using Business.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseAPIController
    {
        private readonly UserInterface _userRepository;
        private readonly IMessageRepository _messageRepository;
        public MessagesController(UserInterface userInterface, IMessageRepository messageRepository)
        {
            _userRepository = userInterface;
            _messageRepository = messageRepository;
        }

        

    }
}
