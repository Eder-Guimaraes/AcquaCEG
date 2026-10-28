#pragma once
#include <Arduino.h>

class SensorNivelAgua {
public:
    // nivelComAgua: HIGH ou LOW, conforme o seu sensor sinaliza "tem água"
    explicit SensorNivelAgua(uint8_t pino, uint8_t nivelComAgua = HIGH);

    void begin();
    bool temAgua() const;

private:
    uint8_t _pino;
    uint8_t _nivelComAgua;
};