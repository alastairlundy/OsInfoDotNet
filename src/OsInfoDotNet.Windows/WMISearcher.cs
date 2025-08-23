

using System.Runtime.Versioning;

using System;
using System.IO;
using AlastairLundy.OsInfoDotNet.Windows.Helpers;

namespace AlastairLundy.OsInfoDotNet.Windows;

/// <summary>
/// A class to make searching WMI easier.
/// </summary>
// ReSharper disable once InconsistentNaming
public class WMISearcher
{
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
    public static string GetWMIClass(string wmiClass)
    {
        if (OperatingSystem.IsWindows())
        {
            var result = Cli.Wrap($"{WinPowershellInfo.Location}{Path.DirectorySeparatorChar}powershell.exe")
                .WithArguments($"Get-WmiObject -Class {wmiClass} | Select-Object *")
                .ExecuteBufferedSync();
            
            return result.StandardOutput;
        }

        throw new PlatformNotSupportedException();
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
    public static string GetWMIValue(string property, string wmiClass)
    {
        if (OperatingSystem.IsWindows())
        {
            var result = Cli.Wrap($"{WinPowershellInfo.Location}{Path.DirectorySeparatorChar}powershell.exe")
                .WithArguments($"Get-CimInstance -Class {wmiClass} -Property {property}")
                .ExecuteBufferedSync();

            string[] arr = result.StandardOutput.Split(Convert.ToChar(Environment.NewLine));
            
           foreach (string str in arr)
           {
               if (str.ToLower().StartsWith(property.ToLower()))
               {
                   return str
                       .Replace(" : ", string.Empty)
                       .Replace(property, string.Empty)
                       .Replace(" ", string.Empty);
               }
           }
           
           throw new ArgumentException();
        }

        throw new PlatformNotSupportedException();
    }
}