using Business.Models;
using Business.PublicClasses;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository.Interface
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);

        Task<Message> GetMessageById(int id);
        Task<PagedList<MessageDto>> GetMessageForUser();
        Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipeintId);
        Task<bool> SaveAllAsync();
    }
}
