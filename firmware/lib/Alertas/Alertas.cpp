#include "Alertas.h"
#include "Config.h"
#include "Display.h"
#include "Controle.h"

// ==============================
// Funcoes de verificacao (privadas do modulo)
// ==============================

// A lib DallasTemperature retorna exatamente -127.0 quando o sensor
// nao responde (cabo solto, sensor com defeito, etc). Precisa ser
// verificado ANTES de temperaturaBaixa(), senao -127 seria lido como
// "agua fria".
bool sensorTemperaturaDesconectado(float temperatura)
{
    return temperatura == -127.0;
}

bool temperaturaAlta(float temperatura)
{
    return temperatura >= 40;
}

bool temperaturaBaixa(float temperatura)
{
    return temperatura <= 5;
}

bool condutividadeAlta(float condutividade)
{
    return condutividade > 1500;
}

// pH fora da faixa aceitavel definida em Config.h (PH_MIN / PH_MAX).
bool phForaDaFaixa(float ph)
{
    return ph < PH_MIN || ph > PH_MAX;
}

// Sensor de nivel agora e digital (0 ou 1): so diz se tem agua ou nao,
// nao existe mais faixa "baixo/alto" como no sensor analogico antigo.
bool semAgua(bool nivelAgua)
{
    return !nivelAgua;
}

// ==============================
// Funcoes de acao (privadas do modulo)
// ==============================

void dispararAlerta(LiquidCrystal_I2C &display, String linha1, String linha2, int frequencia)
{
    mostrarMensagem(display, linha1, linha2);
    digitalWrite(LED_BUZZER, HIGH);
    tone(BUZZER, frequencia);
    definirAlerta(true);
}

void sistemaNormal(LiquidCrystal_I2C &display)
{
    noTone(BUZZER);
    digitalWrite(LED_BUZZER, LOW);
    mostrarMensagem(display, "OK", "SISTEMA OPERANDO");
    definirAlerta(false);
}

// ==============================
// Funcao publica (a unica declarada no .h)
// ==============================

bool verificarSistema(LiquidCrystal_I2C &display, bool nivelAgua,
                      float condutividade, float temperatura, float ph)
{
    if (semAgua(nivelAgua))
    {
        dispararAlerta(display, "ALERTA", "SEM AGUA", 1300);
        return false;
    }
    else if (sensorTemperaturaDesconectado(temperatura))
    {
        dispararAlerta(display, "ERRO SENSOR", "TEMPERATURA", 900);
        return false;
    }
    else if (temperaturaAlta(temperatura))
    {
        dispararAlerta(display, "ALERTA", "AGUA QUENTE", 1200);
        return false;
    }
    else if (temperaturaBaixa(temperatura))
    {
        dispararAlerta(display, "ALERTA", "AGUA FRIA", 1200);
        return false;
    }
    else if (condutividadeAlta(condutividade))
    {
        dispararAlerta(display, "ALERTA", "CONDUT ALTA", 1000);
        return false;
    }
    else if (phForaDaFaixa(ph))
    {
        dispararAlerta(display, "ALERTA", "PH FORA FAIXA", 1100);
        return false;
    }
    else
    {
        sistemaNormal(display);
        return true;
    }
}