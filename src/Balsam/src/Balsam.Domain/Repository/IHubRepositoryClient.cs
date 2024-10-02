namespace Balsam.Repositories
{
    public interface IHubRepositoryClient
    {
        string Path { get; }
        void PersistChanges(string commitMessage);
        void PullChanges();
    }
}