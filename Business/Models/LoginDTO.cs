using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Username feild should not be empty!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password field should not be empty")]
        public string Password { get; set; }
    }
}
