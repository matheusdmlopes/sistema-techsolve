using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;
using PIM.Desktop.Utils;

namespace PIM.Desktop.Forms
{
    public partial class ChamadoDetalhesForm : Form
    {
        private int idChamado;
        private ChamadoDetalhadoDto chamado;

        // Controles de exibição
        private Label lblIdChamado;
        private Label lblTitulo;
        private TextBox txtDescricao;
        private Label lblCategoria;
        private Label lblPrioridade;
        private Label lblStatus;
        private Label lblUsuarioCriador;
        private Label lblTecnico;
        private Label lblDataAbertura;
        private Label lblDataFechamento;
        private TextBox txtSolucaoFinal;
        private DataGridView dgvHistorico;

        // Controles administrativos
        private Panel panelAcoes;
        private ComboBox cboNovoStatus;
        private TextBox txtObservacao;
        private Button btnAtualizarStatus;
        private ComboBox cboTecnico;
        private Button btnAtribuirTecnico;
        private Button btnRefresh;

        private List<StatusResponseDto> todosStatus;
        private List<UsuarioListDto> todosTecnicos;

        public ChamadoDetalhesForm(int idChamado)
        {
            this.idChamado = idChamado;
            InitializeComponent();
            InitializeCustomComponents();
            CarregarDados();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Name = "ChamadoDetalhesForm";
            this.Text = "Detalhes do Chamado";
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Título da página
            Label lblTituloPagina = new Label
            {
                Text = "Detalhes do Chamado",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTituloPagina);

            // Botão Atualizar
            btnRefresh = new Button
            {
                Text = "🔄 Atualizar",
                Location = new Point(860, 20),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await CarregarDados();
            this.Controls.Add(btnRefresh);

            // Painel de informações do chamado
            Panel panelInfo = new Panel
            {
                Location = new Point(20, 65),
                Size = new Size(940, 320),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            int yPos = 15;

            // ID e Título
            Label lblIdLabel = new Label
            {
                Text = "ID:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblIdLabel);

            lblIdChamado = new Label
            {
                Text = "-",
                Location = new Point(120, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelInfo.Controls.Add(lblIdChamado);

            Label lblTituloLabel = new Label
            {
                Text = "Título:",
                Location = new Point(240, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblTituloLabel);

            lblTitulo = new Label
            {
                Text = "-",
                Location = new Point(300, yPos),
                Size = new Size(600, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219)
            };
            panelInfo.Controls.Add(lblTitulo);

            yPos += 35;

            // Descrição
            Label lblDescricaoLabel = new Label
            {
                Text = "Descrição:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblDescricaoLabel);

            txtDescricao = new TextBox
            {
                Location = new Point(15, yPos + 25),
                Size = new Size(900, 60),
                Font = new Font("Segoe UI", 9),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            panelInfo.Controls.Add(txtDescricao);

            yPos += 100;

            // Categoria, Prioridade, Status
            Label lblCategoriaLabel = new Label
            {
                Text = "Categoria:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblCategoriaLabel);

            lblCategoria = new Label
            {
                Text = "-",
                Location = new Point(120, yPos),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelInfo.Controls.Add(lblCategoria);

            Label lblPrioridadeLabel = new Label
            {
                Text = "Prioridade:",
                Location = new Point(340, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblPrioridadeLabel);

            lblPrioridade = new Label
            {
                Text = "-",
                Location = new Point(430, yPos),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelInfo.Controls.Add(lblPrioridade);

            Label lblStatusLabel = new Label
            {
                Text = "Status:",
                Location = new Point(650, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblStatusLabel);

            lblStatus = new Label
            {
                Text = "-",
                Location = new Point(710, yPos),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96)
            };
            panelInfo.Controls.Add(lblStatus);

            yPos += 35;

            // Usuário Criador e Técnico
            Label lblCriadorLabel = new Label
            {
                Text = "Criado por:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblCriadorLabel);

            lblUsuarioCriador = new Label
            {
                Text = "-",
                Location = new Point(120, yPos),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelInfo.Controls.Add(lblUsuarioCriador);

            Label lblTecnicoLabel = new Label
            {
                Text = "Técnico:",
                Location = new Point(540, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblTecnicoLabel);

            lblTecnico = new Label
            {
                Text = "-",
                Location = new Point(620, yPos),
                Size = new Size(280, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelInfo.Controls.Add(lblTecnico);

            yPos += 35;

            // Datas
            Label lblDataAberturaLabel = new Label
            {
                Text = "Data Abertura:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblDataAberturaLabel);

            lblDataAbertura = new Label
            {
                Text = "-",
                Location = new Point(120, yPos),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelInfo.Controls.Add(lblDataAbertura);

            Label lblDataFechamentoLabel = new Label
            {
                Text = "Data Fechamento:",
                Location = new Point(340, yPos),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblDataFechamentoLabel);

            lblDataFechamento = new Label
            {
                Text = "-",
                Location = new Point(470, yPos),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelInfo.Controls.Add(lblDataFechamento);

            yPos += 35;

            // Solução Final
            Label lblSolucaoLabel = new Label
            {
                Text = "Solução Final:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelInfo.Controls.Add(lblSolucaoLabel);

            txtSolucaoFinal = new TextBox
            {
                Location = new Point(15, yPos + 25),
                Size = new Size(900, 40),
                Font = new Font("Segoe UI", 9),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            panelInfo.Controls.Add(txtSolucaoFinal);

            this.Controls.Add(panelInfo);

            // Histórico
            Label lblHistoricoLabel = new Label
            {
                Text = "Histórico de Alterações",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 400),
                AutoSize = true
            };
            this.Controls.Add(lblHistoricoLabel);

            dgvHistorico = new DataGridView
            {
                Location = new Point(20, 430),
                Size = new Size(940, 150),
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

            dgvHistorico.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvHistorico.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistorico.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvHistorico.ColumnHeadersHeight = 30;
            dgvHistorico.EnableHeadersVisualStyles = false;

            dgvHistorico.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            this.Controls.Add(dgvHistorico);

            // Painel de Ações Administrativas
            panelAcoes = new Panel
            {
                Location = new Point(20, 595),
                Size = new Size(940, 100),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblAcoesLabel = new Label
            {
                Text = "Ações Administrativas",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            panelAcoes.Controls.Add(lblAcoesLabel);

            // Atualizar Status
            Label lblNovoStatus = new Label
            {
                Text = "Novo Status:",
                Location = new Point(10, 45),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 8)
            };
            panelAcoes.Controls.Add(lblNovoStatus);

            cboNovoStatus = new ComboBox
            {
                Location = new Point(100, 42),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8)
            };
            panelAcoes.Controls.Add(cboNovoStatus);

            Label lblObservacao = new Label
            {
                Text = "Observação:",
                Location = new Point(230, 45),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 8)
            };
            panelAcoes.Controls.Add(lblObservacao);

            txtObservacao = new TextBox
            {
                Location = new Point(320, 42),
                Size = new Size(250, 23),
                Font = new Font("Segoe UI", 8)
            };
            panelAcoes.Controls.Add(txtObservacao);

            btnAtualizarStatus = new Button
            {
                Text = "Atualizar Status",
                Location = new Point(580, 40),
                Size = new Size(120, 27),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAtualizarStatus.FlatAppearance.BorderSize = 0;
            btnAtualizarStatus.Click += BtnAtualizarStatus_Click;
            panelAcoes.Controls.Add(btnAtualizarStatus);

            // Atribuir Técnico
            Label lblAtribuirTecnico = new Label
            {
                Text = "Técnico:",
                Location = new Point(720, 45),
                Size = new Size(60, 20),
                Font = new Font("Segoe UI", 8)
            };
            panelAcoes.Controls.Add(lblAtribuirTecnico);

            cboTecnico = new ComboBox
            {
                Location = new Point(780, 42),
                Size = new Size(140, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8)
            };
            panelAcoes.Controls.Add(cboTecnico);

            btnAtribuirTecnico = new Button
            {
                Text = "Atribuir",
                Location = new Point(780, 68),
                Size = new Size(140, 25),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAtribuirTecnico.FlatAppearance.BorderSize = 0;
            btnAtribuirTecnico.Click += BtnAtribuirTecnico_Click;
            panelAcoes.Controls.Add(btnAtribuirTecnico);

            this.Controls.Add(panelAcoes);

            // Verificar se é admin/técnico para mostrar ações
            VerificarPermissoes();
        }

        private void VerificarPermissoes()
        {
            // Apenas Admin e Técnico podem fazer ações administrativas
            string perfil = SessionManager.PerfilAcesso;
            if (perfil != "Administrador" && perfil != "Técnico")
            {
                panelAcoes.Visible = false;
            }
        }

        private async Task CarregarDados()
        {
            try
            {
                btnRefresh.Enabled = false;
                btnRefresh.Text = "Carregando...";

                // Carregar detalhes do chamado
                chamado = await ApiService.GetAsync<ChamadoDetalhadoDto>($"/api/Chamados/{idChamado}");

                // Preencher campos
                lblIdChamado.Text = chamado.IdChamado.ToString();
                lblTitulo.Text = chamado.Titulo;
                txtDescricao.Text = chamado.Descricao;
                lblCategoria.Text = chamado.Categoria?.Nome ?? "N/A";
                lblPrioridade.Text = chamado.Prioridade?.Nivel ?? "N/A";
                lblStatus.Text = chamado.Status?.Nome ?? "N/A";
                lblUsuarioCriador.Text = chamado.UsuarioCriador?.Nome ?? "N/A";
                lblTecnico.Text = chamado.UsuarioTecnico?.Nome ?? "Não atribuído";
                lblDataAbertura.Text = chamado.DataAbertura.ToString("dd/MM/yyyy HH:mm");
                lblDataFechamento.Text = chamado.DataFechamento?.ToString("dd/MM/yyyy HH:mm") ?? "-";
                txtSolucaoFinal.Text = chamado.SolucaoFinal ?? "-";

                // Histórico
                if (chamado.Historicos != null && chamado.Historicos.Any())
                {
                    var historicoList = chamado.Historicos.Select(h => new
                    {
                        Data = h.DataMudanca.ToString("dd/MM/yyyy HH:mm"),
                        StatusAnterior = h.StatusAnterior ?? "Novo",
                        StatusNovo = h.StatusNovo,
                        Tecnico = h.Tecnico?.Nome ?? "Sistema",
                        Observacao = h.Observacao ?? "-"
                    }).ToList();

                    dgvHistorico.DataSource = historicoList;
                }

                // Carregar combos para ações administrativas
                if (panelAcoes.Visible)
                {
                    await CarregarCombosAdministrativos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar detalhes: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
                btnRefresh.Text = "🔄 Atualizar";
            }
        }

        private async Task CarregarCombosAdministrativos()
        {
            try
            {
                // Carregar status
                todosStatus = await ApiService.GetAsync<List<StatusResponseDto>>("/api/Status");
                cboNovoStatus.DisplayMember = "Nome";
                cboNovoStatus.ValueMember = "IdStatus";
                cboNovoStatus.DataSource = todosStatus;

                // Carregar técnicos (usuários com perfil Técnico ou Administrador)
                var usuariosApi = await ApiService.GetAsync<List<UsuarioApiResponse>>("/api/Usuarios");
                todosTecnicos = usuariosApi
                    .Where(u => u.Perfil?.Nome == "Técnico" || u.Perfil?.Nome == "Administrador")
                    .Select(u => new UsuarioListDto
                    {
                        IdUsuario = u.Id,
                        NomeCompleto = u.NomeCompleto
                    }).ToList();

                cboTecnico.DisplayMember = "NomeCompleto";
                cboTecnico.ValueMember = "IdUsuario";
                cboTecnico.DataSource = todosTecnicos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar combos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAtualizarStatus_Click(object sender, EventArgs e)
        {
            if (cboNovoStatus.SelectedValue == null)
            {
                MessageBox.Show("Selecione um status.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnAtualizarStatus.Enabled = false;
                btnAtualizarStatus.Text = "Salvando...";

                var request = new AtualizarStatusRequest
                {
                    IdStatus = (int)cboNovoStatus.SelectedValue,
                    Observacao = txtObservacao.Text.Trim()
                };

                await ApiService.PutAsync<AtualizarStatusRequest, object>($"/api/Chamados/{idChamado}/status", request);

                MessageBox.Show("Status atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtObservacao.Clear();
                await CarregarDados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar status: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAtualizarStatus.Enabled = true;
                btnAtualizarStatus.Text = "Atualizar Status";
            }
        }

        private async void BtnAtribuirTecnico_Click(object sender, EventArgs e)
        {
            if (cboTecnico.SelectedValue == null)
            {
                MessageBox.Show("Selecione um técnico.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnAtribuirTecnico.Enabled = false;
                btnAtribuirTecnico.Text = "Atribuindo...";

                var request = new AtribuirTecnicoRequest
                {
                    IdUsuarioTecnico = (int)cboTecnico.SelectedValue
                };

                await ApiService.PutAsync<AtribuirTecnicoRequest, object>($"/api/Chamados/{idChamado}/atribuir", request);

                MessageBox.Show("Técnico atribuído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atribuir técnico: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAtribuirTecnico.Enabled = true;
                btnAtribuirTecnico.Text = "Atribuir";
            }
        }
    }
}
