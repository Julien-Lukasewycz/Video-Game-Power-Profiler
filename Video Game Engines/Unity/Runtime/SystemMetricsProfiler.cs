using System.Diagnostics;
using UnityEngine;

namespace PowerProfiler.Runtime
{
    internal class SystemMetricsProfiler : MonoBehaviour
    {
        private readonly Process process = new();
        
        private void Awake()
        {
            process.StartInfo.FileName = "Assets\\Power Profiler\\Package\\External~\\SystemMetricsProfiler\\bin\\Release\\net8.0\\SystemMetricsProfiler.exe"; // TODO use published files
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.Verb = "runas";
            process.StartInfo.Arguments = $"{Process.GetCurrentProcess().Id}"; // TODO get correct process id
            process.Start();
            Application.quitting += StopProcess;
        }

        private void StopProcess()
        {
            process.Kill();
        }
    }
}
