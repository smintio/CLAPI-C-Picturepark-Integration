using System.Threading.Tasks;

namespace Client.Jobs
{
    public interface ISyncJob
    {
        Task SynchronizeAsync(bool synchronizeGenericMetadata);
    }
}
