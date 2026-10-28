#ifndef ALERTAS_H
#define ALERTAS_H

#include <Arduino.h>
#include <LiquidCrystal_I2C.h>

bool verificarSistema(LiquidCrystal_I2C &display, bool nivelAgua,
                      float condutividade, float temperatura, float ph);

#endif