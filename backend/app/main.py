from fastapi import FastAPI

from app.api.sensors import router

app = FastAPI()

app.include_router(
    router,
    prefix="/api/sensors",
    tags=["Sensors"]
)

@app.get("/")
def read_root():
    return {"status": "AcquaCEG Backend Online"}