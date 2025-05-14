using System.Diagnostics;
using UnityEngine;

namespace PowerProfiler.Runtime
{
    internal class HardwarePowerProfiler : MonoBehaviour
    {
        private readonly Process process = new();
        
        private void Awake()
        {
            process.StartInfo.FileName = "Assets\\Power Profiler\\Package\\External~\\HardwarePowerProfiler\\bin\\Release\\net8.0\\HardwarePowerProfiler.exe"; // TODO use published files
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.Verb = "runas";
            process.Start();
            Application.quitting += StopProcess;
        }

        private void StopProcess()
        {
            process.Kill();
        }
    }
}
