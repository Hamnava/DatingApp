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
    public class UserRepositoryService : UserInterface
    {
        private readonly ApplicationContext _context;
        public UserRepositoryService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
