using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }


    }
}

/*
 LoginDto içindeki prop ile taşınan Email ve Password bilgileri AuthServer'a gelecek. Veriler DB'deki veriler ile eşleşiyorsa AuthServer, Client'a AccessToken ve RefreshToken verecek
 */