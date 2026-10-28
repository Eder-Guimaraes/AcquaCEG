#ifndef WIFI_MANAGER_H
#define WIFI_MANAGER_H

#include <WiFiS3.h>

class WifiManager
{
public:
    void begin();
    bool connect(const char* ssid, const char* password);
    bool isConnected();
    void reconnect();

    WiFiClient& getClient();

private:
    const char* _ssid;
    const char* _password;

    WiFiClient _client;
};

#endif