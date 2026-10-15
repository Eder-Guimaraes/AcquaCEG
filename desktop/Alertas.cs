namespace app
{
    // Espelha os limites definidos em Config.h e a lógica de Alertas.cpp,
    // para o painel no PC mostrar o mesmo status que o LCD do Arduino mostraria.
    public static class Alertas
    {
        // Mesmos limites usados em Alertas.cpp (temperaturaAlta/Baixa, condutividadeAlta)
        public const float TemperaturaMaxima = 40f;
        public const float TemperaturaMinima = 5f;
        public const float CondutividadeMaxima = 1500f;
        public const float PhMinimo = 5.5f;   // espelha PH_MIN em Config.h
        public const float PhMaximo = 9.5f;   // espelha PH_MAX em Config.h

        // Valor que a lib DallasTemperature manda quando o sensor DS18B20
        // esta desconectado (ver lerTemperatura() em Sensores.cpp)
        public const float TemperaturaSensorDesconectado = -127f;

        // Retorna se o sistema está OK e a mensagem correspondente,
        // seguindo a mesma ordem de prioridade de verificarSistema() no firmware.
        public static (bool ok, string mensagem) Verificar(SensorData dados)
        {
            // Sensor de nivel agora e digital (0 = sem agua, 1 = com agua)
            if (dados.NivelAgua == 0)
                return (false, "SEM AGUA");

            if (dados.Temperatura == TemperaturaSensorDesconectado)
                return (false, "ERRO SENSOR TEMP");

            if (dados.Temperatura >= TemperaturaMaxima)
                return (false, "AGUA QUENTE");

            if (dados.Temperatura <= TemperaturaMinima)
                return (false, "AGUA FRIA");

            if (dados.Condutividade > CondutividadeMaxima)
                return (false, "CONDUT ALTA");

            if (dados.PH < PhMinimo || dados.PH > PhMaximo)
                return (false, "PH FORA FAIXA");

            return (true, "SISTEMA OPERANDO");
        }
    }
}