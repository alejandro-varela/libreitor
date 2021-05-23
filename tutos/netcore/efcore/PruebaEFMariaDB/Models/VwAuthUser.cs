using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaEFMariaDB.Models
{
    public partial class VwAuthUser
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint HashAlgoId { get; set; }
        public string HashAlgo { get; set; }
        public string Hash { get; set; }
        public uint RoleId { get; set; }
        public string Role { get; set; }
        public string Path { get; set; }
    }
}
