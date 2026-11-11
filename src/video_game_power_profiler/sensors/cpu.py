from dataclasses import dataclass
from typing import Any
import clr

clr.AddReference("LibreHardwareMonitorLib")
from LibreHardwareMonitor.Hardware import SensorType


@dataclass
class CpuData():
    cpu_power_total: float
    cpu_power_cores: list[float]

    def __init__(self, cpu_power_total: float, cpu_power_cores: list[float]) -> None:
        self.cpu_power_total = cpu_power_total
        self.cpu_power_cores = cpu_power_cores


class Cpu():
    __cpu_hardware: Any # TODO type
    __power_sensor_total: Any # TODO type
    __power_sensors_cores: list[Any] # TODO type

    def __init__(self, cpu_hardware: Any) -> None: # TODO type
        self.__cpu_hardware = cpu_hardware
        self.__cpu_hardware.Update()
        self.__power_sensor_total = next((sensor for sensor in self.__cpu_hardware.Sensors if sensor.SensorType == SensorType.Power and sensor.Name.startswith("Package")), None)
        self.__power_sensors_cores = [sensor for sensor in self.__cpu_hardware.Sensors if sensor.SensorType == SensorType.Power and sensor.Name.startswith("Core")]

    def get_data(self) -> CpuData:
        self.__cpu_hardware.Update()
        power_total: float = self.__power_sensor_total.Value
        power_cores: list[float] = [sensor.Value for sensor in self.__power_sensors_cores]
        cpu_data = CpuData(power_total, power_cores)
        return cpu_data
