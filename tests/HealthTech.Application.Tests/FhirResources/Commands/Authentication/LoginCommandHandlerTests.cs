using Moq;
using Xunit;
using FluentAssertions;
using HealthTech.Application.FhirResources.Commands.Authentication;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;

namespace HealthTech.Application.Tests.FhirResources.Commands.Authentication;

/// <summary>
/// Unit tests for LoginCommandHandler
/// </summary>
public class LoginCommandHandlerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockJwtService = new Mock<IJwtService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _handler = new LoginCommandHandler(_mockUserService.Object, _mockJwtService.Object, _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "password123",
            TenantId = "tenant1",
            IpAddress = "127.0.0.1",
            UserAgent = "TestAgent"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            Role = "HealthcareProvider",
            Status = "Active",
            FirstName = "Test",
            LastName = "User"
        };

        var scopes = new List<UserScope>
        {
            new() { Scope = "patient/*.read" },
            new() { Scope = "user/*.read" }
        };

        _mockUserService.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId))
            .ReturnsAsync(user);

        _mockUserService.Setup(x => x.UpdateLastLoginAsync(user.Id, command.IpAddress))
            .Returns(Task.CompletedTask);

        _mockJwtService.Setup(x => x.GenerateTokenAsync(user.Id, user.Username, user.Role))
            .ReturnsAsync("jwt-token");

        _mockJwtService.Setup(x => x.GenerateRefreshTokenAsync())
            .ReturnsAsync("refresh-token");

        _mockUserService.Setup(x => x.CreateSessionAsync(user.Id, "jwt-token", "refresh-token", command.IpAddress, command.UserAgent))
            .Returns(Task.CompletedTask);

        _mockUserService.Setup(x => x.GetUserScopesAsync(user.Id))
            .ReturnsAsync(scopes);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().Be("jwt-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.User.Should().NotBeNull();
        result.User!.Username.Should().Be("testuser");
        result.User.Email.Should().Be("test@example.com");
        result.User.Role.Should().Be("HealthcareProvider");
        result.User.Scopes.Should().HaveCount(2);
        result.User.Scopes.Should().Contain("patient/*.read");
        result.User.Scopes.Should().Contain("user/*.read");

        _mockUserService.Verify(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId), Times.Once);
        _mockUserService.Verify(x => x.UpdateLastLoginAsync(user.Id, command.IpAddress), Times.Once);
        _mockJwtService.Verify(x => x.GenerateTokenAsync(user.Id, user.Username, user.Role), Times.Once);
        _mockJwtService.Verify(x => x.GenerateRefreshTokenAsync(), Times.Once);
        _mockUserService.Verify(x => x.CreateSessionAsync(user.Id, "jwt-token", "refresh-token", command.IpAddress, command.UserAgent), Times.Once);
        _mockUserService.Verify(x => x.GetUserScopesAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ReturnsFailureResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "wrongpassword",
            TenantId = "tenant1"
        };

        _mockUserService.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid username or password");
        result.Token.Should().BeNull();
        result.RefreshToken.Should().BeNull();
        result.User.Should().BeNull();

        _mockUserService.Verify(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId), Times.Once);
        _mockUserService.Verify(x => x.UpdateLastLoginAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(x => x.GenerateTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_LockedAccount_ReturnsFailureResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "password123",
            TenantId = "tenant1"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            Role = "HealthcareProvider",
            Status = "Locked"
        };

        _mockUserService.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Account is locked. Please contact administrator.");
        result.Token.Should().BeNull();
        result.RefreshToken.Should().BeNull();
        result.User.Should().BeNull();

        _mockUserService.Verify(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId), Times.Once);
        _mockUserService.Verify(x => x.UpdateLastLoginAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(x => x.GenerateTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InactiveAccount_ReturnsFailureResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "password123",
            TenantId = "tenant1"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            Role = "HealthcareProvider",
            Status = "Inactive"
        };

        _mockUserService.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Account is inactive. Please contact administrator.");
        result.Token.Should().BeNull();
        result.RefreshToken.Should().BeNull();
        result.User.Should().BeNull();

        _mockUserService.Verify(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId), Times.Once);
        _mockUserService.Verify(x => x.UpdateLastLoginAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(x => x.GenerateTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsFailureResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "testuser",
            Password = "password123",
            TenantId = "tenant1"
        };

        _mockUserService.Setup(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("An error occurred during login. Please try again.");
        result.Token.Should().BeNull();
        result.RefreshToken.Should().BeNull();
        result.User.Should().BeNull();

        _mockUserService.Verify(x => x.ValidateCredentialsAsync(command.Username, command.Password, command.TenantId), Times.Once);
    }
}
