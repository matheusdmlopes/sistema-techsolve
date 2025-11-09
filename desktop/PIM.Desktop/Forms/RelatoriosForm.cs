using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;

namespace PIM.Desktop.Forms
{
    public partial class RelatoriosForm : Form
    {
        private TabControl tabRelatorios;

        // Tab Métricas Gerais
        private Panel panelMetricas;
        private Label lblTotalChamados;
        private Label lblChamadosAbertos;
        private Label lblChamadosEmAndamento;
        private Label lblChamadosResolvidos;
        private Label lblTecnicosAtivos;
        private Label lblTempoMedio;
        private Label lblSatisfacaoMedia;
        private DataGridView dgvPorPrioridade;
        private DataGridView dgvTopCategorias;
        private Button btnRefreshMetricas;

        // Tab Satisfação
        private Panel panelSatisfacao;
        private Label lblTotalFeedbacks;
        private Label lblNotaMediaSatisfacao;
        private Label lblPercentualSatisfacao;
        private DataGridView dgvDistribuicaoNotas;
        private Button btnRefreshSatisfacao;

        // Tab Feedbacks
        private Panel panelFeedbacks;
        private DataGridView dgvFeedbacks;
        private ComboBox cboFiltroNota;
        private Button btnRefreshFeedbacks;

        public RelatoriosForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            CarregarDados();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Name = "RelatoriosForm";
            this.Text = "Relatórios";
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Título
            Label lblTitulo = new Label
            {
                Text = "Relatórios e Estatísticas",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 10),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            // TabControl
            tabRelatorios = new TabControl
            {
                Location = new Point(20, 50),
                Size = new Size(1050, 615),
                Font = new Font("Segoe UI", 9)
            };

            // Criar abas
            CreateMetricasTab();
            CreateSatisfacaoTab();
            CreateFeedbacksTab();

            this.Controls.Add(tabRelatorios);
        }

        private void CreateMetricasTab()
        {
            TabPage tabMetricas = new TabPage("Métricas Gerais");
            panelMetricas = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(236, 240, 241)
            };

            int yPos = 15;

            // Botão Atualizar
            btnRefreshMetricas = new Button
            {
                Text = "Atualizar",
                Location = new Point(920, 10),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefreshMetricas.FlatAppearance.BorderSize = 0;
            btnRefreshMetricas.Click += async (s, e) => await CarregarMetricas();
            panelMetricas.Controls.Add(btnRefreshMetricas);

            // Cards de estatísticas
            Panel panelCards = new Panel
            {
                Location = new Point(15, yPos),
                Size = new Size(1000, 180),
                BackColor = Color.Transparent
            };

            // Card 1 - Total de Chamados
            var card1 = CreateStatCard("Total de Chamados", "0", Color.FromArgb(52, 152, 219), 0, 0);
            lblTotalChamados = (Label)card1.Controls[1];
            panelCards.Controls.Add(card1);

            // Card 2 - Abertos
            var card2 = CreateStatCard("Abertos", "0", Color.FromArgb(231, 76, 60), 255, 0);
            lblChamadosAbertos = (Label)card2.Controls[1];
            panelCards.Controls.Add(card2);

            // Card 3 - Em Andamento
            var card3 = CreateStatCard("Em Andamento", "0", Color.FromArgb(241, 196, 15), 510, 0);
            lblChamadosEmAndamento = (Label)card3.Controls[1];
            panelCards.Controls.Add(card3);

            // Card 4 - Resolvidos
            var card4 = CreateStatCard("Resolvidos", "0", Color.FromArgb(46, 204, 113), 765, 0);
            lblChamadosResolvidos = (Label)card4.Controls[1];
            panelCards.Controls.Add(card4);

            // Card 5 - Técnicos Ativos
            var card5 = CreateStatCard("Técnicos Ativos", "0", Color.FromArgb(155, 89, 182), 0, 95);
            lblTecnicosAtivos = (Label)card5.Controls[1];
            panelCards.Controls.Add(card5);

            // Card 6 - Tempo Médio
            var card6 = CreateStatCard("Tempo Médio Resolução", "-", Color.FromArgb(52, 73, 94), 255, 95);
            lblTempoMedio = (Label)card6.Controls[1];
            panelCards.Controls.Add(card6);

            // Card 7 - Satisfação Média
            var card7 = CreateStatCard("Satisfação Média", "0.0", Color.FromArgb(230, 126, 34), 510, 95);
            lblSatisfacaoMedia = (Label)card7.Controls[1];
            panelCards.Controls.Add(card7);

            panelMetricas.Controls.Add(panelCards);
            yPos += 195;

            // Seção Distribuição
            Label lblDistribuicao = new Label
            {
                Text = "Distribuição de Chamados",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, yPos),
                AutoSize = true
            };
            panelMetricas.Controls.Add(lblDistribuicao);
            yPos += 35;

