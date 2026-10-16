from app.schemas.sensor import SensorData

class SensorService:

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

    def get_latest(self):
        raw_ph = 0
        raw_condut = 0
        raw_temperature = 0
        raw_water_level = 0
    
        temperature = self.convert_temperature(raw_temperature)
        ph = self.convert_ph(raw_ph)
        condut = self.convert_condut(raw_condut)
        waterLevel = self.convert_water_level(raw_water_level)

        return SensorData(
            temperature = temperature,
            ph = ph,
            conductivity = condut,
            waterLevel = waterLevel
        )