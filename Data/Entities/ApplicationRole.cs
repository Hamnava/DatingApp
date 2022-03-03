﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
