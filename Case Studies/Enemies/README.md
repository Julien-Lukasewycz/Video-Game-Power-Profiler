# Enemies (Case Study)

### Description

For this case study, the [Enemies Demo](https://unity.com/demos/enemies) by [Unity Technologies](https://unity.com) was used.
The Video Game Power Profiler was used to capture the power consumption of the demo on the CPU during its 119 seconds of execution time.

### Hardware Configuration

The tower PC used for the measurements possesses the following configuration:

- **OS**: Windows 11 Pro (Version 24H2, 64-bit, power profile balanced)
- **CPU**: AMD Ryzen 9 7950X3D (4.2 GHz)
- **RAM**: Kingston Fury Beast DDR5 (2*32 GB, 6000 MT/s, CL30)
- **GPU**: Gigabyte Gaming OC Nvidia Geforce RTX 3080 Ti (12 GB, Game Ready Driver Version 572.16)
- **MB**: MSI MAG B650 TOMAHAWK WIFI (BIOS Version 7D75vL1)
- **SSD**: Crucial T500 M.2 (2 TB)
- **PSU**: Be quiet! Straight Power 12 (1200 W)

We additionally used the following peripherals:

- **Monitor**: Monitor Fujitsu P27-8 TS UHD (3840*2160 Pixel, 60 Hz)

### Demo Configuration

We used version 1.0 of the demo for our measurement.
The demo has been run in-engine using Unity version [2023.1.20f1](https://unity.com/releases/editor/whats-new/2023.1.20f1).
The demo itself was configured as follows:

- **Resolution**: 1,920*1,080 pixels
- **Target Frame Rate**: 60 FPS
- **VSync**: enabled

All other settings inside the engine and the demo have been kept as provided.

### Power Profiler Configuration

Version [0.0.0](https://github.com/Julien-Lukasewycz/Video-Game-Power-Profiler/releases/tag/v0.0.0) of the power profiler has been imported into Unity to conduct the measurement.
The power profiler has been configured to measure the power consumption on the CPU and operating system metrics with a frequency of 120 Hz.
Within the engine, every frame was measured to collect metrics about the demo itself using a polling rate of 60 Hz.

### Result Files

We are providing the following files containing our measurement results:

- **HardwarePowerData.csv**: The measured power consumption of hardware components collected by hardware sensors.
    - _Time_: The time the data point was measured.
    - _CpuCoreX_: The power consumption of individual CPU cores in watts.
    - _CpuPackage_, _CpuPPT_, _CpuCorePower_, _CpuSocPower_, _CpuMiscPower_, _CpuTotalPower_, _IGpuCore_, _IGpuSoc_: Various power consumption values of the CPU in watts.
    - _GpuPackage_: Power consumption of the GPU Package in watts.
- **SystemMetricsData.csv**: Operating system metrics tracking the total utilization of the CPU and how much utilization the demo itself uses.
    - _Time_: The time the data point was measured.
    - _TotalCpuUtilization_: The total utilization of the CPU.
    - _ApplicationUtilization_: The proportional utilization of the demo.
- **ProfilerUtilizationData.csv**: Information collected from the Unity profiler providing information about the internals of the demo.
    - _FrameIndex_: The index of the measured frame.
    - _FrameTime_: The duration the current frame was computed in milliseconds.
    - _FrameFPS_: The average frame rate during the current frame.
    - _Rendering_, _Audio_, _Physics_, _UI_, _Lighting_, _VFX_, _AI_, _Scripting_, _IO_, _Other_, _Video_, _Animation_, _Input_: The utilization information of different components within the demo in percent (sum is 100%).

### Remarks

- All measurements have been conducted multiple times to validate the correctness of the results.
- It has been ensured that the environment, e.g., operating system, has as little impact on the resulting data as possible. The influence of the operating system and other applications has been reduced by using an idling system, and only necessary applications were running.
- In the _ProfilerUtilizationData.csv_, every 300 rows (frame 300, 600, 900, etc.), the profiler suddenly provides unrealistic values. This happens because the profiler has ended one recording cycle and started a new one, leading to incorrect measurements. Skipping it and continuing with the value after resolves this problem when analyzing the data; this procedure misses no frame.

### Related Work

The data of this case study was used in the following research articles:

- "[*Measuring the Power Consumption of Video Games*](https://doi.org/10.1109/cog64752.2025.11114260)" by Julien Lukasewycz and Torben Weis

### Questions

If you have questions regarding any information provided for this case study or feel like there might be some information missing, please contact [Julien Lukasewycz](https://github.com/Julien-Lukasewycz) via the email address [julien.lukasewycz@uni-due.de](mailto:julien.lukasewycz@uni-due.de).
