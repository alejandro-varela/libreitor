using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaEFMariaDB.Models
{
    public partial class AuthRolePath
    {
        public uint RoleId { get; set; }
        public uint PathId { get; set; }

        public virtual AuthPath Path { get; set; }
        public virtual AuthRole Role { get; set; }
    }
}
