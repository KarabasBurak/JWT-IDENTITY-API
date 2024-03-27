using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Configurations
{
    // Client ile AuthServer arasında aşağıdaki propertyler ile veri alış-verişi yapacak. DB ile bağlantısı yok
    public class CustomTokenDto
    {
        public List<String> Audience { get; set; } // birden fazla olabilir o yüzden list
        public string Issuer { get; set; }

        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }

        public string SecurityKey { get; set; }


    }
}
