using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipeintId { get; set; }
        public string RecepeintUsername { get; set; }
        public string RecipeintPhotoUrl { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } 
    }
}
