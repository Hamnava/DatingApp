using Business.Models;
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
        Task<IEnumerable<MemberDTO>> GetMembersAsync();
        Task<MemberDTO> GetMemberByUsernameAsync(string username);
    }
}
