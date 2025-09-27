using Application.Common.Security;
using Core.Enums;
using FluentValidation;

namespace Application.Dtos
{
    public record class RegisterRequestDto
    {
        public required string Username { get; init; }
        public required string Password { get; init; }
        public required AccountStatus AccountStatus { get; init; }
        public required string[] Roles { get; init; }
    }
    public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty().WithName("username").WithMessage("Username is required");

            RuleFor(x => x.Password)
            .NotEmpty().WithName("password").WithMessage("Password is required");

            RuleFor(x => x.AccountStatus)
            .NotEmpty().WithName("accountStatus").WithMessage("AccountStatus is required");
            
            RuleForEach(x => x.Roles)
            .Must(role => Role.AllRoles.Contains(role))
            .WithName("roles")
            .WithMessage(role => $"Role '{role}' is not valid. Allowed roles: {string.Join(", ", Role.AllRoles)}");
        }
    }

    public record class LoginRequestDto
    {
        public required string Username { get; init; }
        public required string Password { get; init; }
    }

    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty().WithName("username").WithMessage("Username is required")
            .Length(3, 20).WithName("usernameLength").WithMessage("Username must be 3-20 characters");

            RuleFor(x => x.Password)
            .NotEmpty().WithName("password").WithMessage("Password is required")
            .MinimumLength(6).WithName("passwordLength").WithMessage("Password must be greater than 6 characters");
        }
    }

    public record class RefreshTokenRequestDto
    {
        public required string RefreshToken { get; init; }
    }
    public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
            .NotEmpty().WithName("refreshToken").WithMessage("RefreshToken is required");
        }
    }


    public record class AuthResponseDto
    {
        public UserResponseDto? User { get; init; }
        public required TokenResponseDto AccessToken { get; init; }
        public required TokenResponseDto RefreshToken { get; init; }
    }

    public record class TokenResponseDto
    {
        public required string Token { get; init; }
        public required DateTime Expiration { get; init; }
    }

    public record class UserResponseDto
    {
        public required Guid Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string ProfilePicture { get; init; }
        public required bool IsActive { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }
        public IList<string> Roles { get; set; } = null!;
    }
}
