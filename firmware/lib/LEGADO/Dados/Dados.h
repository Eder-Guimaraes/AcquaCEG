#ifndef DADOS_H
#define DADOS_H

#include <Arduino.h>
#include <LiquidCrystal_I2C.h>

void dadosSistema(LiquidCrystal_I2C &display, float condutividade, float temperatura, float ph);

#endif