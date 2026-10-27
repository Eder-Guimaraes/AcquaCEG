#include "TemperatureSensor.h"

SensorTemperatura::SensorTemperatura(uint8_t pino)
    : _barramento(pino), _sensor(&_barramento) {}

void SensorTemperatura::begin() {
    _sensor.begin();
    _sensor.setWaitForConversion(false);   // não bloqueante
}

void SensorTemperatura::solicitarLeitura() {
    _sensor.requestTemperatures();
}

float SensorTemperatura::lerGraus() {
    float graus = _sensor.getTempCByIndex(0);

    // -127 = sensor desconectado; 85 = valor de fábrica (conversão não terminou)
    if (graus == DEVICE_DISCONNECTED_C || graus == 85.0f) return NAN;
    return graus;
}