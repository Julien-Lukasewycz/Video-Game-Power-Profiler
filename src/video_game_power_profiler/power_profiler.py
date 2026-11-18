from dataclasses import dataclass
from io import TextIOWrapper
import os
from threading import Thread
import time
from typing import Any
import clr

from video_game_power_profiler.sensors.cpu import Cpu, CpuData
from video_game_power_profiler.sensors.gpu import Gpu, GpuData

clr.AddReference("LibreHardwareMonitorLib")
from LibreHardwareMonitor.Hardware import Computer, HardwareType


@dataclass
class PowerProfilerConfiguration():
    working_directory_path: str
    polling_rate: int

    def __init__(self, working_directory_path: str, polling_rate: int) -> None:
        self.working_directory_path = working_directory_path
        self.polling_rate = polling_rate


@dataclass
class ProfilerData():
    pass # TODO


class PowerProfiler():
    __computer: Any = Computer() # TODO type
    __computer.IsCpuEnabled = True
    __computer.IsGpuEnabled = True
    __computer.IsMemoryEnabled = True
    __computer.IsStorageEnabled = True
    __configuration: PowerProfilerConfiguration
    __output_file: TextIOWrapper
    __profiling_thread: Thread = Thread()
    __profiling_thread_kill_flag: bool = True

    def __init__(self, configuration: PowerProfilerConfiguration) -> None:
        self.__configuration = configuration

    def start_profiling(self, process_id: int, file_name: str) -> None:
        self.__computer.Open()
        os.makedirs(f"{self.__configuration.working_directory_path}Data/", exist_ok=True)
        self.__output_file = open(f"{self.__configuration.working_directory_path}Data/{file_name}.csv", "w")
        cpu_hardware = next((hardware for hardware in self.__computer.Hardware if hardware.HardwareType == HardwareType.Cpu))
        cpu = Cpu(cpu_hardware)
        gpu_hardware = next((hardware for hardware in self.__computer.Hardware if hardware.HardwareType == HardwareType.GpuNvidia))
        gpu = Gpu(process_id, gpu_hardware)
        self.__profiling_thread_kill_flag = False
        self.__profiling_thread = Thread(target=self.profile, args=(cpu, gpu,))
        self.__profiling_thread.start()

    def profile(self, cpu: Cpu, gpu: Gpu) -> None:
        cpu_data: CpuData
        gpu_data: GpuData
        while(not(self.__profiling_thread_kill_flag)):
            cpu_data = cpu.get_data()
            gpu_data = gpu.get_data()
            self.__output_file.write(f"{cpu_data.to_csv()};{gpu_data.to_csv()}\n")
            time.sleep(1 / self.__configuration.polling_rate)

    def stop_profiling(self) -> None:
        self.__profiling_thread_kill_flag = True
        self.__profiling_thread.join()
        self.__output_file.close()
        self.__computer.Close()
