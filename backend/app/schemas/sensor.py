from pydantic import BaseModel

class SensorData(BaseModel):
    temperature: float
    ph: float
    conductivity: int
    waterLevel: bool