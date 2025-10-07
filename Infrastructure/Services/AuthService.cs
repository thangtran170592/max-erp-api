using System.Linq.Expressions;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using Application.Utils;
using AutoMapper;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IGenericRepository<RefreshToken> _repositoryRefreshToken;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            ILogger<AuthService> logger,
            IMapper mapper,
            RoleManager<ApplicationRole> roleManager,
            IGenericRepository<RefreshToken> repositoryRefreshToken)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
            _logger = logger;
            _mapper = mapper;
            _roleManager = roleManager;
            _repositoryRefreshToken = repositoryRefreshToken;
        }

        public async Task<UserResponseDto> ProfileAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new Exception("Invalid user");
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(request.Username) ?? throw new Exception("User not found");
            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
            {
                _logger.LogError("Invalid password");
                throw new Exception("Invalid password");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = JwtToken.GenerateJwtToken(user, _config, roles);
            var refreshToken = JwtToken.GenerateRefreshToken();
            var result = _mapper.Map<UserResponseDto>(user);
            result.Roles = roles;
            return new AuthResponseDto
            {
                User = result,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<int> CreateRefreshTokenAsync(Guid userId, string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var refreshTokenObj = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                CreatedByIp = ipAddress,
                User = user ?? throw new Exception("User not found")
            };
            await _repositoryRefreshToken.AddOneAsync(refreshTokenObj, cancellationToken);
            return await _repositoryRefreshToken.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var token = await _repositoryRefreshToken.FindOneAsync(t => t.Token == refreshToken, cancellationToken: cancellationToken);
            if (token == null || token.IsExpired)
            {
                return false;
            }
            return true;
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            var includes = new Expression<Func<RefreshToken, object>>[] { rt => rt.User! };
            var refreshTokenResult = await _repositoryRefreshToken.FindOneAsync(
                rt => rt.Token == refreshToken,
                includes,
                cancellationToken);

            if (refreshTokenResult is null || refreshTokenResult.IsExpired || refreshTokenResult.Revoked.HasValue)
            {
                throw new Exception("Invalid refresh token");
            }

            var user = refreshTokenResult.User ?? throw new Exception("User not found"); ;

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = JwtToken.GenerateJwtToken(user, _config, roles ?? []);
            var newRefreshToken = JwtToken.GenerateRefreshToken();
            await RevokeRefreshTokenAsync(refreshTokenResult.Id, newRefreshToken.Token, ipAddress, cancellationToken);
            await CreateRefreshTokenAsync(user.Id, newRefreshToken.Token, ipAddress, cancellationToken);
            return new AuthResponseDto()
            {
                User = _mapper.Map<UserResponseDto>(user),
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<int> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = request.Username,
                    AccountStatus = request.AccountStatus,
                    DepartmentId = request.DepartmentId,
                    PositionId = request.PositionId,
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Register failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return 0;
                }

                if (request.Roles?.Any() == true)
                {
                    foreach (var role in request.Roles)
                    {
                        if (!await _roleManager.RoleExistsAsync(role))
                            await _roleManager.CreateAsync(new ApplicationRole(role));
                    }

                    var roleResult = await _userManager.AddToRolesAsync(user, request.Roles);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogWarning("Role assignment failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        return 0;
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user");
                return -1;
            }
        }


        public async Task<int> RevokeRefreshTokenAsync(Guid tokenId, string newToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            var token = await _repositoryRefreshToken.FindOneAsync(t => t.Id == tokenId, cancellationToken: cancellationToken) ?? throw new Exception("Refresh token not found");
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReplacedByToken = newToken;

            var revokeToken = _repositoryRefreshToken.UpdateOne(token);
            var result = await _repositoryRefreshToken.SaveChangesAsync(cancellationToken);
            return result;
        }

        public async Task<int> LogoutAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {

                var refreshTokenResult = await _repositoryRefreshToken.FindOneAsync(
                    rt => rt.Token == refreshToken,
                    cancellationToken: cancellationToken);

                if (refreshTokenResult is null || refreshTokenResult.IsExpired || refreshTokenResult.Revoked.HasValue)
                {
                    throw new Exception("Invalid refresh token");
                }
                refreshTokenResult.Revoked = DateTime.UtcNow;
                refreshTokenResult.RevokedByIp = ipAddress;
                refreshTokenResult.ReplacedByToken = string.Empty;
                var user = refreshTokenResult.User ?? await _userManager.FindByIdAsync(refreshTokenResult.UserId.ToString());
                if (user is not null)
                {
                    user.LastLoginAt = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                }
                var updateToken = _repositoryRefreshToken.UpdateOne(refreshTokenResult);
                var result = await _context.SaveChangesAsync(cancellationToken);
                if (result <= 0)
                {
                    _logger.LogError("Error while logging out");
                    throw new Exception("Error while logging out");
                }
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error while logging out");
                return -1;
            }
        }
    }
}