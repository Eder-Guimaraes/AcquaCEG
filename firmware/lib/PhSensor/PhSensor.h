#pragma once

#include <Arduino.h>

class PhSensor
{
public:
    static const uint8_t TOTAL_AMOSTRAS = 10;

    explicit PhSensor(uint8_t pino);

    void begin();

    void iniciarLeituras();

    void coletarAmostra();

    bool leiturasConcluidas();

    float calcularMediaADC();

private:
    uint8_t _pino;

    uint16_t _amostras[TOTAL_AMOSTRAS];

    uint8_t _indice;
    uint8_t _quantidadeAmostras;
};