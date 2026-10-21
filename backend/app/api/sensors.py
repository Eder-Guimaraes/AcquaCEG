from fastapi import APIRouter, HTTPException

from app.schemas.sensor import SensorData
from app.core.depedencies import sensor_service

router = APIRouter()

@router.get("/latest", response_model=SensorData)
def read_latest():

    reading = sensor_service.get_latest()

    if reading is None:
        raise HTTPException(
            status_code=404,
            detail="Nenhuma leitura encontrada."
        )

    return reading