#include <Arduino.h>
#include "Config.h"
#include "Sensores.h"
#include "Display.h"
#include "Alertas.h"
#include "Dados.h"
#include "Controle.h"

// ==============================
// Bibliotecas
// ==============================

#include <LiquidCrystal_I2C.h>
#include <OneWire.h>
#include <DallasTemperature.h>

// ==============================
// Objetos
// ==============================

LiquidCrystal_I2C display(0x27, 16, 2);

OneWire oneWire(SENSOR_TEMPERATURA);
DallasTemperature sensors(&oneWire);

// Tempo (ms) que a tela de alerta/status fica visivel antes de passar
// para as telas de dados (pH/condutividade/temperatura).
#define TEMPO_ALERTA_MS 2000

// ==============================
// Setup
// ==============================

void setup()
{
  Serial.begin(9600);

  inicializarDisplay(display);
  inicializarControle();
  sensors.begin();

  pinMode(BUZZER, OUTPUT);
  pinMode(LED_BUZZER, OUTPUT);

  pinMode(SENSOR_AGUA, INPUT);
  pinMode(SENSOR_PH, INPUT);
  pinMode(SENSOR_CONDUTIVIDADE, INPUT);

  delay(50); // pequeno atraso para a porta serial estabilizar
  Serial.println(sistemaLigado() ? "STATUS:LIGADO" : "STATUS:DESLIGADO");
}

// ==============================
// Loop
// ==============================

void loop()
{
  // Confere se chegou algum comando do painel no PC
  if (Serial.available() > 0)
  {
    char comando = Serial.read();

    if (comando == 'P' && sistemaLigado())
    {
      definirSistemaLigado(false);
    }
    else if (comando == 'R' && !sistemaLigado())
    {
      definirSistemaLigado(true);
    }
  }

  // Detecta transicao de estado (ligado <-> desligado) vinda do comando
  // serial e trata os efeitos colaterais (silenciar alarme, avisar no
  // display) uma unica vez.
  static bool ligadoAnterior = true;
  bool ligado = sistemaLigado();

  if (ligado != ligadoAnterior)
  {
    if (!ligado)
    {
      noTone(BUZZER);
      digitalWrite(LED_BUZZER, LOW);
      mostrarMensagem(display, "SISTEMA", "DESLIGADO");
    }
    ligadoAnterior = ligado;
  }

  if (!ligado)
  {
    // Sistema desligado: nao le sensores, nao manda DATA, so espera
    // o comando 'R'.
    delay(200);
    return;
  }

  // Obter os dados
  bool nivelAgua = lerNivelAgua();
  float condutividade = lerCondut();
  float temperatura = lerTemperatura(sensors);
  float ph = lerPH();

  Serial.print("DATA:");
  Serial.print(nivelAgua ? 1 : 0);
  Serial.print(",");
  Serial.print(condutividade);
  Serial.print(",");
  Serial.print(temperatura);
  Serial.print(",");
  Serial.println(ph);

  // Verificar (mostra alerta ou "sistema normal" no display)
  bool sistemaOK = verificarSistema(display, nivelAgua, condutividade, temperatura, ph);

  delay(TEMPO_ALERTA_MS);

  if (sistemaOK)
  {
    dadosSistema(display, condutividade, temperatura, ph);
  }
}