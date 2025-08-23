using System.Runtime.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Primitives;
using AlastairLundy.OsInfoDotNet.Windows.Helpers;

namespace AlastairLundy.OsInfoDotNet.Windows
{
    public class WinManagementObjectSearcher
    {
        private readonly IProcessInvoker _processInvoker;

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
        public async Task<Dictionary<string, string>> Get(List<string> queryObjectsList, string wmiClass)
        {
            Dictionary<string, string> queryObjectsDictionary = new Dictionary<string, string>();

            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = $"Get-WmiObject -Class {wmiClass} | Select-Object *",
                FileName = $"{WinPowershellInfo.Location}{Path.DirectorySeparatorChar}powershell.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(startInfo);

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