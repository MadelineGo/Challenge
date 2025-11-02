using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using ChallengeApp.Api.Contracts.Requests;
using ChallengeApp.Application.DTOs;
using ChallengeApp.Application.Elastic;
using ChallengeApp.Application.Queries.GetPermissions;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Primitives;
using ChallengeApp.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace ChallengeApp.IntegrationTests.ControllersTests;

public class PermissionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string RESOURCE_NAME = "/api/Permissions";
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _databaseName = $"PermissionsTests_{Guid.NewGuid()}";

    public PermissionsControllerTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));

                services.RemoveAll<IEventBus>();
                services.AddSingleton<IEventBus, InMemoryEventBus>();

                services.RemoveAll(typeof(IElasticsearchService<>));
                services.AddSingleton(typeof(IElasticsearchService<>), typeof(StubElasticsearchService<>));

                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                SeedDatabase(context);
            });
        });

        _client = _factory.CreateClient();
    }

    /// <summary>
    /// Confirma que el endpoint devuelva una lista de autorizaciones y el conteo total.
    /// </summary>
    [Fact]
    public async Task GetPermissions_ShouldReturnListOfPermissions()
    {
        await ResetDatabaseAsync();

        var response = await _client.GetAsync(RESOURCE_NAME);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetPermissionsResponse>();
        result.Should().NotBeNull();
        result!.Permissions.Should().NotBeNull();
        result.Permissions.Should().HaveCountGreaterThan(0);
        result.TotalCount.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// Garantiza que se obtenga una autorización específica por su identificador.
    /// </summary>
    [Fact]
    public async Task GetPermission_ShouldReturnPermissionById()
    {
        await ResetDatabaseAsync();

        var response = await _client.GetAsync($"{RESOURCE_NAME}/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var permission = await response.Content.ReadFromJsonAsync<GetPermissionDto>();
        permission.Should().NotBeNull();
        permission!.Id.Should().Be(1);
    }

    /// <summary>
    /// Verifica que el endpoint cree una nueva autorización y devuelva la ubicación del recurso creado.
    /// </summary>
    [Fact]
    public async Task RequestPermission_ShouldCreateNewPermission()
    {
        await ResetDatabaseAsync();

        var permissionRequest = new CreatePermissionRequest
        {
            EmployeeName = "Carlos",
            EmployeeSurname = "Ramírez",
            PermissionTypeId = 2
        };

        var response = await _client.PostAsJsonAsync(RESOURCE_NAME, permissionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var location = response.Headers.Location!.ToString();
        location.Should().Contain(RESOURCE_NAME);

        var createdResource = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        createdResource.Should().NotBeNull();
        createdResource!.Should().ContainKey("permissionId");
        var createdId = createdResource["permissionId"];

        var createdResponse = await _client.GetAsync(location);
        createdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdPermission = await createdResponse.Content.ReadFromJsonAsync<GetPermissionDto>();
        createdPermission.Should().NotBeNull();
        createdPermission!.EmployeeName.Should().Be(permissionRequest.EmployeeName);
        createdPermission.EmployeeSurname.Should().Be(permissionRequest.EmployeeSurname);
        createdPermission.PermissionTypeId.Should().Be(permissionRequest.PermissionTypeId);
        var stub = GetElasticsearchStub();
        stub.Documents.Should().ContainKey(createdId);
        var createdDocument = stub.Documents[createdId];
        createdDocument.EmployeeName.Should().Be(permissionRequest.EmployeeName);
        createdDocument.EmployeeSurname.Should().Be(permissionRequest.EmployeeSurname);
        createdDocument.PermissionTypeId.Should().Be(permissionRequest.PermissionTypeId);
    }

    /// <summary>
    /// Retorna BadRequest cuando el payload de creación es inválido.
    /// </summary>
    [Fact]
    public async Task RequestPermission_ShouldReturnBadRequest_WhenPayloadInvalid()
    {
        await ResetDatabaseAsync();

        var invalidRequest = new CreatePermissionRequest
        {
            EmployeeName = string.Empty,
            EmployeeSurname = "Ramírez",
            PermissionTypeId = null
        };

        var response = await _client.PostAsJsonAsync(RESOURCE_NAME, invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey(nameof(CreatePermissionRequest.EmployeeName));
        problem.Errors.Should().ContainKey(nameof(CreatePermissionRequest.PermissionTypeId));

        GetElasticsearchStub().Documents.Should().BeEmpty();
    }

    /// <summary>
    /// Comprueba que una autorización existente se actualice correctamente.
    /// </summary>
    [Fact]
    public async Task ModifyPermissions_ShouldUpdatePermission()
    {
        await ResetDatabaseAsync();

        var request = new UpdatePermissionRequest
        {
            EmployeeName = "UpdatedName",
            EmployeeSurname = "UpdatedSurname",
            PermissionTypeId = 2
        };

        var response = await _client.PutAsJsonAsync($"{RESOURCE_NAME}/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updatedResponse = await _client.GetAsync($"{RESOURCE_NAME}/1");
        updatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedPermission = await updatedResponse.Content.ReadFromJsonAsync<GetPermissionDto>();
        updatedPermission.Should().NotBeNull();
        updatedPermission!.EmployeeName.Should().Be(request.EmployeeName);
        updatedPermission.EmployeeSurname.Should().Be(request.EmployeeSurname);
        updatedPermission.PermissionTypeId.Should().Be(request.PermissionTypeId);
        var stub = GetElasticsearchStub();
        stub.Documents.Should().ContainKey(1);
        var elasticDocument = stub.Documents[1];
        elasticDocument.EmployeeName.Should().Be(request.EmployeeName);
        elasticDocument.EmployeeSurname.Should().Be(request.EmployeeSurname);
        elasticDocument.PermissionTypeId.Should().Be(request.PermissionTypeId);
    }

    /// <summary>
    /// Retorna BadRequest cuando el payload de actualización es inválido.
    /// </summary>
    [Fact]
    public async Task ModifyPermissions_ShouldReturnBadRequest_WhenPayloadInvalid()
    {
        await ResetDatabaseAsync();

        var invalidRequest = new UpdatePermissionRequest
        {
            EmployeeName = "",
            EmployeeSurname = "",
            PermissionTypeId = null
        };

        var response = await _client.PutAsJsonAsync($"{RESOURCE_NAME}/1", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey(nameof(UpdatePermissionRequest.EmployeeName));
        problem.Errors.Should().ContainKey(nameof(UpdatePermissionRequest.EmployeeSurname));
        problem.Errors.Should().ContainKey(nameof(UpdatePermissionRequest.PermissionTypeId));
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        SeedDatabase(context);
        GetElasticsearchStub().Clear();
    }

    private StubElasticsearchService<ElasticPermissionDto> GetElasticsearchStub() =>
        (StubElasticsearchService<ElasticPermissionDto>)_factory.Services
            .GetRequiredService<IElasticsearchService<ElasticPermissionDto>>();

    private static void SeedDatabase(AppDbContext context)
    {
        if (!context.Set<PermissionType>().Any())
        {
            context.Set<PermissionType>().AddRange(
                new PermissionType
                {
                    Id = 1,
                    Description = "Annual Leave",
                    Permissions = new List<Permission>()
                },
                new PermissionType
                {
                    Id = 2,
                    Description = "Sick Leave",
                    Permissions = new List<Permission>()
                });
            context.SaveChanges();
        }

        var existingPermissions = context.Set<Permission>().ToList();
        if (existingPermissions.Count > 0)
        {
            context.Set<Permission>().RemoveRange(existingPermissions);
            context.SaveChanges();
        }

        var now = DateTimeOffset.UtcNow;

        context.Set<Permission>().AddRange(
            new Permission
            {
                Id = 1,
                EmployeeName = "Roxana",
                EmployeeSurname = "Gómez",
                PermissionTypeId = 1,
                CreatedDate = now,
                LastModifiedDate = now
            },
            new Permission
            {
                Id = 2,
                EmployeeName = "Sofia",
                EmployeeSurname = "Lopez",
                PermissionTypeId = 2,
                CreatedDate = now,
                LastModifiedDate = now
            },
            new Permission
            {
                Id = 3,
                EmployeeName = "Ana",
                EmployeeSurname = "Velazquez",
                PermissionTypeId = 1,
                CreatedDate = now,
                LastModifiedDate = now
            });

        context.SaveChanges();
    }

    private sealed class InMemoryEventBus : IEventBus
    {
        public Task Publish<T>(T @event) where T : EventMessage => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    private sealed class StubElasticsearchService<T> : IElasticsearchService<T> where T : class
    {
        private readonly Dictionary<int, T> _documents = new();

        public IReadOnlyDictionary<int, T> Documents => _documents;

        public Task RequestOrModify(int id, T document)
        {
            _documents[id] = document;
            return Task.CompletedTask;
        }

        public void Clear() => _documents.Clear();
    }
}
