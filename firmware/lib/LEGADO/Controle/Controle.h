#ifndef CONTROLE_H
#define CONTROLE_H

#include <Arduino.h>

void inicializarControle();
bool sistemaLigado();
void definirSistemaLigado(bool ligado);
void definirAlerta(bool emAlerta);

#endif