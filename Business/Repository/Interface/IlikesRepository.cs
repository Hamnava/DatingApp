using Business.Models;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository.Interface
{
    public interface IlikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<ApplicationUser> GetUserWithLikes(int userId);
        Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId);
    }
}
