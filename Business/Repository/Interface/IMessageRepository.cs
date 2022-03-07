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
        #region SigalR Connection
        Task<Group> GetGroupForConnection(string connectionId);
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnectionAsync(string connectionId);
        Task<Group> GetMessageGroup(string groupName);

        #endregion
        void AddMessage(Message message);
        void DeleteMessage(Message message);

        Task<Message> GetMessageById(int id);
        Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipeintUsername);
        Task<bool> SaveAllAsync();
    }
}
