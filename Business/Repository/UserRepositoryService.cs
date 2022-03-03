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
    public class UserRepositoryService : UserInterface
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        public UserRepositoryService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberByUsernameAsync(string username)
        {
            return await _context.Users.Where(x => x.UserName == username)
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears( - userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(- userParams.MinAge);

            query = query.Where(u=> u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
               "created" => query.OrderByDescending(u=> u.Created),
               _ => query.OrderByDescending(u=> u.LastActive)
            };


            return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(_mapper
                .ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber,userParams.pageSize);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(x=> x.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _context.Users.Include(x=> x.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdateUser(ApplicationUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
