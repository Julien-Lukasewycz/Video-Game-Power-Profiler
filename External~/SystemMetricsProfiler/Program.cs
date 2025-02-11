using System.Diagnostics;
using System.Runtime.Versioning;

namespace SystemMetricsProfiler;

internal static class Program
{
    [SupportedOSPlatform("windows")]
    private static async Task Main(string[] args)
    {
        Process process = Process.GetProcessById(int.Parse(args[0]));
        ProcessDataCollector processDataCollector = new(process);
        PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(1)); // TODO set measurement interval
        Directory.CreateDirectory("Benchmark");
        string path = $"Benchmark/ProcessUtilizationData {DateTime.Now:dd-MM-yyyy HH-mm-ss}.csv";
        string content = "Time;";
        for (int i = 1; i <= 16; i++)
        {
            content += $"CpuCoreUtilization{i};";
        }
        content += "CpuUtilization\r\n";
        await File.AppendAllTextAsync(path, content);
        while (await periodicTimer.WaitForNextTickAsync())
        {
            UtilizationData utilizationData = processDataCollector.GetData();
            content = $"{DateTime.Now:dd:MM:yyyy HH:mm:ss:fff};";
            foreach (float cpuCoreUtilization in utilizationData.CpuCores)
            {
                content += $"{cpuCoreUtilization};";
            }
            content += $"{utilizationData.Cpu};\r\n";
            await File.AppendAllTextAsync(path, content);
        }
    }
}
