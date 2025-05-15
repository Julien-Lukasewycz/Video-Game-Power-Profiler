using Unity.Profiling.Editor;

namespace PowerProfiler.Editor
{
    [ProfilerModuleMetadata("CPU Power (1/2)")] // TODO add icon
    internal class CPUPowerProfilerModule1 : ProfilerModule
    {
        private static readonly ProfilerCounterDescriptor[] KCounters = {
            new(PowerStatistics.RENDERING_POWER_NAME, PowerStatistics.RenderingPowerCategory),
            new(PowerStatistics.AUDIO_POWER_NAME, PowerStatistics.AudioPowerCategory),
            new(PowerStatistics.PHYSICS_POWER_NAME, PowerStatistics.PhysicsPowerCategory),
            new(PowerStatistics.UI_POWER_NAME, PowerStatistics.UIPowerCategory),
            new(PowerStatistics.LIGHTING_POWER_NAME, PowerStatistics.LightingPowerCategory),
            new(PowerStatistics.VFX_POWER_NAME, PowerStatistics.VFXPowerCategory),
            new(PowerStatistics.AI_POWER_NAME, PowerStatistics.AIPowerCategory),
            new(PowerStatistics.SCRIPTING_POWER_NAME, PowerStatistics.ScriptingPowerCategory),
            new(PowerStatistics.IO_POWER_NAME, PowerStatistics.IOPowerCategory),
            new(PowerStatistics.OTHER_POWER_NAME, PowerStatistics.OtherPowerCategory)
        };

        public CPUPowerProfilerModule1() : base(KCounters) {}
    }
    
    [ProfilerModuleMetadata("CPU Power (2/2)")] // TODO add icon
    internal class CPUPowerProfilerModule2 : ProfilerModule
    {
        private static readonly ProfilerCounterDescriptor[] KCounters = {
            new(PowerStatistics.VIDEO_POWER_NAME, PowerStatistics.VideoPowerCategory),
            new(PowerStatistics.ANIMATION_POWER_NAME, PowerStatistics.AnimationPowerCategory),
            new(PowerStatistics.INPUT_POWER_NAME, PowerStatistics.InputPowerCategory)
        };

        public CPUPowerProfilerModule2() : base(KCounters) {}
    }
}
