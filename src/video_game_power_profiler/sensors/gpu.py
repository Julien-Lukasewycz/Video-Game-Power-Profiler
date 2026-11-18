from dataclasses import dataclass
from typing import Any
import clr

clr.AddReference("LibreHardwareMonitorLib")
from LibreHardwareMonitor.Hardware import SensorType


@dataclass
class GpuData():
    gpu_power_total: float

    def __init__(self, gpu_power_total: float) -> None:
        self.gpu_power_total = gpu_power_total

    def to_csv(self) -> str:
        return f"{self.gpu_power_total}"

class Gpu():
    __gpu_hardware: Any # TODO type
    __power_sensor_total: Any # TODO type

    def __init__(self, gpu_hardware: Any) -> None: # TODO type
        self.__gpu_hardware = gpu_hardware
        self.__gpu_hardware.Update()
        self.__power_sensor_total = next((sensor for sensor in self.__gpu_hardware.Sensors if sensor.SensorType == SensorType.Power and sensor.Name.startswith("GPU Package")))

    def get_data(self) -> GpuData:
        self.__gpu_hardware.Update()
        gpu_power_total: float = self.__power_sensor_total.Value
        gpu_data: GpuData = GpuData(gpu_power_total)
        return gpu_data
