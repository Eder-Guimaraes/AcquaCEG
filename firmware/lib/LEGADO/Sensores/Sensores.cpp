#include <Arduino.h>
#include "Sensores.h"
#include "Config.h"

const int N_AMOSTRAS_ADC = 10;

// Media de N leituras do ADC (unidades de ADC, 0-1023).
// mCondut/bCondut e mPH/bPH foram calibrados em ADC, NAO em volts.
static float lerMediaADC(int pino)
{
    analogRead(pino); // descarta a 1a leitura (o ADC e compartilhado entre canais)
    delay(5);

    long soma = 0; // long: int tem 16 bits no Uno
    for (int i = 0; i < N_AMOSTRAS_ADC; i++)
    {
        soma += analogRead(pino);
        delay(10);
    }
    return soma / (float)N_AMOSTRAS_ADC;
}

float lerCondut()
{
    // uS/cm = mCondut * ADC + bCondut
    return mCondut * lerMediaADC(SENSOR_CONDUTIVIDADE) + bCondut;
}

float lerPH()
{
    // pH = mPH * ADC + bPH
     float ph = mPH * lerMediaADC(SENSOR_PH) + bPH;
     return ph - 3.0;
}

// Sensor de nivel nao-invasivo (grudado por fora do recipiente).
// Retorna true se detectou agua, false se nao detectou.
// OBS: se o seu modulo for ativo em LOW (alguns sao), inverta para "== LOW".
bool lerNivelAgua()
{
    return digitalRead(SENSOR_AGUA) == HIGH;
}

float lerTemperatura(DallasTemperature &sensors)
{
    sensors.requestTemperatures();

    // A biblioteca DallasTemperature retorna -127.0 (DEVICE_DISCONNECTED_C)
    // quando o sensor nao responde (cabo solto, sensor com defeito, etc).
    // Esse valor e tratado como caso de erro em Alertas.cpp, entao aqui so
    // repassamos a leitura crua sem mexer.
    return sensors.getTempCByIndex(0);
}