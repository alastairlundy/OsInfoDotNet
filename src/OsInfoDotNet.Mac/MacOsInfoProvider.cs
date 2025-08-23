using System.Runtime.Versioning;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Primitives;

using AlastairLundy.OsInfoDotNet.Mac.Internals.Localizations;

namespace AlastairLundy.OsInfoDotNet.Mac;

// ReSharper disable once InconsistentNaming
/// <summary>
/// A class to Detect macOS versions, macOS features, and find out more about a user's macOS installation.
/// </summary>
public class MacOsInfoProvider : IMacOsInfoProvider
{
    private readonly IProcessInvoker _processInvoker;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    public MacOsInfoProvider(IProcessInvoker processInvoker)
    {
        _processInvoker = processInvoker;
    }

    /// <summary>
    /// Returns whether a Mac is Apple Silicon based.
    /// </summary>
    /// <returns>true if the currently running Mac uses Apple Silicon; false if running on an Intel Mac.</returns>
    [SupportedOSPlatform("macos")]
    public bool IsAppleSiliconMac()
    {
        return OperatingSystem.IsMacOS() && RuntimeInformation.OSArchitecture == Architecture.Arm64;
    }

    /// <summary>
    /// Detects macOS System Information.
    /// </summary>
    /// <returns></returns>
    [SupportedOSPlatform("macos")]
    public async Task<MacOsSystemInfo> GetMacSystemInfoAsync()
    {
        if (OperatingSystem.IsMacOS())
        {
            return new MacOsSystemInfo()
            {
                MacOsBuildNumber = await GetMacOsBuildNumberAsync(),
                MacOsVersion = await GetMacOsVersionAsync(),
                DarwinVersion = GetDarwinVersion(),
                XnuVersion = GetXnuVersion()
            };
        }
        else
        {
            throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
        }
    }

    /// <summary>
    /// Detects the Darwin Version on macOS
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
    [SupportedOSPlatform("macos")]
    public Version GetDarwinVersion()
    {
        if (OperatingSystem.IsMacOS())
        {
            return Version.Parse(RuntimeInformation.OSDescription.Split(' ')[1]);
        }

        throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
    }

    /// <summary>
    /// Detects macOS's XNU Version.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
    [SupportedOSPlatform("macos")]
    public Version GetXnuVersion()
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

                if (IsAppleSiliconMac())
                {
                    array[index] = array[index].Replace("/RELEASE_ARM64_T", string.Empty).Remove(array.Length - 4);
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
    public async Task<Version> GetMacOsVersionAsync()
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
    public async Task<string> GetMacOsBuildNumberAsync()
    {
        if (OperatingSystem.IsMacOS())
        {
            string[] result = await GetMacSwVersInfo();

            return result[2].ToLower().Replace("BuildVersion:",
                string.Empty).Replace(" ", string.Empty);
        }
        else
        {
            throw new PlatformNotSupportedException(Resources.Exceptions_PlatformNotSupported_MacOnly);
        }
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
            RedirectStandardOutput = true,
            FileName = "/usr/bin/sw_vers",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(startInfo);

        // ReSharper disable once StringLiteralTypo
        return result.StandardOutput.Split(Convert.ToChar(Environment.NewLine));
    }
}