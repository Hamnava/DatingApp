using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage ="Username feild should not be empty!")]
        public string Username { get; set; }
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required(ErrorMessage ="Password field should not be empty")]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
