using System.Diagnostics;

namespace SystemMetricsProfiler;

public static class Program
{
    private static void Main(string[] args)
    {
        Process process = Process.GetProcessById(int.Parse(args[0]));
        process.Exited += ProcessExited;
        GetProcessMetrics(process);
    }

    private static void ProcessExited(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private static void GetProcessMetrics(Process process)
    {
        // TODO
    }
}
