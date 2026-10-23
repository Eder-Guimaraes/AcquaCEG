from app.schemas.sensor import SensorData, SensorReadingOut
from app.models.sensor_reading import SensorReading
from app.repositories.sensor_repository import SensorRepository


class SensorService:

    def __init__(self, session_factory):
        # Recebe a FÁBRICA de sessões (SessionLocal), não uma sessão pronta.
        # Cada operação abre e fecha a sua própria sessão, então a thread do
        # MQTT e as threads das requisições HTTP nunca compartilham a mesma.
        self.session_factory = session_factory

    # ---------- conversões (inalteradas) ----------

    def convert_temperature(self, temperature):
        return temperature / 100

    def convert_ph(self, adc):
        # esses valores podem variar um pouco enquanto ainda houver ruido elétrico
        m = 0
        b = 0

        return m * adc + b

    def convert_condut(self, adc):
        # esses valores podem variar um pouco enquanto ainda houver ruido elétrico
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

    # ---------- conversão ORM -> schema ----------

    def _to_schema(self, reading: SensorReading) -> SensorData:
        # só os valores (usado em /latest)
        return SensorData(
            temperature=reading.temperature,
            ph=reading.ph,
            conductivity=reading.conductivity,
            waterLevel=reading.water_level
        )

    def _to_out(self, reading: SensorReading) -> SensorReadingOut:
        # valores + id + created_at (usado em /readings e /readings/{id})
        return SensorReadingOut.model_validate(reading)

    # ---------- operações (uma sessão por chamada) ----------

    def save_reading(self, raw_data):
        temperature, ph, condut, water_level = self._convert_raw_data(raw_data)

        reading = SensorReading(
            ph=ph,
            conductivity=condut,
            temperature=temperature,
            water_level=water_level
        )

        with self.session_factory() as db:
            return SensorRepository(db).save(reading)

    def get_latest(self):

        with self.session_factory() as db:
            reading = SensorRepository(db).get_latest()

            if reading is None:
                return None

            # converter DENTRO do with: depois que a sessão fecha,
            # o objeto ORM fica desanexado
            return self._to_schema(reading)

    def get_all(self, page: int, limit: int):

        with self.session_factory() as db:
            readings = SensorRepository(db).get_all(page, limit)

            return [self._to_out(r) for r in readings]

    def get_by_id(self, reading_id: int):

        with self.session_factory() as db:
            reading = SensorRepository(db).get_by_id(reading_id)

            if reading is None:
                return None

            return self._to_out(reading)

    def delete_reading(self, reading_id: int):

        with self.session_factory() as db:
            return SensorRepository(db).delete(reading_id)