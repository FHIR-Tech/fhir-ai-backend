using Moq;
using FluentAssertions;
using HealthTech.Application.PatientAccess.Commands;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;
using Xunit;

namespace HealthTech.Application.Tests.PatientAccess.Commands;

public class GrantPatientAccessCommandHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IPatientAccessService> _patientAccessServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly GrantPatientAccessCommandHandler _handler;

    public GrantPatientAccessCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _patientAccessServiceMock = new Mock<IPatientAccessService>();
        _userServiceMock = new Mock<IUserService>();
        _handler = new GrantPatientAccessCommandHandler(_currentUserServiceMock.Object, _patientAccessServiceMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_SystemAdministratorGrantingAccess_ShouldReturnSuccess()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            TargetUserId = "target-user-1",
            PatientId = "patient-1",
            AccessLevel = PatientAccessLevel.Read,
            Reason = "Treatment coordination"
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns("admin-1");
        _currentUserServiceMock.Setup(x => x.UserRole).Returns(UserRole.SystemAdministrator);
        _currentUserServiceMock.Setup(x => x.TenantId).Returns("tenant-1");

        _patientAccessServiceMock.Setup(x => x.CanGrantAccessAsync("admin-1", "patient-1", UserRole.SystemAdministrator))
            .ReturnsAsync(true);

        var targetUser = new User
        {
            Id = "target-user-1",
            Username = "targetuser",
            TenantId = "tenant-1"
        };

        _userServiceMock.Setup(x => x.GetUserByIdAsync("target-user-1"))
            .ReturnsAsync(targetUser);

        _patientAccessServiceMock.Setup(x => x.GrantAccessAsync(
            "target-user-1", "patient-1", PatientAccessLevel.Read, "admin-1", "Treatment coordination", null))
            .ReturnsAsync("access-1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AccessId.Should().Be("access-1");
    }

    [Fact]
    public async Task Handle_HealthcareProviderWithPatientAccess_ShouldReturnSuccess()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            TargetUserId = "target-user-1",
            PatientId = "patient-1",
            AccessLevel = PatientAccessLevel.Read,
            Reason = "Consultation"
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns("provider-1");
        _currentUserServiceMock.Setup(x => x.UserRole).Returns(UserRole.HealthcareProvider);
        _currentUserServiceMock.Setup(x => x.TenantId).Returns("tenant-1");

        _patientAccessServiceMock.Setup(x => x.CanGrantAccessAsync("provider-1", "patient-1", UserRole.HealthcareProvider))
            .ReturnsAsync(true);

        var targetUser = new User
        {
            Id = "target-user-1",
            Username = "targetuser",
            TenantId = "tenant-1"
        };

        _userServiceMock.Setup(x => x.GetUserByIdAsync("target-user-1"))
            .ReturnsAsync(targetUser);

        _patientAccessServiceMock.Setup(x => x.GrantAccessAsync(
            "target-user-1", "patient-1", PatientAccessLevel.Read, "provider-1", "Consultation", null))
            .ReturnsAsync("access-1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AccessId.Should().Be("access-1");
    }

    [Fact]
    public async Task Handle_InsufficientPermissions_ShouldReturnFailure()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            TargetUserId = "target-user-1",
            PatientId = "patient-1",
            AccessLevel = PatientAccessLevel.Read
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserServiceMock.Setup(x => x.UserRole).Returns(UserRole.Patient);

        _patientAccessServiceMock.Setup(x => x.CanGrantAccessAsync("user-1", "patient-1", UserRole.Patient))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Insufficient permissions to grant patient access");
    }

    [Fact]
    public async Task Handle_NoPatientAccess_ShouldReturnFailure()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            TargetUserId = "target-user-1",
            PatientId = "patient-1",
            AccessLevel = PatientAccessLevel.Read
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns("provider-1");
        _currentUserServiceMock.Setup(x => x.UserRole).Returns(UserRole.HealthcareProvider);

        _patientAccessServiceMock.Setup(x => x.CanGrantAccessAsync("provider-1", "patient-1", UserRole.HealthcareProvider))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Insufficient permissions to grant patient access");
    }

    [Fact]
    public async Task Handle_TargetUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            TargetUserId = "target-user-1",
            PatientId = "patient-1",
            AccessLevel = PatientAccessLevel.Read
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns("admin-1");
        _currentUserServiceMock.Setup(x => x.UserRole).Returns(UserRole.SystemAdministrator);
        _currentUserServiceMock.Setup(x => x.TenantId).Returns("tenant-1");

        _patientAccessServiceMock.Setup(x => x.CanGrantAccessAsync("admin-1", "patient-1", UserRole.SystemAdministrator))
            .ReturnsAsync(true);

        _userServiceMock.Setup(x => x.GetUserByIdAsync("target-user-1"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Target user not found");
    }

    [Fact]
    public async Task Handle_DifferentTenantUser_ShouldReturnFailure()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            TargetUserId = "target-user-1",
            PatientId = "patient-1",
            AccessLevel = PatientAccessLevel.Read
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns("admin-1");
        _currentUserServiceMock.Setup(x => x.UserRole).Returns(UserRole.SystemAdministrator);
        _currentUserServiceMock.Setup(x => x.TenantId).Returns("tenant-1");

        _patientAccessServiceMock.Setup(x => x.CanGrantAccessAsync("admin-1", "patient-1", UserRole.SystemAdministrator))
            .ReturnsAsync(true);

        var targetUser = new User
        {
            Id = "target-user-1",
            Username = "targetuser",
            TenantId = "tenant-2" // Different tenant
        };

        _userServiceMock.Setup(x => x.GetUserByIdAsync("target-user-1"))
            .ReturnsAsync(targetUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot grant access to user from different tenant");
    }

    [Fact]
    public async Task Handle_EmergencyAccess_ShouldReturnSuccess()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            TargetUserId = "target-user-1",
            PatientId = "patient-1",
            AccessLevel = PatientAccessLevel.Emergency,
            Reason = "Emergency situation",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns("admin-1");
        _currentUserServiceMock.Setup(x => x.UserRole).Returns(UserRole.SystemAdministrator);
        _currentUserServiceMock.Setup(x => x.TenantId).Returns("tenant-1");

        _patientAccessServiceMock.Setup(x => x.CanGrantAccessAsync("admin-1", "patient-1", UserRole.SystemAdministrator))
            .ReturnsAsync(true);

        var targetUser = new User
        {
            Id = "target-user-1",
            Username = "targetuser",
            TenantId = "tenant-1"
        };

        _userServiceMock.Setup(x => x.GetUserByIdAsync("target-user-1"))
            .ReturnsAsync(targetUser);

        _patientAccessServiceMock.Setup(x => x.GrantAccessAsync(
            "target-user-1", "patient-1", PatientAccessLevel.Emergency, "admin-1", "Emergency situation", command.ExpiresAt))
            .ReturnsAsync("access-1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AccessId.Should().Be("access-1");
    }
}
