using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;
using System.Text.Json;

namespace HealthTech.Application.Authentication.Commands;

public record LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? TenantId { get; init; }
}

public record LoginResponse
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public UserInfo? User { get; init; }
    public string? ErrorMessage { get; init; }
}

public record UserInfo
{
    public string Id { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public string? PractitionerId { get; init; }
    public string TenantId { get; init; } = string.Empty;
    public List<string> Scopes { get; init; } = new();
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");
    }
}

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
            var user = await _userService.ValidateCredentialsAsync(request.Username, request.Password, request.TenantId ?? "default");
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

            // Get user scopes
            var scopes = await _userService.GetUserScopesAsync(user.Id);

            // Generate JWT tokens
            var accessToken = _jwtService.GenerateToken(
                user.Id.ToString(),
                user.Username,
                user.Email,
                user.Role.ToString(),
                user.TenantId,
                scopes,
                user.PractitionerId
            );
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id.ToString());

            // Create user session
            await _userService.CreateUserSessionAsync(user.Id, refreshToken);

            // Get user role and practitioner info
            var userRole = user.Role;
            var practitionerId = userRole == UserRole.HealthcareProvider ? user.PractitionerId : null;

            return new LoginResponse
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserInfo
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.DisplayName,
                    Role = userRole,
                    PractitionerId = practitionerId,
                    TenantId = user.TenantId,
                    Scopes = scopes.ToList()
                }
            };
        }
        catch (Exception ex)
        {
            // Log the actual exception for debugging
            Console.WriteLine($"Login Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            Console.WriteLine($"Exception Type: {ex.GetType().Name}");
            
            return new LoginResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred during login: {ex.Message}"
            };
        }
    }
}
