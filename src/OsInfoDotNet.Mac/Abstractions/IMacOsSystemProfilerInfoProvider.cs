using System.Threading.Tasks;

namespace AlastairLundy.OsInfoDotNet.Mac.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IMacOsSystemProfilerInfoProvider
{
    /// <summary>
    /// Gets a value from the Mac System Profiler information associated with a key
    /// </summary>
    /// <param name="macSystemProfilerDataType"></param>
    /// <param name="key"></param>
    /// <returns></returns>
#if NET5_0_OR_GREATER
#endif
    Task<string> GetMacSystemProfilerInformation(string macSystemProfilerDataType, string key);

}