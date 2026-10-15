using System;
using System.IO.Ports;

namespace app
{
    public class ArduinoService
    {
        private SerialPort _porta;
        public event Action<string> DadosRecebidos;

        public ArduinoService(string portaCom, int baudRate)
        {
            _porta = new SerialPort(portaCom, baudRate);

            // Configura o evento disparado pelo hardware
            _porta.DataReceived += (s, e) =>
            {
                try
                {
                    string dados = _porta.ReadLine();
                    DadosRecebidos?.Invoke(dados);
                }
                catch (TimeoutException)
                {
                    // Linha incompleta chegou antes do timeout, ignora e espera a próxima
                }
                catch (Exception)
                {
                    // Outro erro de leitura pontual (porta fechando, etc.) - ignora esta leitura
                }
            };
        }

        public void Conectar() => _porta.Open();
        public void Desconectar() => _porta.Close();
        public bool EstaConectado => _porta.IsOpen;

        // Manda um unico caractere de comando pro Arduino (ex: 'P' pausa, 'R' retoma)
        public void EnviarComando(char comando)
        {
            if (_porta.IsOpen)
                _porta.Write(comando.ToString());
        }
    }
}