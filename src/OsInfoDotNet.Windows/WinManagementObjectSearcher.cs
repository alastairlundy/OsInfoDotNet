using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Specializations.Configurations;
using OsInfoDotNet.Windows.Abstractions;

namespace OsInfoDotNet.Windows
{
    public class WinManagementObjectSearcher : IWinManagementObjectSearcher
    {
        private readonly IProcessInvoker _processInvoker;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processInvoker"></param>
        public WinManagementObjectSearcher(IProcessInvoker processInvoker)
        {
            _processInvoker = processInvoker;
        }

        /// <summary>
        /// Returns a Dictionary of Query objects and their associated WMI values.
        /// WARNING: DO NOT RUN on NON-Windows platforms. This will result in errors.
        /// </summary>
        /// <param name="queryObjectsList"></param>
        /// <param name="wmiClass"></param>
        /// <returns></returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public async Task<Dictionary<string, string>> GetAsync(List<string> queryObjectsList, string wmiClass)
        {
            Dictionary<string, string> queryObjectsDictionary = new Dictionary<string, string>();

            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();

            ClassicPowershellProcessConfiguration classicPowershellConfiguration = new(
                $"Get-WmiObject -Class {wmiClass} | Select-Object *",
                false, true, true);
            
            BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(classicPowershellConfiguration, 
                ProcessExitConfiguration.Default,
                true, CancellationToken.None);
            
            string output = result.StandardOutput.Replace(wmiClass, string.Empty);

            if (output == null)
            {
                throw new ArgumentNullException();
            }

            foreach (string query in queryObjectsList)
            {
                if (query.Contains(output))
                {
                    string value = output.Replace(query + "                         : ", string.Empty);
                    queryObjectsDictionary.Add(query, value);
                }
            }

            return queryObjectsDictionary;
        }
    }
}