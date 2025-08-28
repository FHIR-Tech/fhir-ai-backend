using Moq;
using FluentAssertions;
using HealthTech.Application.Authentication.Commands;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;
using Xunit;

namespace HealthTech.Application.Tests.Authentication.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _jwtServiceMock = new Mock<IJwtService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _handler = new LoginCommandHandler(_userServiceMock.Object, _jwtServiceMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "password123"
        };

        var user = new User
        {
            Id = "user-1",
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Role = UserRole.HealthcareProvider,
            Status = UserStatus.Active,
            TenantId = "tenant-1"
        };

        var scopes = new List<UserScope>
        {
            new() { Scope = "patient/*.read" },
            new() { Scope = "user/*.read" }
        };

        _userServiceMock.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password))
            .ReturnsAsync(user);
        _userServiceMock.Setup(x => x.GetUserScopesAsync(user.Id))
            .ReturnsAsync(scopes);
        _jwtServiceMock.Setup(x => x.GenerateAccessToken(user, scopes))
            .Returns("access-token");
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");
        _userServiceMock.Setup(x => x.CreateUserSessionAsync(user.Id, "refresh-token"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.User.Should().NotBeNull();
        result.User!.Username.Should().Be("testuser");
        result.User.Role.Should().Be(UserRole.HealthcareProvider);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        _userServiceMock.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid username or password");
    }

    [Fact]
    public async Task Handle_WithLockedAccount_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "password123"
        };

        var user = new User
        {
            Id = "user-1",
            Username = "testuser",
            Status = UserStatus.Locked
        };

        _userServiceMock.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Account is locked. Please contact administrator.");
    }

    [Fact]
    public async Task Handle_WithInactiveAccount_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "password123"
        };

        var user = new User
        {
            Id = "user-1",
            Username = "testuser",
            Status = UserStatus.Inactive
        };

        _userServiceMock.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Account is inactive. Please contact administrator.");
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "password123"
        };

        _userServiceMock.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("An error occurred during login. Please try again.");
    }
}
