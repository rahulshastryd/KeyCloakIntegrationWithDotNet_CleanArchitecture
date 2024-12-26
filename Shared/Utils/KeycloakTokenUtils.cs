using Shared.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dtos;

namespace Shared.Utils
{
    public static class KeycloakTokenUtils
    {
        public static FormUrlEncodedContent GetTokenRequestBody(
            KeycloakTokenRequestDto keycloakTokenDto)
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(
                    KeycloakAccessTokenConsts.GrantType, keycloakTokenDto.GrantType),
                new KeyValuePair<string, string>(
                    KeycloakAccessTokenConsts.ClientId, keycloakTokenDto.ClientId),
                new KeyValuePair<string, string>(
                    KeycloakAccessTokenConsts.ClientSecret, keycloakTokenDto.ClientSecret),
                new KeyValuePair<string, string>(
                    KeycloakAccessTokenConsts.Username, keycloakTokenDto.Username),
                new KeyValuePair<string, string>(
                    KeycloakAccessTokenConsts.Password, keycloakTokenDto.Password)
            };
            return new FormUrlEncodedContent(keyValuePairs);
        }
    }
}
