#pragma once
#include <Arduino.h>
#include <OneWire.h>
#include <DallasTemperature.h>

class SensorTemperatura {
public:
    explicit SensorTemperatura(uint8_t pino);

    // proíbe cópia: o DallasTemperature guarda um ponteiro para o OneWire
    // desta instância, e uma cópia apontaria para o objeto errado
    SensorTemperatura(const SensorTemperatura&) = delete;
    SensorTemperatura& operator=(const SensorTemperatura&) = delete;

    void  begin();
    void  solicitarLeitura();   // manda converter, NÃO espera
    float lerGraus();           // NAN se o sensor falhou

private:
    OneWire           _barramento;
    DallasTemperature _sensor;
};