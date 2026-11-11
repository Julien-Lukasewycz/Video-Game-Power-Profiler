from dataclasses import dataclass


@dataclass
class PowerProfilerConfiguration():
    working_directory_path: str
    polling_rate: int

    def __init__(self, working_directory_path: str, polling_rate: int) -> None:
        self.working_directory_path = working_directory_path
        self.polling_rate = polling_rate


class PowerProfiler():
    configuration: PowerProfilerConfiguration

    def __init__(self, configuration: PowerProfilerConfiguration) -> None:
        self.configuration = configuration

    def start_profiling(self):
        pass # TODO

    def stop_profiling(self):
        pass # TODO
