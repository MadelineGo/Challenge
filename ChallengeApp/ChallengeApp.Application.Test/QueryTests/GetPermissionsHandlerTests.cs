using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Queries.GetPermissions;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Primitives;
using ChallengeApp.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChallengeApp.Application.Test.QueryTests;

public class GetPermissionsHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly GetPermissionsHandler _handler;

    public GetPermissionsHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockEventBus = new Mock<IEventBus>();
        _handler = new GetPermissionsHandler(_mockUnitOfWork.Object, _mockMapper.Object, _mockEventBus.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPermissions_WhenPermissionsExist()
    {
        // Arrange
        var query = new GetPermissionsQuery(1, 10);
        (IEnumerable<Permission> Permissions, int TotalCount) permissionsEntity = new()
        {
            Permissions =
            [
                new() {
                    Id = 1,
                    EmployeeName = "Roxana",
                    EmployeeSurname = "Gómez",
                    PermissionTypeId = 1,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedDate = DateTimeOffset.UtcNow
                }
            ],
            TotalCount = 1
        };
        var permissionsDto = new List<GetPermissionDto>
        {
            new(1,
                "Roxana",
                "Gómez",
                1,
                null,
                permissionsEntity.Permissions.First().CreatedDate,
                permissionsEntity.Permissions.First().LastModifiedDate)
        };

        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.GetAllAsync(query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(permissionsEntity);
        _mockMapper.Setup(m => m.Map<IEnumerable<GetPermissionDto>>(permissionsEntity.Permissions))
            .Returns(permissionsDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Permissions.Should().BeEquivalentTo(permissionsDto);
        result.TotalCount.Should().Be(permissionsEntity.TotalCount);
        _mockEventBus.Verify(bus => bus.Publish(It.IsAny<EventMessage>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenErrorOccurs()
    {
        // Arrange
        var query = new GetPermissionsQuery(1, 10);

        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.GetAllAsync(query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Database error");
        _mockEventBus.Verify(bus => bus.Publish(It.IsAny<EventMessage>()), Times.Never);
    }
}