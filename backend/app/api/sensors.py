from fastapi import APIRouter

router = APIRouter()

@router.get("/latest")
def read_latest():
    return {
                "temperature": 25.4,
                "ph": 7.02,
                "conductivity": 910,
                "waterLevel": True
            }