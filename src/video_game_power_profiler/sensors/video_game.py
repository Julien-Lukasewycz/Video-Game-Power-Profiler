import ctypes
from dataclasses import dataclass
from subprocess import Popen
from threading import Lock, Thread
import time


@dataclass
class VideoGameData():
    frame_time: float
    frames_per_second: int

    def __init__(self, frame_time: float, frames_per_second: int) -> None:
        self.frame_time = frame_time
        self.frames_per_second = frames_per_second

    def to_csv(self) -> str:
        return f"{self.frame_time};{self.frames_per_second}"


class PM_QUERY_ELEMENT(ctypes.Structure):
    _fields_ = [
        ("metric", ctypes.c_int),
        ("stat", ctypes.c_int),
        ("deviceId", ctypes.c_uint32),
        ("arrayIndex", ctypes.c_uint32),
        ("dataOffset", ctypes.c_uint64),
        ("dataSize", ctypes.c_uint64),
    ]


class VideoGame():
    __presentmon_dll: ctypes.WinDLL
    __presentmon_query_elements: ctypes.Array[PM_QUERY_ELEMENT] = (PM_QUERY_ELEMENT * 2)(
        PM_QUERY_ELEMENT(
            metric=87,
            stat=12,
            deviceId=0,
            arrayIndex=0,
            dataOffset=0,
            dataSize=ctypes.sizeof(ctypes.c_double),
        ),
        PM_QUERY_ELEMENT(
            metric=62,
            stat=12,
            deviceId=0,
            arrayIndex=0,
            dataOffset=ctypes.sizeof(ctypes.c_double),
            dataSize=ctypes.sizeof(ctypes.c_double),
        ),
    )
    __presentmon_query: ctypes.c_void_p
    __presentmon_service: Popen[bytes]
    __presentmon_session: ctypes.c_void_p
    __presentmon_update_thread: Thread
    __process_id: ctypes.c_uint32
    __video_game_data: VideoGameData
    __video_game_data_lock: Lock

    def __init__(self, process_id: int, presentmon_service_path: str, presentmon_service_dll_path: str) -> None:
        self.__presentmon_service = Popen([presentmon_service_path]) # TODO terminate service
        self.__process_id = ctypes.c_uint32(process_id)
        self.__presentmon_dll = ctypes.WinDLL(presentmon_service_dll_path)
        self.__presentmon_dll.pmOpenSession.argtypes = [ctypes.POINTER(ctypes.c_void_p)]
        self.__presentmon_dll.pmOpenSession.restype = ctypes.c_int
        self.__presentmon_dll.pmCloseSession.argtypes = [ctypes.c_void_p]
        self.__presentmon_dll.pmCloseSession.restype = ctypes.c_int
        self.__presentmon_dll.pmStartTrackingProcess.argtypes = [ctypes.c_void_p, ctypes.c_uint32]
        self.__presentmon_dll.pmStartTrackingProcess.restype = ctypes.c_int
        self.__presentmon_dll.pmStopTrackingProcess.argtypes = [ctypes.c_void_p, ctypes.c_uint32]
        self.__presentmon_dll.pmStopTrackingProcess.restype = ctypes.c_int
        self.__presentmon_dll.pmSetEtwFlushPeriod.argtypes = [ctypes.c_void_p, ctypes.c_uint32]
        self.__presentmon_dll.pmSetEtwFlushPeriod.restype = ctypes.c_int
        self.__presentmon_dll.pmRegisterDynamicQuery.argtypes = [ctypes.c_void_p, ctypes.POINTER(ctypes.c_void_p), ctypes.POINTER(PM_QUERY_ELEMENT), ctypes.c_uint64, ctypes.c_double, ctypes.c_double]
        self.__presentmon_dll.pmRegisterDynamicQuery.restype = ctypes.c_int
        self.__presentmon_dll.pmPollDynamicQuery.argtypes = [ctypes.c_void_p, ctypes.c_uint32, ctypes.POINTER(ctypes.c_uint8), ctypes.POINTER(ctypes.c_uint32)]
        self.__presentmon_dll.pmPollDynamicQuery.restype = ctypes.c_int
        self.__presentmon_query = ctypes.c_void_p(None)
        self.__presentmon_session = ctypes.c_void_p(None)
        self.__presentmon_dll.pmOpenSession(ctypes.byref(self.__presentmon_session))
        self.__presentmon_dll.pmSetEtwFlushPeriod(self.__presentmon_session, ctypes.c_uint32(10))
        self.__presentmon_dll.pmStartTrackingProcess(self.__presentmon_session, self.__process_id)
        self.__presentmon_dll.pmRegisterDynamicQuery(self.__presentmon_session, ctypes.byref(self.__presentmon_query), self.__presentmon_query_elements, ctypes.c_uint64(2), ctypes.c_double(100), ctypes.c_double(0))
        self.__video_game_data = VideoGameData(0, 0)
        self.__video_game_data_lock = Lock()
        self.__presentmon_update_thread = Thread(target=self.__update_presentmon_data, daemon=True)
        self.__presentmon_update_thread.start()

    def __update_presentmon_data(self) -> None:
        while(True):
            number_swaped_chains = ctypes.c_uint32(8)
            data: ctypes.Array[ctypes.c_uint8] = (ctypes.c_uint8 * 256)()
            error: int = self.__presentmon_dll.pmPollDynamicQuery(self.__presentmon_query, self.__process_id, ctypes.cast(data, ctypes.POINTER(ctypes.c_uint8)), ctypes.byref(number_swaped_chains))
            if error == 0 and number_swaped_chains.value != 0:
                frame_time: float = ctypes.c_double.from_buffer(data, 0).value
                frames_per_second : int = int(ctypes.c_double.from_buffer(data, ctypes.sizeof(ctypes.c_double)).value)
                with self.__video_game_data_lock:
                    self.__video_game_data = VideoGameData(frame_time, frames_per_second)
            time.sleep(0.002)

    def get_data(self) -> VideoGameData:
        with self.__video_game_data_lock:
            return self.__video_game_data
