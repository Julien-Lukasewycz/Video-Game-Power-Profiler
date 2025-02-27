using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace PowerProfiler.Runtime
{
    internal class ProfilerDataCollector : MonoBehaviour
    {
        private int frameCount;
        
        private void Awake()
        {
            Profiler.logFile = $"Benchmark/ProfilerData {DateTime.Now:dd-MM-yyyy HH-mm-ss-ffffff}.raw";
            Profiler.enableBinaryLog = true;
            Profiler.enabled = true;
            EditorApplication.isPaused = true;
            // TODO resume after delay
        }

        private void Update()
        {
            frameCount++;
            if (frameCount == 300)
            {
                Profiler.logFile = $"Benchmark/ProfilerData {DateTime.Now:dd-MM-yyyy HH-mm-ss-ffffff}.raw";
                Profiler.enableBinaryLog = true;
                Profiler.enabled = true;
                frameCount = 0;
            }
        }
        
        private void OnApplicationQuit()
        {
            Profiler.enabled = false;
            Profiler.logFile = string.Empty;
        }
    }
}
