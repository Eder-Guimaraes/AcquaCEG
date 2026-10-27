#include "MqttManager.h"

MqttManager::MqttManager(Client& wifiClient)
    : _client(wifiClient)
{
}

void MqttManager::begin()
{
    _client.setServer(_host, _port);
}

bool MqttManager::connect(const char* clientId)
{
    Serial.println("Conectando ao broker MQTT...");

    if (_client.connect(clientId))
    {
        Serial.println("Broker conectado.");
        return true;
    }

    Serial.print("Erro MQTT: ");
    Serial.println(_client.state());

    return false;
}

bool MqttManager::isConnected()
{
    return _client.connected();
}

void MqttManager::reconnect(const char* clientId)
{
    static unsigned long ultimaTentativa = 0;

    if (_client.connected())
        return;

    if (millis() - ultimaTentativa < 5000)
        return;

    ultimaTentativa = millis();

    connect(clientId);
}

void MqttManager::loop()
{
    _client.loop();
}

bool MqttManager::publish(const char* payload)
{
    if (!client.connected())
        return;
        
    return _client.publish(_topic, payload);
}