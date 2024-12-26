using Application.Dtos;

namespace Application.Interfaces
{
    public interface IKeycloakTokenService
    {
        Task<KeycloakTokenResponseDto?> GetTokenResponseAsync(KeycloakUserDto keycloakUserDto);
    }
}