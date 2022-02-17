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
        Task<ApplicationUser> GetUserById(int id);
        Task<IEnumerable<ApplicationUser>> GetUsers();
    }
}
