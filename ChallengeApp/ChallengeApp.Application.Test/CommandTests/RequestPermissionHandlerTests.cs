using AutoMapper;
using ChallengeApp.Application.Commands.RequestPermission;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Elastic;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Events;
using ChallengeApp.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChallengeApp.Application.Test.CommandTests;

public class RequestPermissionHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IElasticsearchService<ElasticPermissionDto>> _mockElasticsearch;
    private readonly RequestPermissionHandler _handler;

    public RequestPermissionHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockElasticsearch = new Mock<IElasticsearchService<ElasticPermissionDto>>();
        _mockMapper.Setup(m => m.Map<ElasticPermissionDto>(It.IsAny<Permission>()))
            .Returns((Permission permission) =>
                new ElasticPermissionDto(
                    permission.Id,
                    permission.EmployeeName,
                    permission.EmployeeSurname,
                    permission.PermissionTypeId,
                    permission.CreatedDate,
                    permission.LastModifiedDate));
        _handler = new RequestPermissionHandler(_mockUnitOfWork.Object, _mockMapper.Object, _mockElasticsearch.Object);
    }

    /// <summary>
    /// Verifica que el manejador propague la excepción cuando Elasticsearch falla al persistir la autorización.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowException_WhenElasticsearchFails()
    {
        // Arrange
        var command = new RequestPermissionCommand(new PermissionDto("Roxana", "Gómez", 1));
        var permission = new Permission
        {
            Id = 123,
            EmployeeName = "Roxana",
            EmployeeSurname = "Gómez",
            PermissionTypeId = 1,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _mockMapper.Setup(m => m.Map<Permission>(It.Is<PermissionDto>(dto => dto == command.Permission)))
            .Returns(permission);
        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.AddAsync(permission, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockElasticsearch.Setup(es => es.RequestOrModify(
            permission.Id,
            It.IsAny<ElasticPermissionDto>()
        ))
        .ThrowsAsync(new Exception("Elasticsearch failed"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Elasticsearch failed");
    }

    /// <summary>
    /// Comprueba que se agregue un evento de dominio al crear una nueva autorización.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldAddDomainEvent_WhenPermissionIsAdded()
    {
        // Arrange
        var command = new RequestPermissionCommand(new PermissionDto("Roxana", "Gómez", 1));
        var permission = new Permission
        {
            Id = 123,
            EmployeeName = "Roxana",
            EmployeeSurname = "Gómez",
            PermissionTypeId = 1,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _mockMapper.Setup(m => m.Map<Permission>(It.Is<PermissionDto>(dto => dto == command.Permission)))
            .Returns(permission);
        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.AddAsync(permission, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        permission.DomainEvents.Should().Contain(e => e is RequestPermissionEvent);
    }

    /// <summary>
    /// Asegura que el manejador invoque la persistencia y la sincronización en Elasticsearch tras crear la autorización.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallAddDomainEvent_WhenPermissionIsAdded()
    {
        // Arrange
        var command = new RequestPermissionCommand(new PermissionDto("Roxana", "Gómez", 1));
        var permission = new Permission
        {
            Id = 123,
            EmployeeName = "Roxana",
            EmployeeSurname = "Gómez",
            PermissionTypeId = 1,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _mockMapper.Setup(m => m.Map<Permission>(It.Is<PermissionDto>(dto => dto == command.Permission)))
            .Returns(permission);
        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.AddAsync(permission, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockElasticsearch.Setup(es => es.RequestOrModify(
            permission.Id,
            It.IsAny<ElasticPermissionDto>()
        ))
        .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUnitOfWork.Verify(uow => uow.PermissionsRepository.AddAsync(permission, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockElasticsearch.Verify(es => es.RequestOrModify(
            permission.Id,
            It.Is<ElasticPermissionDto>(ep =>
                ep.Id == permission.Id &&
                ep.EmployeeName == permission.EmployeeName &&
                ep.EmployeeSurname == permission.EmployeeSurname
            )
        ), Times.Once);
    }

    /// <summary>
    /// Garantiza que el identificador devuelto corresponda a la autorización guardada.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnCorrectPermissionId_WhenDataIsValid()
    {
        // Arrange
        var command = new RequestPermissionCommand(new PermissionDto("Roxana", "Gómez", 1));
        var permission = new Permission
        {
            Id = 123,
            EmployeeName = "Roxana",
            EmployeeSurname = "Gómez",
            PermissionTypeId = 1,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _mockMapper.Setup(m => m.Map<Permission>(It.Is<PermissionDto>(dto => dto == command.Permission)))
            .Returns(permission);
        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.AddAsync(permission, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockElasticsearch.Setup(es => es.RequestOrModify(
            permission.Id,
            It.IsAny<ElasticPermissionDto>()
        ))
        .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(permission.Id);
    }

    /// <summary>
    /// Verifica que el comando se mapée correctamente a la entidad de dominio.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldMapCommandToPermissionCorrectly()
    {
        // Arrange
        var command = new RequestPermissionCommand(new PermissionDto("Roxana", "Gómez", 1));
        var permission = new Permission
        {
            Id = 123,
            EmployeeName = "Roxana",
            EmployeeSurname = "Gómez",
            PermissionTypeId = 1,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _mockMapper.Setup(m => m.Map<Permission>(It.Is<PermissionDto>(dto => dto == command.Permission)))
            .Returns(permission);
        _mockUnitOfWork.Setup(uow => uow.PermissionsRepository.AddAsync(permission, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockElasticsearch.Setup(es => es.RequestOrModify(
            permission.Id,
            It.IsAny<ElasticPermissionDto>()
        ))
        .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockMapper.Verify(m => m.Map<Permission>(command.Permission), Times.Once);
    }
}
