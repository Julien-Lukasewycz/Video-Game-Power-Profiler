from dataclasses import dataclass
from typing import Any
import clr

clr.AddReference("LibreHardwareMonitorLib")
from LibreHardwareMonitor.Hardware import Computer


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

    def __init__(self, configuration: PowerProfilerConfiguration) -> None:
        self.__configuration = configuration

    def start_profiling(self, process_id: int) -> None:
        self.__computer.Open()
        pass # TODO

    def stop_profiling(self) -> None:
        self.__computer.Close()
        pass # TODO
