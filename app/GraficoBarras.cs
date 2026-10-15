using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace app
{
    // Grafico de barras simples, desenhado a mao (GDI+), sem depender
    // de nenhum pacote NuGet externo (System.Windows.Forms.DataVisualization
    // nao vem pronto no .NET moderno e adicionar pacote so ia criar mais
    // um ponto de falha de instalacao).
    public class GraficoBarras : Control
    {
        public List<float> Valores { get; set; } = new List<float>();

        // Valores >= este limite sao desenhados em vermelho (alerta).
        // Deixe float.MaxValue se nao quiser destacar nenhum valor.
        public float LimiteAlerta { get; set; } = float.MaxValue;

        // Largura fixa de cada barra e espaco entre elas, em pixels.
        public int LarguraBarra { get; set; } = 14;
        public int EspacoBarras { get; set; } = 4;

        public GraficoBarras()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(24, 24, 24);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Valores == null || Valores.Count == 0)
            {
                using (var fonteVazio = new Font("Segoe UI", 9))
                using (var brushVazio = new SolidBrush(Color.Gray))
                {
                    g.DrawString("Sem dados para exibir ainda", fonteVazio, brushVazio, 10, 10);
                }
                return;
            }

            int passo = LarguraBarra + EspacoBarras;
            int maxVisiveis = Math.Max(Width / passo, 1);
            var visiveis = Valores.Count > maxVisiveis
                ? Valores.Skip(Valores.Count - maxVisiveis).ToList()
                : Valores;

            float max = Math.Max(visiveis.Max(), 0.1f);
            float min = Math.Min(visiveis.Min(), 0f);
            float faixa = Math.Max(max - min, 0.1f);

            const int margemInferior = 22;
            const int margemSuperior = 12;
            int areaUtil = Math.Max(Height - margemInferior - margemSuperior, 10);

            int qtd = visiveis.Count;


            using (var brushNormal = new SolidBrush(Color.FromArgb(0, 255, 127)))
            using (var brushAlerta = new SolidBrush(Color.FromArgb(255, 87, 34)))
            using (var penBase = new Pen(Color.FromArgb(60, 60, 60)))
            {
                for (int i = 0; i < qtd; i++)
                {
                    float valor = visiveis[i];
                    float alturaBarra = ((valor - min) / faixa) * areaUtil;
                    float x = i * passo + (EspacoBarras / 2f);
                    float y = margemSuperior + (areaUtil - alturaBarra);
                    float larguraReal = LarguraBarra;

                    var brush = valor >= LimiteAlerta ? brushAlerta : brushNormal;
                    g.FillRectangle(brush, x, y, larguraReal, alturaBarra);
                }

                g.DrawLine(penBase, 0, margemSuperior + areaUtil, Width, margemSuperior + areaUtil);
            }
        }
    }
}