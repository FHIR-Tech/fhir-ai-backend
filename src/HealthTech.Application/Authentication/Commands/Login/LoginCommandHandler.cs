using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;
using HealthTech.Application.Authentication.Commands.Login;
using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.Commands.Login;

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
                    IsSuccess = false,
                    Message = "Invalid username or password",
                    RequestId = request.RequestId
                };
            }

            // Check if account is locked
            if (user.Status == UserStatus.Locked)
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Account is locked. Please contact administrator.",
                    RequestId = request.RequestId
                };
            }

            // Check if account is inactive
            if (user.Status == UserStatus.Inactive)
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Account is inactive. Please contact administrator.",
                    RequestId = request.RequestId
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
                IsSuccess = true,
                Message = "Login successful",
                RequestId = request.RequestId,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserInfoDto
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
                IsSuccess = false,
                Message = $"An error occurred during login: {ex.Message}",
                RequestId = request.RequestId
            };
        }
    }
}
