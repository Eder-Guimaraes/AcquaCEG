#include "PhSensor.h"

PhSensor::PhSensor(uint8_t pino)
{
    _pino = pino;
    _indice = 0;
    _quantidadeAmostras = 0;
}

void PhSensor::begin()
{
    pinMode(_pino, INPUT);
}

void PhSensor::iniciarLeituras()
{
    _indice = 0;
    _quantidadeAmostras = 0;
}

void PhSensor::coletarAmostra()
{
    _amostras[_indice] = analogRead(_pino);

    _indice++;

    if (_indice >= TOTAL_AMOSTRAS)
        _indice = 0;

    if (_quantidadeAmostras < TOTAL_AMOSTRAS)
        _quantidadeAmostras++;
}

bool PhSensor::leiturasConcluidas()
{
    return _quantidadeAmostras == TOTAL_AMOSTRAS;
}

float PhSensor::calcularMediaADC()
{
    if (!leiturasConcluidas())
        return NAN;

    uint32_t soma = 0;

    uint16_t menor = _amostras[0];
    uint16_t maior = _amostras[0];

    for (int i = 0; i < TOTAL_AMOSTRAS; i++)
    {
        uint16_t valor = _amostras[i];

        soma += valor;

        if (valor < menor)
            menor = valor;

        if (valor > maior)
            maior = valor;
    }

    return (float)(soma - menor - maior) / (TOTAL_AMOSTRAS - 2);
}