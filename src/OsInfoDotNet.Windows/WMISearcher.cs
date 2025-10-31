using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Specializations.Configurations;
using OsInfoDotNet.Windows.Abstractions;

namespace OsInfoDotNet.Windows;

/// <summary>
/// A class to make searching WMI easier.
/// </summary>
// ReSharper disable once InconsistentNaming
public class WMISearcher : IWMISearcher
{
    private readonly IProcessInvoker _processInvoker;

    public WMISearcher(IProcessInvoker processInvoker)
    {
        _processInvoker = processInvoker;
    }
    
        // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Gets information from a WMI class in WMI.
    /// </summary>
    /// <param name="wmiClass"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on an Operating System that isn't Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public async Task<string> GetWMIClass(string wmiClass)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        
        ClassicPowershellProcessConfiguration classicPowershellConfig = new(
            $"Get-WmiObject -Class {wmiClass} | Select-Object *",
            false, true, true);
  
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(classicPowershellConfig, 
            ProcessExitConfiguration.DefaultNoException, true, CancellationToken.None);
        
        return result.StandardOutput;
    }
    
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Gets a property/value in a WMI Class from WMI.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="wmiClass"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on an Operating System that isn't Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public async Task<string> GetWMIValue(string property, string wmiClass)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();

        ClassicPowershellProcessConfiguration classicPowershellConfig = new(
            $"Get-WmiObject -Class {wmiClass} -Property {property}",
            false, true, true);
  
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(classicPowershellConfig, 
            ProcessExitConfiguration.DefaultNoException, true, CancellationToken.None);
        
        string[] arr = result.StandardOutput.Split(Convert.ToChar(Environment.NewLine));

        string? str = arr.FirstOrDefault(x => x.ToLower().StartsWith(property.ToLower()));
        
        if(str is null)
            throw new ArgumentException();
        
        return str.Replace(" : ", string.Empty)
            .Replace(property, string.Empty)
            .Replace(" ", string.Empty);
    }
}