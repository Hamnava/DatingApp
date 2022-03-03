using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class UserLike
    {
        public ApplicationUser SourceUser { get; set; }
        public int SourceUserId { get; set; }

        public ApplicationUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}
