/*
        MIT License
       
       Copyright (c) 2020-2025 Alastair Lundy
       
       Permission is hereby granted, free of charge, to any person obtaining a copy
       of this software and associated documentation files (the "Software"), to deal
       in the Software without restriction, including without limitation the rights
       to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
       copies of the Software, and to permit persons to whom the Software is
       furnished to do so, subject to the following conditions:
       
       The above copyright notice and this permission notice shall be included in all
       copies or substantial portions of the Software.
       
       THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
       IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
       FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
       AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
       LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
       OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
       SOFTWARE.
   */


using System.Runtime.Versioning;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Primitives;

namespace AlastairLundy.OsInfoDotNet.Windows;

/// <summary>
/// A class to make searching the Windows Registry easier.
/// </summary>
public class WinRegistrySearcher : IWinRegistrySearcher
{
    private readonly IProcessInvoker _processInvoker;

    public WinRegistrySearcher(IProcessInvoker processInvoker)
    {
        _processInvoker = processInvoker;
    }
    
    /// <summary>
    ///  Gets the value of a registry key in the Windows registry.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on an Operating System that isn't Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public async Task<string> GetValue(string query){
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            Arguments = $"REG QUERY {query}",
            FileName = Environment.SystemDirectory + Path.DirectorySeparatorChar + "cmd.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };
        
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(startInfo);
          
        if(string.IsNullOrEmpty(result.StandardOutput))
            throw new ArgumentException();
            
        return result.StandardOutput.Replace("REG_SZ", string.Empty);
    }
    
    /// <summary>
    ///  Gets the value of a registry key in the Windows registry.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on an Operating System that isn't Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public async Task<string> GetValue(string query, string value){
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            Arguments = $"REG QUERY {query} /v {value}",
            FileName = Environment.SystemDirectory + Path.DirectorySeparatorChar + "cmd.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };
        
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(startInfo);
          
        if(string.IsNullOrEmpty(result.StandardOutput))
            throw new ArgumentException();
        
        return result.StandardOutput.Replace(value, string.Empty)
            .Replace("REG_SZ", string.Empty);
    }
}