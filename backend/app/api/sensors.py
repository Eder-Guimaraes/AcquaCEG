from fastapi import APIRouter

from app.schemas.sensor import SensorData

router = APIRouter()

@router.get("/latest", response_model = SensorData)
def read_latest():
    return SensorData (
        temperature = 24.5,
        ph = 7.0,
        conductivity = 160.0,
        waterLevel = True
    )