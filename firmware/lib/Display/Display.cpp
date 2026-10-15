#include "Display.h"

void inicializarDisplay(LiquidCrystal_I2C &display)
{
    display.init();
    display.backlight();
}

void mostrarMensagem(LiquidCrystal_I2C &display, String linha1, String linha2)
{
    display.clear();

    display.setCursor(0, 0);
    display.print(linha1);

    display.setCursor(0, 1);
    display.print(linha2);
}

void mostrarDados(LiquidCrystal_I2C &display, String linha1, float linha2)
{
    display.clear();

    display.setCursor(0, 0);
    display.print(linha1);

    display.setCursor(0, 1);
    display.print(linha2);
}