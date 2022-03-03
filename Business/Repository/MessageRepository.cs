using AutoMapper;
using AutoMapper.QueryableExtensions;
using Business.Models;
using Business.PublicClasses;
using Business.Repository.Interface;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

       

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

       

        public async Task<Message> GetMessageById(int id)
        {
            return await _context.Messages
                .Include(u=> u.Sender)
                .Include(u=> u.Recipeint)
                .SingleOrDefaultAsync(u=> u.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipeint.UserName == messageParams.Username && u.RecipeintDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipeint.UserName == messageParams.Username && u.RecipeintDeleted == false && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages,
                  messageParams.PageNumber, messageParams.pageSize);
        }

       

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipeintUsername)
        {
            var messages = await _context.Messages
                .Include(u=> u.Sender).ThenInclude(p=> p.Photos)
                .Include(r=> r.Recipeint).ThenInclude(p=> p.Photos)
                .Where(m=> m.Recipeint.UserName== currentUsername && m.RecipeintDeleted == false &&
                       m.Sender.UserName == recipeintUsername ||
                       m.Recipeint.UserName == recipeintUsername && m.SenderDeleted == false && 
                       m.Sender.UserName == currentUsername)
                .OrderBy(m=> m.MessageSent).ToListAsync();

            var UnreadMessage = messages.Where(m => m.DateRead == null
                               && m.Recipeint.UserName == currentUsername).ToList();

            if (UnreadMessage.Any())
            {
                foreach (var message in UnreadMessage)
                {
                    message.DateRead = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        #region SignalR ConnectionGroup

        
        public async Task<Connection> GetConnectionAsync(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }
        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }
        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
        public async Task<Group> GetMessageGroup(string groupName)
        {
           return await _context.Groups.Include(x=> x.Connections)
                .FirstOrDefaultAsync(x=> x.Name == groupName);    
        }

        #endregion

    }
}
