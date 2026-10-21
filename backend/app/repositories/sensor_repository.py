from sqlalchemy.orm import Session

from app.models.sensor_reading import SensorReading


class SensorRepository:

    def __init__(self, db: Session):
        self.db = db

    def save(self, reading: SensorReading):

        self.db.add(reading)
        self.db.commit()
        self.db.refresh(reading)

        return reading

    def get_latest(self):
        return (
            self.db.query(SensorReading)
            .order_by(SensorReading.created_at.desc())
            .first()
        )

    def get_all(self):
        return self.db.query(SensorReading).all()