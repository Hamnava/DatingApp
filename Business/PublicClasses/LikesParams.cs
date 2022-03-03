using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.PublicClasses
{
    public class LikesParams : PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}
