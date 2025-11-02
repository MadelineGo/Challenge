using AutoMapper;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Queries.GetPermissionsType;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChallengeApp.Application.Test.QueryTests;

public class GetPermissionsTypeHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetPermissionsTypeHandler _handler;

    public GetPermissionsTypeHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetPermissionsTypeHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPermissionTypes_WhenPermissionTypesExist()
    {
        // Arrange
        var query = new GetPermissionsTypeQuery();
        List<PermissionType> permissionTypesEntity =
        [
            new() {
                Id = 1,
                Description = "Type1"
            },
            new() {
                Id = 2,
                Description = "Type2"
            }
        ];

        List<GetPermissionTypeDto> permissionTypesDto =
        [
            new GetPermissionTypeDto(1, "Type1"),
            new GetPermissionTypeDto(2, "Type2")
        ];

        _mockUnitOfWork.Setup(uow => uow.PermissionsTypeRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(permissionTypesEntity);
        _mockMapper.Setup(m => m.Map<IEnumerable<GetPermissionTypeDto>>(permissionTypesEntity))
            .Returns(permissionTypesDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(permissionTypesDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoPermissionTypesExist()
    {
        // Arrange
        var query = new GetPermissionsTypeQuery();
        List<PermissionType> permissionTypesEntity = [];
        List<GetPermissionTypeDto> permissionTypesDto = [];

        _mockUnitOfWork.Setup(uow => uow.PermissionsTypeRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(permissionTypesEntity);
        _mockMapper.Setup(m => m.Map<IEnumerable<GetPermissionTypeDto>>(permissionTypesEntity))
            .Returns(permissionTypesDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}