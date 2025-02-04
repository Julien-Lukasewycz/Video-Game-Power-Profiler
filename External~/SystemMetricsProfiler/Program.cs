using System.Diagnostics;
using System.Runtime.Versioning;

namespace SystemMetricsProfiler;

internal static class Program
{
    [SupportedOSPlatform("windows")]
    private static void Main(string[] args)
    {
        Process process = Process.GetProcessById(int.Parse(args[0]));
        process.Exited += ProcessExited;
        ProcessDataCollector processDataCollector = new(process);
        processDataCollector.GetProcessMetrics(); // TODO
    }

    private static void ProcessExited(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}
