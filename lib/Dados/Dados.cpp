#include "Dados.h"
#include "Alertas.h"
#include "Config.h"
#include "Display.h"

#define TEMPO_EXIBICAO_MS 1500

void dadosSistema(LiquidCrystal_I2C &display, float condutividade, float temperatura, float ph)
{
    mostrarDados(display, "Condutividade:", condutividade);
    delay(TEMPO_EXIBICAO_MS);

    mostrarDados(display, "Temperatura:", temperatura);
    delay(TEMPO_EXIBICAO_MS);

    mostrarDados(display, "pH:", ph);
    delay(TEMPO_EXIBICAO_MS);
}