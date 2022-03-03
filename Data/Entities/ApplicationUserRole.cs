using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class ApplicationUserRole: IdentityUserRole<int>
    {
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
