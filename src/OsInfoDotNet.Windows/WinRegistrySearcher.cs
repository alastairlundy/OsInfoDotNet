


using System.Runtime.Versioning;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Primitives;
using AlastairLundy.OsInfoDotNet.Windows.Abstractions;

namespace AlastairLundy.OsInfoDotNet.Windows;

/// <summary>
/// A class to make searching the Windows Registry easier.
/// </summary>
public class WinRegistrySearcher : IWinRegistrySearcher
{
    private readonly IProcessInvoker _processInvoker;

    public WinRegistrySearcher(IProcessInvoker processInvoker)
    {
        _processInvoker = processInvoker;
    }
    
    /// <summary>
    ///  Gets the value of a registry key in the Windows registry.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on an Operating System that isn't Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public async Task<string> GetValueAsync(string query){
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            Arguments = $"REG QUERY {query}",
            FileName = Environment.SystemDirectory + Path.DirectorySeparatorChar + "cmd.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };
        
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(startInfo);
          
        if(string.IsNullOrEmpty(result.StandardOutput))
            throw new ArgumentException();
            
        return result.StandardOutput.Replace("REG_SZ", string.Empty);
    }
    
    /// <summary>
    ///  Gets the value of a registry key in the Windows registry.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on an Operating System that isn't Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public async Task<string> GetValueAsync(string query, string value){
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            Arguments = $"REG QUERY {query} /v {value}",
            FileName = Environment.SystemDirectory + Path.DirectorySeparatorChar + "cmd.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };
        
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(startInfo);
          
        if(string.IsNullOrEmpty(result.StandardOutput))
            throw new ArgumentException();
        
        return result.StandardOutput.Replace(value, string.Empty)
            .Replace("REG_SZ", string.Empty);
    }
}