using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;

using AlastairLundy.OsReleaseNet;
using AlastairLundy.OsReleaseNet.Abstractions;

using OsInfoDotNet.Abstractions;

namespace OsInfoDotnet.Linux;

public class LinuxOperatingSystemInfoProvider : IOperatingSystemInfoProvider
{
    private readonly ILinuxOsReleaseProvider _linuxOsReleaseProvider;
    private readonly IProcessInvoker _processInvoker;

    public LinuxOperatingSystemInfoProvider(ILinuxOsReleaseProvider linuxOsReleaseProvider, IProcessInvoker processInvoker)
    {
        _linuxOsReleaseProvider = linuxOsReleaseProvider;
        _processInvoker = processInvoker;
    }
    
    
    public async Task<OperatingSystemInfo> GetOperatingSystemInfoAsync()
    {
        LinuxOsReleaseInfo linuxOsReleaseInfo = await _linuxOsReleaseProvider.GetReleaseInfoAsync();
        
        string version = linuxOsReleaseInfo.Version.Replace("LTS", string.Empty);

       version = Regex.Replace(version, "[A-Za-z]", "");

       OperatingSystemInfo operatingSystemInfo = new OperatingSystemInfo(
           linuxOsReleaseInfo.PrettyName,
           Version.Parse(version), await GetKernelVersionAsync(),
           OperatingSystemFamily.Linux, linuxOsReleaseInfo.VersionId);

       return operatingSystemInfo;
    }
    
    private async Task<Version> GetKernelVersionAsync()
    {
        if (!OperatingSystem.IsLinux())
        {
            throw new PlatformNotSupportedException();
        }
        
        ProcessConfiguration processConfiguration = new ProcessConfiguration("/usr/bin/uname",
            false, true, true, "-v", Environment.CurrentDirectory);
            
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(processConfiguration, ProcessExitConfiguration.Default,
            true, CancellationToken.None);
        
        string versionString = result.StandardOutput.Replace(" ", string.Empty);

        return Version.Parse(versionString);
    }
}