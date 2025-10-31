using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Core;

using OsInfoDotNet.Abstractions;
using OsInfoDotnet.Android.Abstractions;
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace OsInfoDotnet.Android;

/// <summary>
/// 
/// </summary>
public class AndroidOperatingSystemInfoProvider : IOperatingSystemInfoProvider, 
    IAndroidOperatingSystemInfoProvider
{
    private readonly IProcessInvoker _processInvoker;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    public AndroidOperatingSystemInfoProvider(IProcessInvoker processInvoker)
    {
        _processInvoker = processInvoker;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OperatingSystemInfo> GetOperatingSystemInfoAsync(CancellationToken cancellationToken)
    {
         OperatingSystemInfo operatingSystemInfo = new(await GetPlatformNameAsync(cancellationToken),
             await GetPlatformVersionAsync(cancellationToken),
             await GetKernelVersionAsync(cancellationToken),
             OperatingSystemFamily.Android,
             await GetBuildNumberAsync(cancellationToken));

         return operatingSystemInfo;
    }

    [SupportedOSPlatform("android")]
    [SupportedOSPlatform("linux")]
    private async Task<string> GetUnameValueAsync(string property, CancellationToken cancellationToken)
    {
        ProcessConfiguration processConfiguration = new("/usr/bin/uname", false,
            true, true, property);
        
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(processConfiguration,
            ProcessExitConfiguration.Default, true, cancellationToken);
            
        return result.StandardOutput;
    }

        [SupportedOSPlatform("android")]
        [SupportedOSPlatform("linux")]
        private async Task<string> GetBuildNumberAsync(CancellationToken cancellationToken)
        {
            string descProp = await GetPropValueAsync("ro.build.description", cancellationToken);
                
            string[] results = descProp.Split(' ');
                
            return results[3];
        }
        
        [SupportedOSPlatform("android")]
        private async Task<string> GetPlatformNameAsync(CancellationToken cancellationToken) 
            => await GetUnameValueAsync("-o", cancellationToken);

        
        [SupportedOSPlatform("android")]
        private async Task<Version> GetPlatformVersionAsync(CancellationToken cancellationToken)
        {
            string version = await GetPropValueAsync("release", cancellationToken);
            
            return Version.Parse(version);
        }

        [SupportedOSPlatform("android")]
        [SupportedOSPlatform("linux")]
        private async Task<Version> GetKernelVersionAsync(CancellationToken cancellationToken)
        {
                string result = await GetUnameValueAsync("-r", cancellationToken);

                int indexOfDash = result.IndexOf('-');

                string versionString = indexOfDash != -1 ?
                    result.Substring(indexOfDash, result.Length - indexOfDash) : result;
                
                return Version.Parse(versionString);
        }
    

        [SupportedOSPlatform("android")]
        private async Task<string> GetPropValueAsync(string value, CancellationToken cancellationToken)
        {
            if (OperatingSystem.IsAndroid() == false)
                throw new PlatformNotSupportedException();
            
            ProcessConfiguration processConfiguration = new("getprop", false,
                true, true, $"ro.build.version.{value}");
            
            BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(processConfiguration,
                ProcessExitConfiguration.Default, true, cancellationToken);
            
            return result.StandardOutput.Replace(" ", string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [SupportedOSPlatform("android")]
        public async Task<string> GetOSVersionCodenameAsync(CancellationToken cancellationToken)
            => await GetPropValueAsync("codename", cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [SupportedOSPlatform("android")]
        public async Task<Version> GetSdkApiLevelAsync(CancellationToken cancellationToken)
        {
            string version = await GetPropValueAsync("sdk", cancellationToken);

            if (version.Count(x => x == '.') < 1)
                version = $"{version}.0";
            
            return Version.Parse(version);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [SupportedOSPlatform("android")]
        public async Task<string> GetDeviceModelAsync(CancellationToken cancellationToken)
            => await GetPropValueAsync("ro.product.model", cancellationToken);
}