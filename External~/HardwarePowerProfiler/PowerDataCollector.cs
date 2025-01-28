using LibreHardwareMonitor.Hardware;

namespace HardwarePowerProfiler;

internal class PowerDataCollector
{
    private readonly Computer computer = new()
    {
        IsBatteryEnabled = true,
        IsControllerEnabled = true,
        IsCpuEnabled = true,
        IsGpuEnabled = true,
        IsMemoryEnabled = true,
        IsMotherboardEnabled = true,
        IsNetworkEnabled = true,
        IsPsuEnabled = true,
        IsStorageEnabled = true
    };
    private readonly UpdateVisitor updateVisitor = new();

    internal void StartCollecting()
    {
        computer.Open();
    }

    internal void GetData()
    {
        computer.Accept(updateVisitor);
        // TODO
    }

    internal void StopCollecting()
    {
        computer.Close();
    }
}
