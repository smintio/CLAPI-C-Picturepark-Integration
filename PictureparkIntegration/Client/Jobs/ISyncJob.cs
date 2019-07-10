using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Jobs
{
    public interface ISyncJob
    {
        Task SynchronizeAssetsAsync();
    }
}
