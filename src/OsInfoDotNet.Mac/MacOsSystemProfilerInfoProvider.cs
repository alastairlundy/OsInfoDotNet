using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Primitives;
using AlastairLundy.OsInfoDotNet.Mac.Internals.Localizations;

namespace AlastairLundy.OsInfoDotNet.Mac;

/// <summary>
/// 
/// </summary>
public class MacOsSystemProfilerInfoProvider : IMacOsSystemProfilerInfoProvider
{
    private readonly IProcessInvoker _processInvoker;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    public MacOsSystemProfilerInfoProvider(IProcessInvoker processInvoker)
    {
        _processInvoker = processInvoker;
    }
    
    /// <summary>
    /// Gets a value from the Mac System Profiler information associated with a key
    /// </summary>
    /// <param name="macSystemProfilerDataType"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [SupportedOSPlatform("macos")]
    public async Task<string> GetMacSystemProfilerInformation(string macSystemProfilerDataType, string key)
    {
        if (OperatingSystem.IsMacOS() == false && OperatingSystem.IsMacCatalyst() == false)
            throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
        
        if (macSystemProfilerDataType.StartsWith("SP") == false)
        {
            macSystemProfilerDataType = "SP" + macSystemProfilerDataType;
        }
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "/usr/bin/system_profiler",
            Arguments = macSystemProfilerDataType,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
            
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(startInfo);
        
        string[] array = result.StandardOutput.Split(Environment.NewLine);

        foreach (string str in array)
        {
            if (str.ToLower().Contains(key.ToLower()))
            {
                return str.Replace(key, string.Empty).Replace(":", string.Empty);
            }
        }

        throw new ArgumentException();
    }    
}