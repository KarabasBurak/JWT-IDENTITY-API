using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
    public class Client
    {
        public string Id { get; set; } // ClientId; ClientSecret'a karşılık gelecek
        public string Secret { get; set; }
        public List<String> Audiences { get; set; } // Audiences; Client'ın hangi API'lere erişimi olacağı bilgisini liste şeklinde Audiences property'sinde tutacağız

    }
}


// Client, AuthServer'a istekte bulunacak. Model veya entity değil.
// İstekte bulunurken Id, Secret, Audiences propları istek atacak
