using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace app
{
    class BancoDeDados
    {
        private readonly string _connectionString;


        public BancoDeDados(string nomeArquivo = "acquaceg.db")
        {
            string caminhoCompleto = Path.Combine(AppContext.BaseDirectory, nomeArquivo);

            _connectionString = $"Data Source={caminhoCompleto}";
            CriarTabelaSeNaoExistir();
        }


        private void CriarTabelaSeNaoExistir()
        {
            using var conexao = new SqliteConnection(_connectionString);
            conexao.Open();

            var comando = conexao.CreateCommand();
            comando.CommandText =
                @"CREATE TABLE IF NOT EXISTS leituras (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DataHora TEXT NOT NULL,
                    NivelAgua INTEGER NOT NULL,
                    Condutividade REAL NOT NULL,
                    Temperatura REAL NOT NULL,
                    PH REAL
                )";

            comando.ExecuteNonQuery();

            AdicionarColunaPhSeNaoExistir(conexao);
        }

        // Bancos criados antes do sensor de pH nao tem a coluna PH.
        // A coluna e nullable de proposito: leituras antigas ficam com NULL
        // (mostradas como "--") em vez de um 0 falso.
        private void AdicionarColunaPhSeNaoExistir(SqliteConnection conexao)
        {
            bool existe = false;

            var consulta = conexao.CreateCommand();
            consulta.CommandText = "PRAGMA table_info(leituras)";
            using (var leitor = consulta.ExecuteReader())
            {
                while (leitor.Read())
                {
                    // Coluna 1 do PRAGMA table_info = nome da coluna
                    if (string.Equals(leitor.GetString(1), "PH", StringComparison.OrdinalIgnoreCase))
                    {
                        existe = true;
                        break;
                    }
                }
            }

            if (!existe)
            {
                var alterar = conexao.CreateCommand();
                alterar.CommandText = "ALTER TABLE leituras ADD COLUMN PH REAL";
                alterar.ExecuteNonQuery();
            }
        }

        public void Salvar(SensorData dados)
        {
            using var conexao = new SqliteConnection(_connectionString);
            conexao.Open();
 
            var comando = conexao.CreateCommand();
            comando.CommandText =
                @"INSERT INTO leituras (DataHora, NivelAgua, Condutividade, Temperatura, PH)
                  VALUES ($dataHora, $nivel, $condutividade, $temperatura, $ph)";
 
            comando.Parameters.AddWithValue("$dataHora", DateTime.Now.ToString("s"));
            comando.Parameters.AddWithValue("$nivel", dados.NivelAgua);
            comando.Parameters.AddWithValue("$condutividade", dados.Condutividade);
            comando.Parameters.AddWithValue("$temperatura", dados.Temperatura);
            comando.Parameters.AddWithValue("$ph", dados.PH);
 
            comando.ExecuteNonQuery();
        }

        // Busca as leituras mais recentes salvas no banco, mais nova primeiro.
        // Usado pela tela de Historico para popular a tabela e o grafico.
        public List<LeituraRegistro> ObterUltimasLeituras(int quantidade = 200)
        {
            var resultado = new List<LeituraRegistro>();

            using var conexao = new SqliteConnection(_connectionString);
            conexao.Open();

            var comando = conexao.CreateCommand();
            comando.CommandText =
                @"SELECT Id, DataHora, NivelAgua, Condutividade, Temperatura, PH
                  FROM leituras
                  ORDER BY Id DESC
                  LIMIT $quantidade";
            comando.Parameters.AddWithValue("$quantidade", quantidade);

            using var leitor = comando.ExecuteReader();
            while (leitor.Read())
            {
                resultado.Add(new LeituraRegistro
                {
                    Id = leitor.GetInt32(0),
                    DataHora = DateTime.Parse(leitor.GetString(1)),
                    NivelAgua = leitor.GetInt32(2),
                    Condutividade = (float)leitor.GetDouble(3),
                    Temperatura = (float)leitor.GetDouble(4),
                    PH = leitor.IsDBNull(5) ? (float?)null : (float)leitor.GetDouble(5)
                });
            }

            return resultado;
        }
    }
}