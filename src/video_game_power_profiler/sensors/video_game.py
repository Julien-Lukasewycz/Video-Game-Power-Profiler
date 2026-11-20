from dataclasses import dataclass


@dataclass
class VideoGameData():
    frame_time: float
    frames_per_second: int

    def __init__(self, frame_time: float, frames_per_second: int) -> None:
        self.frame_time = frame_time
        self.frames_per_second = frames_per_second

    def to_csv(self) -> str:
        return f"{self.frame_time};{self.frames_per_second}"


class VideoGame():
    def get_data(self) -> VideoGameData:
        with self.__video_game_data_lock:
            return self.__video_game_data
