from fastapi import APIRouter

from app.schemas.sensor import SensorData
from app.services.sensor_service import SensorService

router = APIRouter()

sensor_service = SensorService()

@router.get("/latest", response_model = SensorData)
def read_latest():
    return sensor_service.get_latest()