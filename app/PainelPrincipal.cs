using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace app
{
    public class PainelPrincipal : Form
    {

        // Cores reutilizadas em varios lugares da interface
        private static readonly Color CorFundo = Color.FromArgb(12, 20, 35);
        private static readonly Color CorFundoPainel = Color.FromArgb(22, 38, 58);
        private static readonly Color CorSidebar = Color.FromArgb(8, 15, 28);

        private static readonly Color CorBotao = Color.FromArgb(45, 75, 110);
        private static readonly Color CorBotaoAtivo = Color.FromArgb(0, 174, 255);
        private static readonly Color CorTextoClaro = Color.FromArgb(240, 248, 255);

        // --- Sidebar (menu lateral) ---
        private Panel pnlSidebar;
        private Button btnNavMonitor;
        private Button btnNavHistorico;

        // --- Painel: Monitor (tela original, ao vivo) ---
        private Panel pnlMonitor;
        private Label lblNivelValor;
        private Label lblCondutValor;
        private Label lblTempValor;
        private Label lblPHValor;
        private Label lblStatus;
        private Button btnConectar;
        private Button btnPausar;
        private ListBox logConsole;

        // --- Painel: Historico ---
        private Panel pnlHistorico;
        private DataGridView dgvHistorico;
        private GraficoBarras graficoCondutividade;
        private Button btnAtualizarHistorico;
        private Label lblGraficoTitulo;

        // Servico de comunicacao
        private ArduinoService _arduino;
        private bool _sistemaPausado = false;
        private BancoDeDados _bancoDeDados;

        public PainelPrincipal()
        {
            ConfigurarEstiloJanela();
            CriarSidebar();
            CriarPainelMonitor();
            CriarPainelHistorico();
            MostrarPainel(pnlMonitor, btnNavMonitor);

            // Inicializa o servico (Ajuste a sua porta COM aqui!)
            _arduino = new ArduinoService("COM5", 9600);

            // Cria (ou abre, se ja existir) o arquivo acquaceg.db
            _bancoDeDados = new BancoDeDados();

            // Se inscreve no evento de dados recebidos
            _arduino.DadosRecebidos += AoReceberDadosDoArduino;
        }

        private void ConfigurarEstiloJanela()
        {
            this.Text = "AcquaCEG - Painel de Monitoramento";
            this.Size = new Size(820, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = CorFundo;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        // ==============================
        // Sidebar (menu lateral de navegacao)
        // ==============================

        private void CriarSidebar()
        {
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 110,
                BackColor = CorSidebar
            };

            var lblLogo = new Label
            {
                Text = "AquaCEG",
                ForeColor = CorTextoClaro,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 20),
                Size = new Size(110, 40)
            };

            btnNavMonitor = CriarBotaoNav("MONITOR", 90);
            btnNavHistorico = CriarBotaoNav("HISTORICO", 140);

            btnNavMonitor.Click += (s, e) => MostrarPainel(pnlMonitor, btnNavMonitor);
            btnNavHistorico.Click += (s, e) =>
            {
                MostrarPainel(pnlHistorico, btnNavHistorico);
                CarregarHistorico();
            };

            pnlSidebar.Controls.Add(lblLogo);
            pnlSidebar.Controls.Add(btnNavMonitor);
            pnlSidebar.Controls.Add(btnNavHistorico);

            this.Controls.Add(pnlSidebar);
        }

        private Button CriarBotaoNav(string texto, int y)
        {
            var botao = new Button
            {
                Text = texto,
                Size = new Size(110, 42),
                Location = new Point(0, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = CorSidebar,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            botao.FlatAppearance.BorderSize = 0;
            return botao;
        }

        // Troca qual painel de conteudo esta visivel e destaca o botao ativo
        private void MostrarPainel(Panel painelAlvo, Button botaoAtivo)
        {
            pnlMonitor.Visible = (painelAlvo == pnlMonitor);
            pnlHistorico.Visible = (painelAlvo == pnlHistorico);

            foreach (var botao in new[] { btnNavMonitor, btnNavHistorico })
            {
                bool ativo = (botao == botaoAtivo);
                botao.BackColor = ativo ? Color.FromArgb(30, 30, 30) : CorSidebar;
                botao.ForeColor = ativo ? CorBotaoAtivo : Color.Gainsboro;
            }
        }

        // ==============================
        // Painel: Monitor (ao vivo)
        // ==============================

        private void CriarPainelMonitor()
        {
            pnlMonitor = new Panel
            {
                Location = new Point(110, 0),
                Size = new Size(this.ClientSize.Width - 110, this.ClientSize.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = CorFundo
            };

            var lblTitulo = new Label
            {
                Text = "SENSOR MONITOR",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true
            };

            lblNivelValor = CriarLabelValor("NIVEL DE AGUA: --", 70);
            lblCondutValor = CriarLabelValor("CONDUTIVIDADE: -- uS/cm", 115);
            lblTempValor = CriarLabelValor("TEMPERATURA: -- C", 160);
            lblPHValor = CriarLabelValor("PH: --", 205);

            lblStatus = new Label
            {
                Text = "STATUS: DESCONECTADO",
                ForeColor = Color.OrangeRed,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 255),
                AutoSize = true
            };
            

            btnConectar = new Button
            {
                Text = "INICIAR CONEXAO",
                Size = new Size(200, 45),
                Location = new Point(30, 295),
                FlatStyle = FlatStyle.Flat,
                BackColor = CorBotao,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnConectar.FlatAppearance.BorderSize = 0;
            btnConectar.Click += (s, e) => AlternarConexao();

            btnPausar = new Button
            {
                Text = "PAUSAR SISTEMA",
                Size = new Size(200, 45),
                Location = new Point(250, 295),
                FlatStyle = FlatStyle.Flat,
                BackColor = CorBotao,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnPausar.FlatAppearance.BorderSize = 0;
            btnPausar.Click += (s, e) => AlternarPausa();

            logConsole = new ListBox
            {
                Location = new Point(30, 365),
                Size = new Size(420, 185),
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 9)
            };

            pnlMonitor.Controls.Add(lblTitulo);
            pnlMonitor.Controls.Add(lblNivelValor);
            pnlMonitor.Controls.Add(lblCondutValor);
            pnlMonitor.Controls.Add(lblTempValor);
            pnlMonitor.Controls.Add(lblPHValor);
            pnlMonitor.Controls.Add(lblStatus);
            pnlMonitor.Controls.Add(btnConectar);
            pnlMonitor.Controls.Add(btnPausar);
            pnlMonitor.Controls.Add(logConsole);

            this.Controls.Add(pnlMonitor);
        }

        private Label CriarLabelValor(string textoInicial, int y)
        {
            return new Label
            {
                Text = textoInicial,
                ForeColor = CorTextoClaro,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(30, y),
                AutoSize = true
            };
        }

        // ==============================
        // Painel: Historico
        // ==============================

        private void CriarPainelHistorico()
        {
            pnlHistorico = new Panel
            {
                Location = new Point(110, 0),
                Size = new Size(this.ClientSize.Width - 110, this.ClientSize.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = CorFundo,
                Visible = false
            };

            var lblTitulo = new Label
            {
                Text = "HISTORICO DE LEITURAS",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true
            };

            btnAtualizarHistorico = new Button
            {
                Text = "ATUALIZAR",
                Size = new Size(120, 32),
                Location = new Point(560, 16),
                FlatStyle = FlatStyle.Flat,
                BackColor = CorBotao,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAtualizarHistorico.FlatAppearance.BorderSize = 0;
            btnAtualizarHistorico.Click += (s, e) => CarregarHistorico();

            dgvHistorico = CriarTabelaHistorico();
            dgvHistorico.Location = new Point(30, 60);
            dgvHistorico.Size = new Size(650, 280);

            lblGraficoTitulo = new Label
            {
                Text = "CONDUTIVIDADE - ULTIMAS LEITURAS (vermelho = alerta, > 1500 uS/cm)",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(30, 355),
                AutoSize = true
            };

            graficoCondutividade = new GraficoBarras
            {
                Location = new Point(30, 380),
                Size = new Size(650, 170),
                LimiteAlerta = Alertas.CondutividadeMaxima
            };

            pnlHistorico.Controls.Add(lblTitulo);
            pnlHistorico.Controls.Add(btnAtualizarHistorico);
            pnlHistorico.Controls.Add(dgvHistorico);
            pnlHistorico.Controls.Add(lblGraficoTitulo);
            pnlHistorico.Controls.Add(graficoCondutividade);

            this.Controls.Add(pnlHistorico);
        }

        private DataGridView CriarTabelaHistorico()
        {
            var grid = new DataGridView
            {
                BackgroundColor = CorFundoPainel,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(50, 50, 50),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 9)
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.ColumnHeadersHeight = 32;

            grid.DefaultCellStyle.BackColor = CorFundoPainel;
            grid.DefaultCellStyle.ForeColor = Color.Gainsboro;
            grid.DefaultCellStyle.SelectionBackColor = CorTextoClaro;
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            grid.Columns.Add("Data", "Data/Hora");
            grid.Columns.Add("Nivel", "Nivel de Agua");
            grid.Columns.Add("Condutividade", "Condutividade");
            grid.Columns.Add("Temperatura", "Temperatura");
            grid.Columns.Add("PH", "pH");

            return grid;
        }

        // Busca as leituras salvas no SQLite e atualiza a tabela + o grafico
        private void CarregarHistorico()
        {
            if (_bancoDeDados == null)
                return;

            var leituras = _bancoDeDados.ObterUltimasLeituras(200);

            dgvHistorico.Rows.Clear();
            foreach (var l in leituras)
            {
                dgvHistorico.Rows.Add(
                    l.DataHora.ToString("dd/MM HH:mm:ss"),
                    l.NivelAgua != 0 ? "OK" : "SEM AGUA",
                    $"{l.Condutividade:F1} uS/cm",
                    $"{l.Temperatura:F1} C",
                    l.PH.HasValue ? $"{l.PH.Value:F2}" : "--"
                );
            }

            // O grafico mostra em ordem cronologica (mais antiga -> mais recente),
            // por isso inverte a lista (que veio mais nova primeiro).
            graficoCondutividade.Valores = leituras
                .Select(l => l.Condutividade)
                .Reverse()
                .ToList();
            graficoCondutividade.Invalidate();
        }

        // ==============================
        // Logica de conexao / eventos do Arduino (igual a versao anterior)
        // ==============================

        private void AlternarConexao()
        {
            try
            {
                _arduino.Conectar();
                lblStatus.Text = "STATUS: ONLINE";
                lblStatus.ForeColor = Color.LimeGreen;
                btnConectar.Text = "CONECTADO";
                btnConectar.Enabled = false;
                btnPausar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar: " + ex.Message);
            }
        }

        private void AlternarPausa()
        {
            _sistemaPausado = !_sistemaPausado;
            _arduino.EnviarComando(_sistemaPausado ? 'P' : 'R');

            if (_sistemaPausado)
            {
                btnPausar.Text = "RETOMAR SISTEMA";
                lblStatus.Text = "STATUS: PAUSADO";
                lblStatus.ForeColor = Color.Gray;
            }
            else
            {
                btnPausar.Text = "PAUSAR SISTEMA";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_arduino != null && _arduino.EstaConectado)
            {
                _arduino.Desconectar();
            }
            base.OnFormClosing(e);
        }

        private void AoReceberDadosDoArduino(string dado)
        {
            if (this.IsDisposed || !this.IsHandleCreated)
                return;

            try
            {
                this.Invoke(new Action(() =>
                {
                    string valorLimpo = dado.Trim();

                    logConsole.Items.Add($"[{DateTime.Now:HH:mm:ss}] -> {valorLimpo}");
                    logConsole.SelectedIndex = logConsole.Items.Count - 1;

                    if (valorLimpo == "STATUS:LIGADO")
                    {
                        _sistemaPausado = false;
                        btnPausar.Text = "PAUSAR SISTEMA";
                        lblStatus.Text = "STATUS: ONLINE";
                        lblStatus.ForeColor = Color.LimeGreen;
                        return;
                    }

                    if (valorLimpo == "STATUS:DESLIGADO")
                    {
                        _sistemaPausado = true;
                        btnPausar.Text = "RETOMAR SISTEMA";
                        lblStatus.Text = "STATUS: PAUSADO";
                        lblStatus.ForeColor = Color.Gray;
                        return;
                    }

                    if (!SensorData.TryParse(valorLimpo, out SensorData dados))
                        return;

                    _bancoDeDados.Salvar(dados);

                    lblNivelValor.Text = $"NIVEL DE AGUA: {(dados.NivelAgua != 0 ? "OK" : "SEM AGUA")}";
                    lblCondutValor.Text = $"CONDUTIVIDADE: {dados.Condutividade:F1} uS/cm";
                    lblTempValor.Text = $"TEMPERATURA: {dados.Temperatura:F1} C";
                    lblPHValor.Text = $"PH: {dados.PH:F2}";

                    var (ok, mensagem) = Alertas.Verificar(dados);
                    lblStatus.Text = "STATUS: " + mensagem;
                    lblStatus.ForeColor = ok ? Color.LimeGreen : Color.OrangeRed;

                    // Se a aba de Historico estiver aberta, mantem ela atualizada
                    // em tempo real tambem, sem precisar clicar em "Atualizar".
                    if (pnlHistorico.Visible)
                    {
                        CarregarHistorico();
                    }
                }));
            }
            catch (ObjectDisposedException)
            {
                // Janela foi fechada entre o IsDisposed check e o Invoke - ignora esta leitura
            }
        }
    }
}