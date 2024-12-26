using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class KeycloakAuthCodeDto
    {
        public string AuthorizationCode { get; set; }
        public string RedirectUri { get; set; }
    }

}
