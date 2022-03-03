using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class LikeDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
    }
}
