using System.Globalization;

namespace app
{
    // Representa uma leitura recebida do Arduino.
    // Formato esperado (definido em main.cpp): "DATA:nivelAgua,condutividade,temperatura,ph"
    public class SensorData
    {
        public int NivelAgua { get; set; }
        public float Condutividade { get; set; }
        public float Temperatura { get; set; }
        public float PH { get; set; }

        public static bool TryParse(string linha, out SensorData dados)
        {
            dados = null;

            if (string.IsNullOrWhiteSpace(linha))
                return false;

            const string prefixo = "DATA:";
            if (!linha.StartsWith(prefixo))
                return false;

            string[] partes = linha.Substring(prefixo.Length).Split(',');
            if (partes.Length != 4)
                return false;

            bool ok = true;
            ok &= int.TryParse(partes[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int nivel);
            ok &= float.TryParse(partes[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float condutividade);
            ok &= float.TryParse(partes[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float temperatura);
            ok &= float.TryParse(partes[3], NumberStyles.Any, CultureInfo.InvariantCulture, out float ph);

            if (!ok)
                return false;

            dados = new SensorData
            {
                NivelAgua = nivel,
                Condutividade = condutividade,
                Temperatura = temperatura,
                PH = ph
            };
            return true;
        }
    }
}