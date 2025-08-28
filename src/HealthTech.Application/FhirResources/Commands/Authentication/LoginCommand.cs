using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;

namespace HealthTech.Application.FhirResources.Commands.Authentication;

/// <summary>
/// Command for user login
/// </summary>
public record LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? TenantId { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}

/// <summary>
/// Response for login command
/// </summary>
public record LoginResponse
{
    public bool Success { get; init; }
    public string? Token { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string? ErrorMessage { get; init; }
    public UserInfo? User { get; init; }
}

/// <summary>
/// User information in login response
/// </summary>
public record UserInfo
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PractitionerId { get; init; }
    public List<string> Scopes { get; init; } = new();
}

/// <summary>
/// Validator for login command
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}

/// <summary>
/// Handler for login command
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly ICurrentUserService _currentUserService;

    public LoginCommandHandler(
        IUserService userService,
        IJwtService jwtService,
        ICurrentUserService currentUserService)
    {
        _userService = userService;
        _jwtService = jwtService;
        _currentUserService = currentUserService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate user credentials
            var user = await _userService.ValidateCredentialsAsync(
                request.Username, 
                request.Password, 
                request.TenantId);

            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password"
                };
            }

            // Check if account is locked
            if (user.Status == UserStatus.Locked)
            {
                return new LoginResponse
                {
                    Success = false,
                    ErrorMessage = "Account is locked. Please contact administrator."
                };
            }

            // Check if account is inactive
            if (user.Status == UserStatus.Inactive)
            {
                return new LoginResponse
                {
                    Success = false,
                    ErrorMessage = "Account is inactive. Please contact administrator."
                };
            }

            // Update last login
            await _userService.UpdateLastLoginAsync(user.Id, request.IpAddress);

            // Generate JWT token
            var token = await _jwtService.GenerateTokenAsync(user.Id, user.Username, user.Role);

            // Generate refresh token
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync();

            // Create user session
            await _userService.CreateSessionAsync(
                user.Id, 
                token, 
                refreshToken, 
                request.IpAddress, 
                request.UserAgent);

            // Get user scopes
            var scopes = await _userService.GetUserScopesAsync(user.Id);

            return new LoginResponse
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PractitionerId = user.PractitionerId,
                    Scopes = scopes.Select(s => s.Scope).ToList()
                }
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                ErrorMessage = "An error occurred during login. Please try again."
            };
        }
    }
}
