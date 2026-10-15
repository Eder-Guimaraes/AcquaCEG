#include "Controle.h"
#include "Config.h"

namespace
{
    bool ligado = true;
    bool emAlerta = false;

    void atualizarLeds()
    {
        bool normal = ligado && !emAlerta;
        bool erro = ligado && emAlerta;
        digitalWrite(LED_VERDE, normal ? HIGH : LOW);
        digitalWrite(LED_VERMELHO, erro ? HIGH : LOW);
    }
}

void inicializarControle()
{
    pinMode(LED_VERDE, OUTPUT);
    pinMode(LED_VERMELHO, OUTPUT);
    atualizarLeds();
}

bool sistemaLigado()
{
    return ligado;
}

void definirSistemaLigado(bool valor)
{
    if (ligado != valor)
    {
        ligado = valor;
        atualizarLeds();
    }
}

void definirAlerta(bool valor)
{
    if (emAlerta != valor)
    {
        emAlerta = valor;
        atualizarLeds();
    }
}