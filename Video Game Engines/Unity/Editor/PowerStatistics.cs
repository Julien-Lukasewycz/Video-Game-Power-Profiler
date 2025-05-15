using Unity.Profiling;

namespace PowerProfiler.Editor
{
    internal static class PowerStatistics
    {
        internal const string RENDERING_POWER_NAME = "Rendering Power";
        internal const string AUDIO_POWER_NAME = "Audio Power";
        internal const string PHYSICS_POWER_NAME = "Physics Power";
        internal const string UI_POWER_NAME = "UI Power";
        internal const string LIGHTING_POWER_NAME = "Lighting Power";
        internal const string VFX_POWER_NAME = "VFX Power";
        internal const string AI_POWER_NAME = "AI Power";
        internal const string SCRIPTING_POWER_NAME = "Scripting Power";
        internal const string IO_POWER_NAME = "IO Power";
        internal const string OTHER_POWER_NAME = "Other Power";
        internal const string VIDEO_POWER_NAME = "Video Power";
        internal const string ANIMATION_POWER_NAME = "Animation Power";
        internal const string INPUT_POWER_NAME = "Input Power";
        
        internal static readonly ProfilerCategory RenderingPowerCategory = new(RENDERING_POWER_NAME);
        internal static readonly ProfilerCategory AudioPowerCategory = new(AUDIO_POWER_NAME);
        internal static readonly ProfilerCategory PhysicsPowerCategory = new(PHYSICS_POWER_NAME);
        internal static readonly ProfilerCategory UIPowerCategory = new(UI_POWER_NAME);
        internal static readonly ProfilerCategory LightingPowerCategory = new(LIGHTING_POWER_NAME);
        internal static readonly ProfilerCategory VFXPowerCategory = new(VFX_POWER_NAME);
        internal static readonly ProfilerCategory AIPowerCategory = new(AI_POWER_NAME);
        internal static readonly ProfilerCategory ScriptingPowerCategory = new(SCRIPTING_POWER_NAME);
        internal static readonly ProfilerCategory IOPowerCategory = new(IO_POWER_NAME);
        internal static readonly ProfilerCategory OtherPowerCategory = new(OTHER_POWER_NAME);
        internal static readonly ProfilerCategory VideoPowerCategory = new(VIDEO_POWER_NAME);
        internal static readonly ProfilerCategory AnimationPowerCategory = new(ANIMATION_POWER_NAME);
        internal static readonly ProfilerCategory InputPowerCategory = new(INPUT_POWER_NAME);
    }
}
