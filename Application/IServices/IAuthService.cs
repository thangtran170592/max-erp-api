using Application.Dtos;

namespace Application.IServices
{
    public interface IAuthService
    {
        Task<UserResponseDto> ProfileAsync(Guid id);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
        Task<int> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
        Task<int> CreateRefreshTokenAsync(Guid userId, string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<int> RevokeRefreshTokenAsync(Guid id, string newToken, string ipAddress, CancellationToken cancellationToken = default);
        Task<int> LogoutAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
    }
}