using System.Threading.Tasks;

namespace OsInfoDotNet.Abstractions;

public interface IOperatingSystemInfoProvider
{
    Task<OperatingSystemInfo> GetOperatingSystemInfoAsync();
}