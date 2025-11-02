namespace ChallengeApp.Application.Elastic;

public interface IElasticsearchService<T> where T : class
{
    Task RequestOrModify(int id, T document);
}