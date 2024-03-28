using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } // RevokeRefreshToken metodunda bu property üzerinden silinecek token'ı alacağız

    }
}
