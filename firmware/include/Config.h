#ifndef CONFIG_H
#define CONFIG_H

#define SENSOR_AGUA 3 // sensor de nivel nao-invasivo (digital: HIGH = agua presente)
#define SENSOR_PH A1
#define SENSOR_CONDUTIVIDADE A2
#define SENSOR_TEMPERATURA 4

#define LED_VERDE 7
#define LED_VERMELHO 8

#define BUZZER 11
#define LED_BUZZER 12

const double mCondut = 3.5;
const double bCondut = 0.0;

const double mPH = -0.02885;
const double bPH = 30.08;
const double PH_MIN = 5.5;
const double PH_MAX = 9.5;

#endif