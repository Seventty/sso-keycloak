using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Dtos
{
    public class TokenResponse
    {
        public string access_token { get; set; } = default!;
        public string refresh_token { get; set; } = default!;
        public int expires_in { get; set; }
    }

}
