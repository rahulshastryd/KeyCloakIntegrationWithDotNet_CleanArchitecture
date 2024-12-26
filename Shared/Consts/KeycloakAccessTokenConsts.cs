using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Consts
{
    public static class KeycloakAccessTokenConsts
    {
        public static string GrantType => "grant_type";
        public static string GrantTypePassword => "password";
        public static string GrantTypeAuthCodeType => "authorization_code";
        public static string GrantTypeAuthCode => "code";
        public static string RedirectUri => "redirect_uri";
        public static string ClientId => "client_id";
        public static string ClientSecret => "client_secret";
        public static string Username => "username";
        public static string Password => "password";
    }
}
