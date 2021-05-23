using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaEFMariaDB.Models
{
    public partial class AuthUserRole
    {
        public uint UserId { get; set; }
        public uint RoleId { get; set; }

        public virtual AuthRole Role { get; set; }
        public virtual AuthUser User { get; set; }
    }
}
