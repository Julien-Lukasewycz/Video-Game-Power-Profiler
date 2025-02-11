namespace HardwarePowerProfiler;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        PowerDataCollector collector = new();
        collector.StartCollecting();
        PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(1)); // TODO set measurement interval
        Directory.CreateDirectory("Benchmark");
        string path = $"Benchmark/HardwarePowerData {DateTime.Now:dd-MM-yyyy HH-mm-ss}.csv";
        string content = "Time;";
        for (int i = 1; i <= 16; i++)
        {
            content += $"CpuCore{i};";
        }
        content += "CpuPackage;CpuPPT;CpuCorePower;CpuSocPower;CpuMiscPower;CpuTotalPower;IGpuCore;IGpuSoc;GpuPackage\r\n";
        await File.AppendAllTextAsync(path, content);
        while (await periodicTimer.WaitForNextTickAsync())
        {
            PowerData data = collector.GetData();
            content = $"{DateTime.Now:dd:MM:yyyy HH:mm:ss:fff};";
            foreach (float cpuCore in data.CpuCores)
            {
                content += $"{cpuCore};";
            }
            content += $"{data.CpuPackage};{data.CpuPPT};{data.CpuCorePower};{data.CpuSocPower};{data.CpuMiscPower};{data.CpuTotalPower};{data.IGpuCore};{data.IGpuSoc};{data.GpuPackage}\r\n";
            await File.AppendAllTextAsync(path, content);
        }
    }
}
