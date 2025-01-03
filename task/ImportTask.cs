using Microsoft.Extensions.Logging;

namespace FakturowniaService.task
{
    public interface ImportTask
    {
        void ExecuteTask(ILogger<FakturService> logger);
    }
}
