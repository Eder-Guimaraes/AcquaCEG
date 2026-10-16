from fastapi import APIRouter, HTTPException

from app.schemas.sensor import SensorData
from app.services.sensor_service import SensorService
from app.mqtt.mqtt_service import MQTTService

router = APIRouter()

mqtt_service = MQTTService()
mqtt_service.connect()

sensor_service = SensorService(mqtt_service)


@router.get("/latest", response_model=SensorData)
def read_latest():

    if not mqtt_service.get_latest():
        raise HTTPException(
            status_code=503,
            detail="Nenhuma leitura recebida do ESP32."
        )

    return sensor_service.get_latest()