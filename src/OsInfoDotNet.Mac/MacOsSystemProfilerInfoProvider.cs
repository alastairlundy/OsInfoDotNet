using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Builders;
using AlastairLundy.CliInvoke.Core.Factories;
using AlastairLundy.OsInfoDotNet.Mac.Abstractions;
using AlastairLundy.OsInfoDotNet.Mac.Internals.Localizations;

// ReSharper disable ConvertToLocalFunction

namespace AlastairLundy.OsInfoDotNet.Mac;

/// <summary>
/// 
/// </summary>
public class MacOsSystemProfilerInfoProvider : IMacOsSystemProfilerInfoProvider
{
    private readonly IProcessInvoker _processInvoker;
    private readonly IProcessConfigurationFactory _processConfigurationFactory;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    /// <param name="processConfigurationFactory"></param>
    public MacOsSystemProfilerInfoProvider(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
    {
        _processInvoker = processInvoker;
        _processConfigurationFactory = processConfigurationFactory;
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

        Action<IProcessConfigurationBuilder> action = (builder =>
        {
            builder.ConfigureShellExecution(false)
                .ConfigureWindowCreation(false)
                .RedirectStandardInput(false)
                .RedirectStandardOutput(true)
                .RedirectStandardInput(true);
        });
        
       ProcessConfiguration configuration = _processConfigurationFactory.Create("/usr/bin/system_profiler",
           macSystemProfilerDataType, action);
            
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(configuration, ProcessExitConfiguration.Default,
            true, CancellationToken.None);
        
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