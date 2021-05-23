using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaEFMariaDB.Models
{
    public partial class AuthUser
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint HashAlgoId { get; set; }
        public string HashPwd { get; set; }

        public virtual AuthHashAlgo HashAlgo { get; set; }
    }
}
