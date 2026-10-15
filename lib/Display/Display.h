#ifndef DISPLAY_H
#define DISPLAY_H

#include <Arduino.h>
#include <LiquidCrystal_I2C.h>

void inicializarDisplay(LiquidCrystal_I2C &display);
void mostrarMensagem(LiquidCrystal_I2C &display, String linha1, String linha2);
void mostrarDados(LiquidCrystal_I2C &display, String linha1, float linha2);

#endif