            // Grid Por Prioridade
            Label lblPrioridade = new Label
            {
                Text = "Por Prioridade:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, yPos),
                AutoSize = true
            };
            panelMetricas.Controls.Add(lblPrioridade);
            yPos += 25;

            dgvPorPrioridade = new DataGridView
            {
                Location = new Point(15, yPos),
                Size = new Size(480, 150),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };
            dgvPorPrioridade.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvPorPrioridade.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPorPrioridade.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvPorPrioridade.EnableHeadersVisualStyles = false;
            panelMetricas.Controls.Add(dgvPorPrioridade);

            // Grid Top Categorias
            Label lblCategorias = new Label
            {
                Text = "Top Categorias:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(520, yPos - 25),
                AutoSize = true
            };
            panelMetricas.Controls.Add(lblCategorias);

            dgvTopCategorias = new DataGridView
            {
                Location = new Point(520, yPos),
                Size = new Size(500, 150),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };
            dgvTopCategorias.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvTopCategorias.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTopCategorias.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvTopCategorias.EnableHeadersVisualStyles = false;
            panelMetricas.Controls.Add(dgvTopCategorias);

            tabMetricas.Controls.Add(panelMetricas);
            tabRelatorios.TabPages.Add(tabMetricas);
        }

        private void CreateSatisfacaoTab()
        {
            TabPage tabSatisfacao = new TabPage("Satisfação");
            panelSatisfacao = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(236, 240, 241)
            };

            int yPos = 15;

            // Botão Atualizar
            btnRefreshSatisfacao = new Button
            {
                Text = "Atualizar",
                Location = new Point(920, 10),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefreshSatisfacao.FlatAppearance.BorderSize = 0;
            btnRefreshSatisfacao.Click += async (s, e) => await CarregarSatisfacao();
            panelSatisfacao.Controls.Add(btnRefreshSatisfacao);

            // Cards de satisfação
            Panel panelCardsSat = new Panel
            {
                Location = new Point(15, yPos),
                Size = new Size(1000, 95),
                BackColor = Color.Transparent
            };

            var cardTotal = CreateStatCard("Total de Feedbacks", "0", Color.FromArgb(52, 152, 219), 0, 0);
            lblTotalFeedbacks = (Label)cardTotal.Controls[1];
            panelCardsSat.Controls.Add(cardTotal);

            var cardNota = CreateStatCard("Nota Média", "0.0", Color.FromArgb(46, 204, 113), 255, 0);
            lblNotaMediaSatisfacao = (Label)cardNota.Controls[1];
            panelCardsSat.Controls.Add(cardNota);

            var cardPercentual = CreateStatCard("% Satisfação", "0%", Color.FromArgb(230, 126, 34), 510, 0);
            lblPercentualSatisfacao = (Label)cardPercentual.Controls[1];
            panelCardsSat.Controls.Add(cardPercentual);

            panelSatisfacao.Controls.Add(panelCardsSat);
            yPos += 110;

            // Grid Distribuição de Notas
            Label lblDistNotas = new Label
            {
                Text = "Distribuição de Notas",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, yPos),
                AutoSize = true
            };
            panelSatisfacao.Controls.Add(lblDistNotas);
            yPos += 35;

            dgvDistribuicaoNotas = new DataGridView
            {
                Location = new Point(15, yPos),
                Size = new Size(1000, 200),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };
            dgvDistribuicaoNotas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvDistribuicaoNotas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDistribuicaoNotas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvDistribuicaoNotas.EnableHeadersVisualStyles = false;
            panelSatisfacao.Controls.Add(dgvDistribuicaoNotas);

            tabSatisfacao.Controls.Add(panelSatisfacao);
            tabRelatorios.TabPages.Add(tabSatisfacao);
        }

        private void CreateFeedbacksTab()
        {
            TabPage tabFeedbacks = new TabPage("Feedbacks");
            panelFeedbacks = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(236, 240, 241)
            };

            // Filtros
            Panel panelFiltrosFeed = new Panel
            {
                Location = new Point(15, 15),
                Size = new Size(1000, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblFiltroNota = new Label
            {
                Text = "Nota Mínima:",
                Location = new Point(10, 17),
                AutoSize = true
            };
            panelFiltrosFeed.Controls.Add(lblFiltroNota);

            cboFiltroNota = new ComboBox
            {
                Location = new Point(100, 14),
                Size = new Size(100, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboFiltroNota.Items.AddRange(new object[] { "Todas", "1", "2", "3", "4", "5" });
            cboFiltroNota.SelectedIndex = 0;
            panelFiltrosFeed.Controls.Add(cboFiltroNota);

            btnRefreshFeedbacks = new Button
            {
                Text = "Atualizar",
                Location = new Point(220, 12),
                Size = new Size(100, 28),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefreshFeedbacks.FlatAppearance.BorderSize = 0;
            btnRefreshFeedbacks.Click += async (s, e) => await CarregarFeedbacks();
            panelFiltrosFeed.Controls.Add(btnRefreshFeedbacks);

            panelFeedbacks.Controls.Add(panelFiltrosFeed);

            // Grid Feedbacks
            dgvFeedbacks = new DataGridView
            {
                Location = new Point(15, 80),
                Size = new Size(1000, 470),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };
            dgvFeedbacks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvFeedbacks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvFeedbacks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvFeedbacks.ColumnHeadersHeight = 35;
            dgvFeedbacks.EnableHeadersVisualStyles = false;
            dgvFeedbacks.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            panelFeedbacks.Controls.Add(dgvFeedbacks);

            tabFeedbacks.Controls.Add(panelFeedbacks);
            tabRelatorios.TabPages.Add(tabFeedbacks);
        }

        private Panel CreateStatCard(string titulo, string valor, Color cor, int x, int y)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(240, 85),
                BackColor = cor
            };

            Label lblTitulo = new Label
            {
                Text = titulo,
                Location = new Point(10, 10),
                Size = new Size(220, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            card.Controls.Add(lblTitulo);

            Label lblValor = new Label
            {
                Text = valor,
                Location = new Point(10, 35),
                Size = new Size(220, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblValor);

            return card;
        }

        private async void CarregarDados()
        {
            await CarregarMetricas();
            await CarregarSatisfacao();
            await CarregarFeedbacks();
        }

        private async Task CarregarMetricas()
        {
            try
            {
                btnRefreshMetricas.Enabled = false;
                btnRefreshMetricas.Text = "Carregando...";

                var metricas = await ApiService.GetAsync<MetricasDashboard>("/api/Dashboard/metricas");

                // Atualizar cards
                lblTotalChamados.Text = metricas.ResumoGeral.TotalChamados.ToString();
                lblChamadosAbertos.Text = metricas.ResumoGeral.ChamadosAbertos.ToString();
                lblChamadosEmAndamento.Text = metricas.ResumoGeral.ChamadosEmAndamento.ToString();
                lblChamadosResolvidos.Text = metricas.ResumoGeral.ChamadosResolvidos.ToString();
                lblTecnicosAtivos.Text = metricas.ResumoGeral.TecnicosAtivos.ToString();
                lblTempoMedio.Text = metricas.ResumoGeral.TempoMedioResolucao?.Formatado ?? "-";
                lblSatisfacaoMedia.Text = metricas.ResumoGeral.SatisfacaoMedia.ToString("F2");

                // Grid por prioridade
                if (metricas.Distribuicao?.PorPrioridade != null)
                {
                    dgvPorPrioridade.DataSource = metricas.Distribuicao.PorPrioridade;
                    if (dgvPorPrioridade.Columns.Count > 0)
                    {
                        dgvPorPrioridade.Columns["Prioridade"].HeaderText = "Prioridade";
                        dgvPorPrioridade.Columns["Total"].HeaderText = "Total";
                    }
                }

                // Grid top categorias
                if (metricas.Distribuicao?.TopCategorias != null)
                {
                    dgvTopCategorias.DataSource = metricas.Distribuicao.TopCategorias;
                    if (dgvTopCategorias.Columns.Count > 0)
                    {
                        dgvTopCategorias.Columns["Categoria"].HeaderText = "Categoria";
                        dgvTopCategorias.Columns["Total"].HeaderText = "Total";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar métricas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefreshMetricas.Enabled = true;
                btnRefreshMetricas.Text = "Atualizar";
            }
        }

        private async Task CarregarSatisfacao()
        {
            try
            {
                btnRefreshSatisfacao.Enabled = false;
                btnRefreshSatisfacao.Text = "Carregando...";

                var relatorio = await ApiService.GetAsync<RelatorioSatisfacao>("/api/feedbacks/relatorio");

                lblTotalFeedbacks.Text = relatorio.TotalFeedbacks.ToString();
                lblNotaMediaSatisfacao.Text = relatorio.NotaMedia.ToString("F2");
                lblPercentualSatisfacao.Text = relatorio.PercentualSatisfacao.ToString("F1") + "%";

                // Grid distribuição
                if (relatorio.DistribuicaoNotas != null)
                {
                    var distribuicao = relatorio.DistribuicaoNotas.Select(d => new
                    {
                        Nota = d.Key + " ★",
                        Quantidade = d.Value,
                        Percentual = relatorio.TotalFeedbacks > 0
                            ? ((d.Value / (double)relatorio.TotalFeedbacks) * 100).ToString("F1") + "%"
                            : "0%"
                    }).ToList();

                    dgvDistribuicaoNotas.DataSource = distribuicao;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar satisfação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefreshSatisfacao.Enabled = true;
                btnRefreshSatisfacao.Text = "Atualizar";
            }
        }

        private async Task CarregarFeedbacks()
        {
            try
            {
                btnRefreshFeedbacks.Enabled = false;
                btnRefreshFeedbacks.Text = "Carregando...";

                string endpoint = "/api/feedbacks";
                if (cboFiltroNota.SelectedIndex > 0)
                {
                    endpoint += $"?notaMinima={cboFiltroNota.SelectedItem}";
                }

                var feedbacks = await ApiService.GetAsync<List<FeedbackListDto>>(endpoint);

                dgvFeedbacks.DataSource = feedbacks;

                if (dgvFeedbacks.Columns.Count > 0)
                {
                    dgvFeedbacks.Columns["Id"].HeaderText = "ID";
                    dgvFeedbacks.Columns["Id"].Width = 50;
                    dgvFeedbacks.Columns["IdChamado"].HeaderText = "Chamado";
                    dgvFeedbacks.Columns["IdChamado"].Width = 80;
                    dgvFeedbacks.Columns["TituloChamado"].HeaderText = "Título";
                    dgvFeedbacks.Columns["Nota"].HeaderText = "Nota";
                    dgvFeedbacks.Columns["Nota"].Width = 60;
                    dgvFeedbacks.Columns["Comentario"].HeaderText = "Comentário";
                    dgvFeedbacks.Columns["DataHoraFeedback"].HeaderText = "Data";
                    dgvFeedbacks.Columns["DataHoraFeedback"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dgvFeedbacks.Columns["Solicitante"].HeaderText = "Solicitante";
                    dgvFeedbacks.Columns["Tecnico"].HeaderText = "Técnico";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar feedbacks: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefreshFeedbacks.Enabled = true;
                btnRefreshFeedbacks.Text = "Atualizar";
            }
        }
    }
}
