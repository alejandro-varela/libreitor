using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaEFMariaDB.Models
{
    public partial class AuthHashAlgo
    {
        public AuthHashAlgo()
        {
            AuthUsers = new HashSet<AuthUser>();
        }

        public uint Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<AuthUser> AuthUsers { get; set; }
    }
}
