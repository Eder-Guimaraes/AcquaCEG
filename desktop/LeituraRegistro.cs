using System;

namespace app
{
    // Representa uma linha salva na tabela "leituras" do SQLite,
    // usada para popular a tela de Historico.
    public class LeituraRegistro
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public int NivelAgua { get; set; }
        public float Condutividade { get; set; }
        public float Temperatura { get; set; }
        public float? PH { get; set; }
    }
}