using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using AlastairLundy.OsInfoDotNet.Mac.Internals.Localizations;

using OsInfoDotNet.Abstractions;

namespace AlastairLundy.OsInfoDotNet.Mac;

/// <summary>
/// 
/// </summary>
public class MacOperatingSystemInfoProvider : IOperatingSystemInfoProvider
{
    
    public async Task<OperatingSystemInfo> GetOperatingSystemInfoAsync()
    {
        OperatingSystemInfo operatingSystemInfo = new OperatingSystemInfo(
            GetOsName(),
            await GetMacOsVersionAsync(),
            GetXnuVersion(),
            OperatingSystemFamily.Darwin,
            await GetMacOsBuildNumberAsync()
        );

        return operatingSystemInfo;
    }

    private string GetOsName()
    {
        
    }
    
    /// <summary>
    /// Detects macOS's XNU Version.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
    [SupportedOSPlatform("macos")]
    private Version GetXnuVersion()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
        }

        string[] array = RuntimeInformation.OSDescription.Split(' ');

        for (int index = 0; index < array.Length; index++)
        {
            if (array[index].ToLower().StartsWith("root:xnu-"))
            {
                array[index] = array[index].Replace("root:xnu-", string.Empty)
                    .Replace("~", ".");

                if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
                {
                    array[index] = array[index].Replace("/RELEASE_ARM64_T", string.Empty).
                        Remove(array.Length - array[index].LastIndexOf("T", StringComparison.Ordinal));
                }
                else
                {
                    array[index] = array[index].Replace("/RELEASE_X86_64", string.Empty);
                }

                return Version.Parse(array[index]);
            }
        }

        throw new ArgumentException();
    }

    
    /// <summary>
    /// Detects the macOS version and returns it as a System.Version object.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
    [SupportedOSPlatform("macos")]
    private async Task<Version> GetMacOsVersionAsync()
    {
        if (OperatingSystem.IsMacOS())
        {
            string[] result = await GetMacSwVersInfo();

            return Version.Parse(result[1].Replace("ProductVersion:", string.Empty)
                .Replace(" ", string.Empty));
        }
        else
        {
            throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
        }
    }
    
    /// <summary>
    /// Detects the Build Number of the installed version of macOS.
    /// </summary>
    /// <returns>the build number of the installed version of macOS.</returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
    [SupportedOSPlatform("macos")]
    private async Task<string> GetMacOsBuildNumberAsync()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
        }

        string[] result = await GetMacSwVersInfo();

        return result[2].ToLower().Replace("BuildVersion:",
            string.Empty).Replace(" ", string.Empty);
    }

    // ReSharper disable once IdentifierTypo
    /// <summary>
    /// Gets info from sw_vers command on Mac.
    /// </summary>
    /// <returns></returns>
    [SupportedOSPlatform("macos")]
    private async Task<string[]> GetMacSwVersInfo()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "/usr/bin/sw_vers",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        Process process = new Process()
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };
        
        Task waitForExit = process.WaitForExitAsync();
        Task<string> standardOutput = process.StandardOutput.ReadToEndAsync();

        try
        {
            process.Start();

            await Task.WhenAll(waitForExit, standardOutput);
        }
        finally
        {
            process.Dispose();
            waitForExit.Dispose();
            standardOutput.Dispose();
        }

        string standardOutputStr = await standardOutput;
        
        // ReSharper disable once StringLiteralTypo
        return standardOutputStr.Split(Convert.ToChar(Environment.NewLine));
    }
}