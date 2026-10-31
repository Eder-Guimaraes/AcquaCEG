#include <Arduino.h>

#include "Pins.h"
#include "Config.h"

#include <PhSensor.h>
#include <CondutSensor.h>
#include <TemperatureSensor.h>
#include <LevelWaterSensor.h>

#include <WifiManager.h>
#include <MqttManager.h>

// Sensores
PhSensor sensorPH(PH);
CondutSensor sensorCondutividade(CONDUCTIVITY);
SensorTemperatura sensorTemperatura(TEMPERATURE);
SensorNivelAgua sensorNivelAgua(WATER);

// Wi-Fi
WifiManager wifiManager;

// MQTT
MqttManager mqttManager(wifiManager.getClient());

// Configurações
const unsigned long TEMPO_TURNO = 5000;
const unsigned long INTERVALO_LEITURA = 100;
const uint8_t CICLOS_AQUECIMENTO = 2;

// Controle
bool turnoPH = true;

bool aquecimentoConcluido = false;
uint8_t ciclosConcluidos = 0;

unsigned long inicioTurno = 0;
unsigned long ultimaLeitura = 0;

float ultimoPH = NAN;
float ultimaCondutividade = NAN;

void setup()
{
    Serial.begin(115200);

    // Wi-Fi
    wifiManager.begin();
    wifiManager.connect(WIFI_SSID, WIFI_PASSWORD);

    // MQTT
    mqttManager.begin();
    mqttManager.connect(MQTT_CLIENT_ID, MQTT_USERNAME, MQTT_PASSWORD);

    // Sensores
    sensorPH.begin();
    sensorCondutividade.begin();
    sensorTemperatura.begin();
    sensorNivelAgua.begin();

    sensorTemperatura.solicitarLeitura();
    sensorPH.iniciarLeituras();

    inicioTurno = millis();

    Serial.println("Iniciando aquecimento...");
}

void loop()
{
    unsigned long agora = millis();

    // Mantém conexões
    wifiManager.reconnect();

    if (wifiManager.isConnected())
    {
        mqttManager.reconnect(MQTT_CLIENT_ID, MQTT_USERNAME, MQTT_PASSWORD);
        mqttManager.loop();
    }

    // Leitura dos sensores
    if (agora - ultimaLeitura >= INTERVALO_LEITURA)
    {
        ultimaLeitura = agora;

        if (turnoPH)
            sensorPH.coletarAmostra();
        else
            sensorCondutividade.coletarAmostra();
    }

    // Alternância entre sensores
    if (agora - inicioTurno >= TEMPO_TURNO)
    {
        inicioTurno = agora;

        if (turnoPH)
        {
            ultimoPH = sensorPH.calcularMediaADC();

            Serial.print("ADC pH: ");
            Serial.println(ultimoPH);

            sensorCondutividade.iniciarLeituras();
        }
        else
        {
            ultimaCondutividade = sensorCondutividade.calcularMediaADC();

            Serial.print("ADC Condutividade: ");
            Serial.println(ultimaCondutividade);

            float temperatura = sensorTemperatura.lerGraus();
            bool temAgua = sensorNivelAgua.temAgua();

            sensorTemperatura.solicitarLeitura();

            if (!aquecimentoConcluido)
            {
                ciclosConcluidos++;

                Serial.print("Aquecimento ");
                Serial.print(ciclosConcluidos);
                Serial.print("/");
                Serial.println(CICLOS_AQUECIMENTO);

                if (ciclosConcluidos >= CICLOS_AQUECIMENTO)
                {
                    aquecimentoConcluido = true;
                    Serial.println("Sistema pronto.");
                }
            }
            else
            {
                Serial.println("========== MQTT ==========");

                Serial.print("pH ADC: ");
                Serial.println(ultimoPH);

                Serial.print("Condutividade ADC: ");
                Serial.println(ultimaCondutividade);

                Serial.print("Temperatura: ");
                Serial.println(temperatura);

                Serial.print("Água: ");
                Serial.println(temAgua ? "SIM" : "NAO");

                // Monta o JSON
                String payload = "{";
                payload += "\"adc_ph\":" + String(ultimoPH, 0);
                payload += ",\"adc_condut\":" + String(ultimaCondutividade, 0);
                payload += ",\"temperature\":" + String(temperatura, 2);
                payload += ",\"water_level\":" + String(temAgua ? 1 : 0);
                payload += "}";

                // Exibe o JSON
                Serial.println(payload);

                // Publica no MQTT
                mqttManager.publish(payload.c_str());

                Serial.println("==========================");
            }

            sensorPH.iniciarLeituras();
        }

        turnoPH = !turnoPH;
    }
}