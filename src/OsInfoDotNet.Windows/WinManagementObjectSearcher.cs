

using System.Runtime.Versioning;

using System;
using System.Collections.Generic;
using System.IO;
using AlastairLundy.OsInfoDotNet.Windows.Helpers;

namespace AlastairLundy.OsInfoDotNet.Windows
{
    public class WinManagementObjectSearcher
    {

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
        public static Dictionary<string, string> Get(List<string> queryObjectsList, string wmiClass)
        {
            Dictionary<string, string> queryObjectsDictionary = new Dictionary<string, string>();
            
                if (OperatingSystem.IsWindows())
                {
                    var result = Cli.Wrap($"{WinPowershellInfo.Location}{Path.DirectorySeparatorChar}powershell.exe")
                        .WithArguments($"Get-WmiObject -Class {wmiClass} | Select-Object *")
                        .ExecuteBufferedSync();
                    
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

                throw new PlatformNotSupportedException();
        }
    }
}