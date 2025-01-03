﻿using Application.Dtos;
using Application.Interfaces;
using Newtonsoft.Json;
using Shared.Consts;
using Shared.Dtos;
using Shared.Exceptions;
using Shared.Settings;
using Shared.TokenDtos;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class KeycloakTokenService: IKeycloakTokenService
    {
        private readonly KeycloakSettings keycloakSettings;
        private readonly IHttpClientFactory httpClientFactory;
        public KeycloakTokenService(KeycloakSettings keycloakSettings, IHttpClientFactory httpClientFactory)
        {
            this.keycloakSettings = keycloakSettings;
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<KeycloakTokenResponseDto?> GetTokenResponseAsync(
            KeycloakUserDto keycloakUserDto)
        {
            using (var httpClient = httpClientFactory.CreateClient())
            {

                var keycloakTokenRequestDto = new KeycloakTokenRequestDto
                {
                    GrantType = KeycloakAccessTokenConsts.GrantTypePassword,
                    ClientId = keycloakSettings.ClientId ??
                        throw new KeycloakException(nameof(keycloakSettings.ClientId)),
                    ClientSecret = keycloakSettings.ClientSecret ??
                        throw new KeycloakException(nameof(keycloakSettings.ClientSecret)),
                    Username = keycloakUserDto.Username,
                    Password = keycloakUserDto.Password
                };


                var tokenRequestBody = KeycloakTokenUtils.GetTokenRequestBody(keycloakTokenRequestDto);

                var response = await httpClient
                    .PostAsync($"{keycloakSettings.BaseUrl}/token", tokenRequestBody)
                    .ConfigureAwait(false);


                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var keycloakTokenResponseDto = JsonConvert.DeserializeObject<KeycloakTokenResponseDto>(
                                    responseJson);

                return keycloakTokenResponseDto;
            }
        }

        public async Task<KeycloakTokenResponseDto?> GetTokenResponseAsync(
            KeycloakAuthCodeDto keycloakAuthCodeDto)
        {
            if (keycloakAuthCodeDto == null)
                throw new ArgumentNullException(nameof(keycloakAuthCodeDto));

            if (string.IsNullOrWhiteSpace(keycloakAuthCodeDto.AuthorizationCode))
                throw new ArgumentException("Authorization code is required", nameof(keycloakAuthCodeDto.AuthorizationCode));

            if (string.IsNullOrWhiteSpace(keycloakAuthCodeDto.RedirectUri))
                throw new ArgumentException("Redirect URI is required", nameof(keycloakAuthCodeDto.RedirectUri));

            using (var httpClient = httpClientFactory.CreateClient())
            {

                var keycloakTokenRequestDto = new KeycloakAuthCodeTokenRequestDto
                {
                    GrantType = KeycloakAccessTokenConsts.GrantTypeAuthCodeType,
                    ClientId = keycloakSettings.ClientId ??
                        throw new KeycloakException(nameof(keycloakSettings.ClientId)),
                    ClientSecret = keycloakSettings.ClientSecret ??
                        throw new KeycloakException(nameof(keycloakSettings.ClientSecret)),
                    AuthorizationCode = keycloakAuthCodeDto.AuthorizationCode,
                    RedirectUri = keycloakAuthCodeDto.RedirectUri
                };


                var tokenRequestBody = KeycloakTokenUtils.GetTokenRequestBody(keycloakTokenRequestDto);

                var response = await httpClient
                    .PostAsync($"{keycloakSettings.BaseUrl}/protocol/openid-connect/token", tokenRequestBody)
                    .ConfigureAwait(false);


                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var keycloakTokenResponseDto = JsonConvert.DeserializeObject<KeycloakTokenResponseDto>(
                                    responseJson);

                return keycloakTokenResponseDto;
            }
        }
    }
}
