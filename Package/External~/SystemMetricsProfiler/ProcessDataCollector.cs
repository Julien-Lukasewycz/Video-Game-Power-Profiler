using System.Diagnostics;
using System.Runtime.Versioning;

namespace SystemMetricsProfiler;

internal class ProcessDataCollector(Process process)
{
    [SupportedOSPlatform("windows")]
    private readonly PerformanceCounter cpuCounter = new("Process", "% Processor Time", GetProcessInstanceName(process.Id), true);
    [SupportedOSPlatform("windows")]
    private readonly PerformanceCounter totalCpuCounter = new("Processor Information", "% Processor Utility", "_Total", true);
    private UtilizationData utilizationData = new();
    
    [SupportedOSPlatform("windows")]
    internal void StartCollecting()
    {
        _ = totalCpuCounter.NextValue();
        _ = cpuCounter.NextValue();
        // TODO
        Thread.Sleep(1000);
    }

    [SupportedOSPlatform("windows")]
    internal UtilizationData GetData()
    {
        utilizationData.CpuCores.Clear();
        utilizationData.totalCpu = totalCpuCounter.NextValue();
        utilizationData.Cpu = cpuCounter.NextValue() / Environment.ProcessorCount;
        // TODO
        return utilizationData;
    }
    
    [SupportedOSPlatform("windows")]
    private static string GetProcessInstanceName(int processId)
    {
        foreach (string? instance in new PerformanceCounterCategory("Process").GetInstanceNames())
        {
            PerformanceCounter counter = new("Process", "ID Process", instance, true);
            if ((int)counter.NextValue() == processId)
            {
                return instance;
            }
        }
        throw new Exception("Process instance not found");
    }
}
