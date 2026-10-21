from app.schemas.sensor import SensorData
from app.models.sensor_reading import SensorReading

class SensorService:

    def __init__(self, repository):
        self.repository = repository

    def convert_temperature(self, temperature):
        return temperature / 100

    def convert_ph(self, adc):
        #esses valores podem variar um pouco enquanto ainda houver ruido elétrico
         m = 0
         b = 0

         return m * adc + b

    def convert_condut(self, adc):
        #esses valores podem variar um pouco enquanto ainda houver ruido elétrico
        m = 0
        b = 0
    
        return m * adc + b

    def convert_water_level(self, water_level):
        return water_level == 1

    def _convert_raw_data(self, raw_data):
        
        raw_ph = raw_data["adc_ph"]
        raw_condut = raw_data["adc_condut"]
        raw_temperature = raw_data["temperature"]
        raw_water_level = raw_data["water_level"]
            
        temperature = self.convert_temperature(raw_temperature)
        ph = self.convert_ph(raw_ph)
        condut = self.convert_condut(raw_condut)
        waterLevel = self.convert_water_level(raw_water_level)

        return temperature, ph, condut, waterLevel

    def get_latest(self):
        reading = self.repository.get_latest()

        if reading is None:
            return None

        return SensorData(
            temperature=reading.temperature,
            ph=reading.ph,
            conductivity=reading.conductivity,
            waterLevel=reading.water_level
        )
        

    def save_reading(self, raw_data):
        temperature, ph, condut, water_level = self._convert_raw_data(raw_data)

        reading = SensorReading(
            ph=ph,
            conductivity=condut,
            temperature=temperature,
            water_level=water_level
        )

        saved = self.repository.save(reading)

        return saved