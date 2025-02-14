namespace HardwarePowerProfiler;

internal struct PowerData
{
    internal List<float> CpuCores = [];
    internal float CpuPackage = 0;
    internal float CpuPPT = 0;
    internal float CpuCorePower = 0;
    internal float CpuSocPower = 0;
    internal float CpuMiscPower = 0;
    internal float CpuTotalPower = 0;
    internal float IGpuCore = 0;
    internal float IGpuSoc = 0;
    internal float GpuPackage = 0;

    public PowerData() {}
}
