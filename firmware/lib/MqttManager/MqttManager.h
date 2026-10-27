#ifndef MQTT_MANAGER_H
#define MQTT_MANAGER_H

#include <Arduino.h>
#include <PubSubClient.h>
#include <WiFiS3.h>
#include "Config.h"

class MqttManager
{
public:
    MqttManager(Client& wifiClient);

    void begin();

    bool connect(const char* clientId);

    void reconnect(const char* clientId);

    bool isConnected();

    void loop();

    bool publish(const char* payload);

private:
    const char* _host = MQTT_HOST;
    const uint16_t _port = MQTT_PORT;
    const char* _topic = MQTT_TOPIC;

    PubSubClient _client;
};

#endif