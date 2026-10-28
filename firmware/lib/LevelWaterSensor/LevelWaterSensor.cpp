#include "LevelWaterSensor.h"

SensorNivelAgua::SensorNivelAgua(uint8_t pino, uint8_t nivelComAgua)
    : _pino(pino), _nivelComAgua(nivelComAgua) {}

void SensorNivelAgua::begin() {
    pinMode(_pino, INPUT_PULLDOWN);   // evita o pino flutuando em 1
}

bool SensorNivelAgua::temAgua() const {
    return digitalRead(_pino) == _nivelComAgua;
}