using Moq;
using Xunit;
using FluentAssertions;
using HealthTech.Application.FhirResources.Commands.PatientAccess;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;

namespace HealthTech.Application.Tests.FhirResources.Commands.PatientAccess;

/// <summary>
/// Unit tests for GrantPatientAccessCommandHandler
/// </summary>
public class GrantPatientAccessCommandHandlerTests
{
    private readonly Mock<IPatientAccessService> _mockPatientAccessService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly GrantPatientAccessCommandHandler _handler;

    public GrantPatientAccessCommandHandlerTests()
    {
        _mockPatientAccessService = new Mock<IPatientAccessService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockUserService = new Mock<IUserService>();
        _handler = new GrantPatientAccessCommandHandler(_mockPatientAccessService.Object, _mockCurrentUserService.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task Handle_SystemAdministrator_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            PatientId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AccessLevel = "ReadOnly",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsEmergencyAccess = false,
            Purpose = "Clinical care"
        };

        var targetUser = new User
        {
            Id = command.UserId,
            Username = "targetuser",
            Email = "target@example.com",
            Role = "Nurse",
            Status = "Active"
        };

        _mockCurrentUserService.Setup(x => x.IsSystemAdministrator())
            .Returns(true);

        _mockUserService.Setup(x => x.GetByIdAsync(command.UserId))
            .ReturnsAsync(targetUser);

        _mockPatientAccessService.Setup(x => x.GrantAccessAsync(
                command.PatientId,
                command.UserId,
                command.AccessLevel,
                command.ExpiresAt,
                command.IsEmergencyAccess,
                command.EmergencyJustification,
                command.Purpose,
                It.IsAny<Guid>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AccessId.Should().NotBeNull();

        _mockCurrentUserService.Verify(x => x.IsSystemAdministrator(), Times.Once);
        _mockUserService.Verify(x => x.GetByIdAsync(command.UserId), Times.Once);
        _mockPatientAccessService.Verify(x => x.GrantAccessAsync(
            command.PatientId,
            command.UserId,
            command.AccessLevel,
            command.ExpiresAt,
            command.IsEmergencyAccess,
            command.EmergencyJustification,
            command.Purpose,
            It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HealthcareProviderWithAccess_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            PatientId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AccessLevel = "ReadWrite",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsEmergencyAccess = false,
            Purpose = "Treatment"
        };

        var targetUser = new User
        {
            Id = command.UserId,
            Username = "targetuser",
            Email = "target@example.com",
            Role = "Nurse",
            Status = "Active"
        };

        _mockCurrentUserService.Setup(x => x.IsSystemAdministrator())
            .Returns(false);

        _mockCurrentUserService.Setup(x => x.IsHealthcareProvider())
            .Returns(true);

        _mockCurrentUserService.Setup(x => x.CanAccessPatientAsync(command.PatientId.ToString()))
            .ReturnsAsync(true);

        _mockUserService.Setup(x => x.GetByIdAsync(command.UserId))
            .ReturnsAsync(targetUser);

        _mockPatientAccessService.Setup(x => x.GrantAccessAsync(
                command.PatientId,
                command.UserId,
                command.AccessLevel,
                command.ExpiresAt,
                command.IsEmergencyAccess,
                command.EmergencyJustification,
                command.Purpose,
                It.IsAny<Guid>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AccessId.Should().NotBeNull();

        _mockCurrentUserService.Verify(x => x.IsSystemAdministrator(), Times.Once);
        _mockCurrentUserService.Verify(x => x.IsHealthcareProvider(), Times.Once);
        _mockCurrentUserService.Verify(x => x.CanAccessPatientAsync(command.PatientId.ToString()), Times.Once);
        _mockUserService.Verify(x => x.GetByIdAsync(command.UserId), Times.Once);
        _mockPatientAccessService.Verify(x => x.GrantAccessAsync(
            command.PatientId,
            command.UserId,
            command.AccessLevel,
            command.ExpiresAt,
            command.IsEmergencyAccess,
            command.EmergencyJustification,
            command.Purpose,
            It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientPermissions_ReturnsFailureResponse()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            PatientId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AccessLevel = "ReadOnly",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsEmergencyAccess = false,
            Purpose = "Clinical care"
        };

        _mockCurrentUserService.Setup(x => x.IsSystemAdministrator())
            .Returns(false);

        _mockCurrentUserService.Setup(x => x.IsHealthcareProvider())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Insufficient permissions to grant patient access");
        result.AccessId.Should().BeNull();

        _mockCurrentUserService.Verify(x => x.IsSystemAdministrator(), Times.Once);
        _mockCurrentUserService.Verify(x => x.IsHealthcareProvider(), Times.Once);
        _mockUserService.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockPatientAccessService.Verify(x => x.GrantAccessAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<DateTime?>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoPatientAccess_ReturnsFailureResponse()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            PatientId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AccessLevel = "ReadOnly",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsEmergencyAccess = false,
            Purpose = "Clinical care"
        };

        _mockCurrentUserService.Setup(x => x.IsSystemAdministrator())
            .Returns(false);

        _mockCurrentUserService.Setup(x => x.IsHealthcareProvider())
            .Returns(true);

        _mockCurrentUserService.Setup(x => x.CanAccessPatientAsync(command.PatientId.ToString()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("You do not have access to this patient");
        result.AccessId.Should().BeNull();

        _mockCurrentUserService.Verify(x => x.IsSystemAdministrator(), Times.Once);
        _mockCurrentUserService.Verify(x => x.IsHealthcareProvider(), Times.Once);
        _mockCurrentUserService.Verify(x => x.CanAccessPatientAsync(command.PatientId.ToString()), Times.Once);
        _mockUserService.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockPatientAccessService.Verify(x => x.GrantAccessAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<DateTime?>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TargetUserNotFound_ReturnsFailureResponse()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            PatientId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AccessLevel = "ReadOnly",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsEmergencyAccess = false,
            Purpose = "Clinical care"
        };

        _mockCurrentUserService.Setup(x => x.IsSystemAdministrator())
            .Returns(true);

        _mockUserService.Setup(x => x.GetByIdAsync(command.UserId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Target user not found");
        result.AccessId.Should().BeNull();

        _mockCurrentUserService.Verify(x => x.IsSystemAdministrator(), Times.Once);
        _mockUserService.Verify(x => x.GetByIdAsync(command.UserId), Times.Once);
        _mockPatientAccessService.Verify(x => x.GrantAccessAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<DateTime?>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmergencyAccess_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new GrantPatientAccessCommand
        {
            PatientId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AccessLevel = "EmergencyAccess",
            ExpiresAt = DateTime.UtcNow.AddHours(4),
            IsEmergencyAccess = true,
            EmergencyJustification = "Patient in critical condition"
        };

        var targetUser = new User
        {
            Id = command.UserId,
            Username = "emergencyuser",
            Email = "emergency@example.com",
            Role = "HealthcareProvider",
            Status = "Active"
        };

        _mockCurrentUserService.Setup(x => x.IsSystemAdministrator())
            .Returns(true);

        _mockUserService.Setup(x => x.GetByIdAsync(command.UserId))
            .ReturnsAsync(targetUser);

        _mockPatientAccessService.Setup(x => x.GrantAccessAsync(
                command.PatientId,
                command.UserId,
                command.AccessLevel,
                command.ExpiresAt,
                command.IsEmergencyAccess,
                command.EmergencyJustification,
                command.Purpose,
                It.IsAny<Guid>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AccessId.Should().NotBeNull();

        _mockCurrentUserService.Verify(x => x.IsSystemAdministrator(), Times.Once);
        _mockUserService.Verify(x => x.GetByIdAsync(command.UserId), Times.Once);
        _mockPatientAccessService.Verify(x => x.GrantAccessAsync(
            command.PatientId,
            command.UserId,
            command.AccessLevel,
            command.ExpiresAt,
            command.IsEmergencyAccess,
            command.EmergencyJustification,
            command.Purpose,
            It.IsAny<Guid>()), Times.Once);
    }
}
