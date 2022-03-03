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
    public interface UserInterface
    {
        Task<ApplicationUser> GetUserByIdAsync(int id);
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
        void UpdateUser(ApplicationUser user);
        Task<ApplicationUser> GetUserByUsernameAsync(string username);
        Task<bool> SaveAllAsync();

        // imroved method
        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);
        Task<MemberDTO> GetMemberByUsernameAsync(string username);
    }
}
