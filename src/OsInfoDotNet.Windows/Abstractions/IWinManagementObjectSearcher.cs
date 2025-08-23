using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace AlastairLundy.OsInfoDotNet.Windows.Abstractions;

public interface IWinManagementObjectSearcher
{
    /// <summary>
    /// Returns a Dictionary of Query objects and their associated WMI values.
    /// WARNING: DO NOT RUN on NON-Windows platforms. This will result in errors.
    /// </summary>
    /// <param name="queryObjectsList"></param>
    /// <param name="wmiClass"></param>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    Task<Dictionary<string, string>> Get(List<string> queryObjectsList, string wmiClass);
}