from dataclasses import dataclass
from typing import Any
import clr
from nvitop.api import Device, GpuProcess, NaType, PhysicalDevice

clr.AddReference("LibreHardwareMonitorLib")
from LibreHardwareMonitor.Hardware import SensorType


@dataclass
class GpuData():
    gpu_power_total: float
    gpu_utilization_total: int
    gpu_process_utilization_total: float
    gpu_process_power_total: float

    def __init__(self, gpu_power_total: float, gpu_utilization_total: int, gpu_process_utilization_total: float, gpu_process_power_total: float) -> None:
        self.gpu_power_total = gpu_power_total
        self.gpu_utilization_total = gpu_utilization_total
        self.gpu_process_utilization_total = gpu_process_utilization_total
        self.gpu_process_power_total = gpu_process_power_total

    def to_csv(self) -> str:
        return f"{self.gpu_power_total};{self.gpu_utilization_total};{self.gpu_process_utilization_total};{self.gpu_process_power_total}"

class Gpu():
    __gpu_hardware: Any # TODO type
    __power_sensor_total: Any # TODO type
    __gpu_device: PhysicalDevice
    __gpu_process: GpuProcess

    def __init__(self, process_id: int, gpu_hardware: Any) -> None: # TODO type
        self.__gpu_hardware = gpu_hardware
        self.__gpu_hardware.Update()
        self.__power_sensor_total = next((sensor for sensor in self.__gpu_hardware.Sensors if sensor.SensorType == SensorType.Power and sensor.Name.startswith("GPU Package")))
        self.__gpu_device = Device.all()[0]
        self.__gpu_process = self.__gpu_device.processes().get(process_id) # type: ignore # TODO

    def get_data(self) -> GpuData:
        self.__gpu_hardware.Update()
        gpu_power_total: float = self.__power_sensor_total.Value
        gpu_utilization_total: int | NaType = self.__gpu_device.gpu_utilization()
        if type(gpu_utilization_total) == NaType:
            gpu_utilization_total = 0
        self.__gpu_process.update_gpu_status()
        gpu_process_utilization_total: int | NaType = self.__gpu_process.gpu_sm_utilization()
        if type(gpu_process_utilization_total) == NaType:
            gpu_process_utilization_total = 0
        gpu_process_power_total: float = gpu_power_total if gpu_utilization_total <= gpu_process_utilization_total else gpu_power_total * ((gpu_process_utilization_total * 0.01) / (gpu_utilization_total * 0.01))
        gpu_data: GpuData = GpuData(gpu_power_total, gpu_utilization_total, gpu_process_utilization_total, gpu_process_power_total)
        return gpu_data
