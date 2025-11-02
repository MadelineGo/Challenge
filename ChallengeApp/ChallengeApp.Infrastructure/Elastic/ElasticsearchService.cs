using ChallengeApp.Application.Elastic;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChallengeApp.Infrastructure.Elastic;

public class ElasticsearchService<T> : IElasticsearchService<T> where T : class
{
    private readonly ElasticsearchClient _client;
    private readonly string _indexName;
    private readonly ILogger<ElasticsearchService<T>> _logger;

    public ElasticsearchService(IConfiguration configuration, ILogger<ElasticsearchService<T>> logger)
    {
        var elasticsearchUri = configuration["Elasticsearch:Uri"]
                               ?? throw new ArgumentException("Elasticsearch URI is not configured.");
        var elasticUser = configuration["Elasticsearch:Username"]
                          ?? throw new ArgumentException("Elasticsearch username is not configured.");
        var elasticPassword = configuration["Elasticsearch:Password"]
                              ?? throw new ArgumentException("Elasticsearch password is not configured.");

        var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUri))
            .Authentication(new BasicAuthentication(elasticUser, elasticPassword))
            .DisableDirectStreaming()
            .IncludeServerStackTraceOnError()
            .ServerCertificateValidationCallback((sender, certificate, chain, errors) => true);

        _client = new ElasticsearchClient(settings);
        _indexName = typeof(T).Name.ToLower() + "s";
        _logger = logger;
    }
    
    public async Task RequestOrModify(int id, T document)
    {
        try
        {
            var indexResponse = await _client.IndexAsync(document, idx => idx
                .Index(typeof(T).Name.ToLower())
                .Id(id));

            if (!indexResponse.IsSuccess())
                throw new Exception($"Failed to index document: {indexResponse.DebugInformation}");
            
            _logger.LogInformation($"The document '{indexResponse.Id}' was {indexResponse.Result.ToString()}.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Elasticsearch failed: {ex.Message}");
        }
    }
}