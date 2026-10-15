#ifndef SENSORES_H
#define SENSORES_H

#include <Arduino.h>
#include <DallasTemperature.h>

float lerCondut();
float lerPH();
float lerTemperatura(DallasTemperature &sensors);
bool lerNivelAgua();

#endif