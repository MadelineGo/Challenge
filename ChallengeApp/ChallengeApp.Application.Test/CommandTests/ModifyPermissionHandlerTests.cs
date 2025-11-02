using AutoMapper;
using ChallengeApp.Application.Commands.ModifyPermission;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Elastic;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Exceptions;
using ChallengeApp.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChallengeApp.Application.Test.CommandTests;

public class ModifyPermissionHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IElasticsearchService<ElasticPermissionDto>> _mockElasticsearch;
    private readonly ModifyPermissionHandler _handler;

    public ModifyPermissionHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockElasticsearch = new Mock<IElasticsearchService<ElasticPermissionDto>>();
        _handler = new ModifyPermissionHandler(_mockUnitOfWork.Object, _mockMapper.Object, _mockElasticsearch.Object);
    }

    
    /// <summary>
    /// Verifica que una autorización existente se actualice y se sincronice con Elasticsearch.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdatePermissionAndSendToElasticsearch()
    {
        // Arrange
        var command = new ModifyPermissionCommand(123, "John", "Doe", 1);
        var existingPermission = new Permission
        {
            Id = 123,
            EmployeeName = "Jane",
            EmployeeSurname = "Smith",
            PermissionTypeId = 2,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPermission);
        
        _mockMapper.Setup(m => m.Map(command, existingPermission))
            .Verifiable();
        
        _mockMapper.Setup(m => m.Map<ElasticPermissionDto>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var permission = (Permission)source;
                return new ElasticPermissionDto(permission.Id,
                                                permission.EmployeeName,
                                                permission.EmployeeSurname,
                                                permission.PermissionTypeId,
                                                permission.CreatedDate,
                                                permission.LastModifiedDate);
            })
            .Verifiable();
        
        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.Update(existingPermission))
            .Verifiable();
        
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        _mockElasticsearch.Setup(es => es.RequestOrModify(
            command.Id,
            It.Is<ElasticPermissionDto>(ep => ep.Id == command.Id)
        ))
        .Returns(Task.CompletedTask)
        .Verifiable();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockMapper.Verify();
        _mockUnitOfWork.Verify(uow => uow.PermissionsRepository.Update(existingPermission), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockElasticsearch.Verify(es => es.RequestOrModify(
            command.Id,
            It.Is<ElasticPermissionDto>(ep => ep.Id == command.Id)
        ), Times.Once);
    }

    
    /// <summary>
    /// Garantiza que el manejador lance NotFoundException al intentar modificar una autorización inexistente.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenPermissionNotFound()
    {
        // Arrange
        var command = new ModifyPermissionCommand(123, "Roxana", "Gómez", 1);
        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Permission)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"The permission '{command.Id}' was not found");
    }
}
