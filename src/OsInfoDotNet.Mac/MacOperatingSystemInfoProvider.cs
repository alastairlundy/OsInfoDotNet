using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Core;

using OsInfoDotNet.Abstractions;
using OsInfoDotNet.Mac.Internals.Localizations;

namespace OsInfoDotNet.Mac;

/// <summary>
/// 
/// </summary>
public class MacOperatingSystemInfoProvider : IOperatingSystemInfoProvider
{
    private readonly IProcessInvoker _processInvoker;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    public MacOperatingSystemInfoProvider(IProcessInvoker processInvoker)
    {
        _processInvoker = processInvoker;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    public async Task<OperatingSystemInfo> GetOperatingSystemInfoAsync(CancellationToken cancellationToken)
    {
        OperatingSystemInfo operatingSystemInfo = new OperatingSystemInfo(
            GetOsName(),
            await GetMacOsVersionAsync(cancellationToken),
            GetXnuVersion(),
            OperatingSystemFamily.Darwin,
            await GetMacOsBuildNumberAsync(cancellationToken)
        );

        return operatingSystemInfo;
    }

    private string GetOsName()
    {
        return RuntimeInformation.OSDescription.Split(' ').First();
    }
    
    /// <summary>
    /// Detects macOS's XNU Version.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
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
    [SupportedOSPlatform("maccatalyst")]
    private async Task<Version> GetMacOsVersionAsync(CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
        }

        string[] result = await GetMacSwVersInfo(cancellationToken);

        return Version.Parse(result[1].Replace("ProductVersion:", string.Empty)
            .Replace(" ", string.Empty));
    }
    
    /// <summary>
    /// Detects the Build Number of the installed version of macOS.
    /// </summary>
    /// <returns>the build number of the installed version of macOS.</returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    private async Task<string> GetMacOsBuildNumberAsync(CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
        }

        string[] result = await GetMacSwVersInfo(cancellationToken);

        return result[2].ToLower().Replace("BuildVersion:",
            string.Empty).Replace(" ", string.Empty);
    }

    // ReSharper disable once IdentifierTypo
    /// <summary>
    /// Gets info from sw_vers command on Mac.
    /// </summary>
    /// <returns></returns>
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    private async Task<string[]> GetMacSwVersInfo(CancellationToken cancellationToken)
    {
        ProcessConfiguration processConfiguration = new("/usr/bin/sw_vers",
            false, true, true);

        BufferedProcessResult processResult = await _processInvoker.ExecuteBufferedAsync(processConfiguration,
            ProcessExitConfiguration.Default, true, cancellationToken);
        
        // ReSharper disable once StringLiteralTypo
        return processResult.StandardOutput.Split(Convert.ToChar(Environment.NewLine));
    }
}