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
    private readonly Dictionary<string, int[]> sensorPositions = new();
    private PowerData powerData = new();
    private readonly UpdateVisitor updateVisitor = new();

    internal void StartCollecting()
    {
        computer.Open();
        computer.Accept(updateVisitor);
        int cpuPosition = Array.FindIndex(computer.Hardware.ToArray(), hardware => hardware.Name == "AMD Ryzen 9 7950X3D");
        int iGpuPosition = Array.FindIndex(computer.Hardware.ToArray(), hardware => hardware.Name == "AMD Radeon(TM) Graphics");
        int gpuPosition = Array.FindIndex(computer.Hardware.ToArray(), hardware => hardware.Name == "NVIDIA GeForce RTX 3080 Ti");
        for (int i = 1; i <= 16; i++)
        {
            string name = $"Core #{i} (SMU)";
            sensorPositions.Add(name, [cpuPosition, Array.FindIndex(computer.Hardware[cpuPosition].Sensors, sensor => sensor.Name == name)]);
        }
        sensorPositions.Add("Package", [cpuPosition, Array.FindIndex(computer.Hardware[cpuPosition].Sensors, sensor => sensor.Name == "Package")]);
        sensorPositions.Add("CPU PPT", [cpuPosition, Array.FindIndex(computer.Hardware[cpuPosition].Sensors, sensor => sensor.Name == "CPU PPT")]);
        sensorPositions.Add("Core Power", [cpuPosition, Array.FindIndex(computer.Hardware[cpuPosition].Sensors, sensor => sensor.Name == "Core Power")]);
        sensorPositions.Add("SOC Power", [cpuPosition, Array.FindIndex(computer.Hardware[cpuPosition].Sensors, sensor => sensor.Name == "SOC Power")]);
        sensorPositions.Add("Misc Power", [cpuPosition, Array.FindIndex(computer.Hardware[cpuPosition].Sensors, sensor => sensor.Name == "Misc Power")]);
        sensorPositions.Add("Total Power", [cpuPosition, Array.FindIndex(computer.Hardware[cpuPosition].Sensors, sensor => sensor.Name == "Total Power")]);
        sensorPositions.Add("GPU Core", [iGpuPosition, Array.FindIndex(computer.Hardware[iGpuPosition].Sensors, sensor => sensor.Name == "GPU Core")]);
        sensorPositions.Add("GPU SoC", [iGpuPosition, Array.FindIndex(computer.Hardware[iGpuPosition].Sensors, sensor => sensor.Name == "GPU SoC")]);
        sensorPositions.Add("GPU Package", [gpuPosition, Array.FindIndex(computer.Hardware[gpuPosition].Sensors, sensor => sensor.Name == "GPU Package")]);
    }

    internal PowerData GetData()
    {
        powerData.CpuCores.Clear();
        computer.Accept(updateVisitor);
        for (int i = 1; i <= 16; i++)
        {
            string name = $"Core #{i} (SMU)";
            powerData.CpuCores.Add(computer.Hardware[sensorPositions[name][0]].Sensors[sensorPositions[name][1]].Value ?? 0);
        }
        powerData.CpuPackage = computer.Hardware[sensorPositions["Package"][0]].Sensors[sensorPositions["Package"][1]].Value ?? 0;
        powerData.CpuPPT = computer.Hardware[sensorPositions["CPU PPT"][0]].Sensors[sensorPositions["CPU PPT"][1]].Value ?? 0;
        powerData.CpuCorePower = computer.Hardware[sensorPositions["Core Power"][0]].Sensors[sensorPositions["Core Power"][1]].Value ?? 0;
        powerData.CpuSocPower = computer.Hardware[sensorPositions["SOC Power"][0]].Sensors[sensorPositions["SOC Power"][1]].Value ?? 0;
        powerData.CpuMiscPower = computer.Hardware[sensorPositions["Misc Power"][0]].Sensors[sensorPositions["Misc Power"][1]].Value ?? 0;
        powerData.CpuTotalPower = computer.Hardware[sensorPositions["Total Power"][0]].Sensors[sensorPositions["Total Power"][1]].Value ?? 0;
        powerData.IGpuCore = computer.Hardware[sensorPositions["GPU Core"][0]].Sensors[sensorPositions["GPU Core"][1]].Value ?? 0;
        powerData.IGpuSoc = computer.Hardware[sensorPositions["GPU SoC"][0]].Sensors[sensorPositions["GPU SoC"][1]].Value ?? 0;
        powerData.GpuPackage = computer.Hardware[sensorPositions["GPU Package"][0]].Sensors[sensorPositions["GPU Package"][1]].Value ?? 0;
        return powerData;
    }

    internal void StopCollecting()
    {
        computer.Close();
    }
}
