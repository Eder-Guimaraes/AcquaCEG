#include "WifiManager.h"
#include <Arduino.h>

void WifiManager::begin()
{
    Serial.println("Inicializando Wi-Fi...");
}

bool WifiManager::connect(const char* ssid, const char* password)
{
    _ssid = ssid;
    _password = password;

    Serial.print("Conectando em ");
    Serial.println(_ssid);

    WiFi.begin(_ssid, _password);

    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        Serial.print(".");
    }

    Serial.println();
    Serial.println("Wi-Fi conectado!");
    Serial.println(WiFi.localIP());

    return true;
}

bool WifiManager::isConnected()
{
    return WiFi.status() == WL_CONNECTED;
}

void WifiManager::reconnect()
{
    static unsigned long ultimaTentativa = 0;

    if (isConnected())
        return;

    if (millis() - ultimaTentativa < 5000)
        return;

    ultimaTentativa = millis();

    Serial.println("Reconectando...");

    WiFi.begin(_ssid, _password);
}

WiFiClient& WifiManager::getClient()
{
    return _client;
}