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
    public partial class ChamadosListForm : Form
    {
        private DataGridView dgvChamados;
        private TextBox txtBusca;
        private ComboBox cboFiltroStatus;
        private ComboBox cboFiltroCategoria;
        private Button btnNovo;
        private Button btnVerDetalhes;
        private Button btnRefresh;
        private Label lblTotal;

        private List<ChamadoListDto> todosChamados;
        private List<StatusResponseDto> todosStatus;
        private List<CategoriaResponseDto> todasCategorias;

        public ChamadosListForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Name = "ChamadosListForm";
            this.Text = "Gerenciamento de Chamados";
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Título
            Label lblTitulo = new Label
            {
                Text = "Gerenciamento de Chamados",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            // Label Total
            lblTotal = new Label
            {
                Text = "Total: 0 chamados",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 55),
                AutoSize = true
            };
            this.Controls.Add(lblTotal);

            // Painel de Filtros
            Panel panelFiltros = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(1050, 60),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblBusca = new Label
            {
                Text = "Buscar:",
                Location = new Point(10, 20),
                AutoSize = true
            };
            panelFiltros.Controls.Add(lblBusca);

            txtBusca = new TextBox
            {
                Location = new Point(70, 17),
                Size = new Size(250, 23),
                Font = new Font("Segoe UI", 9)
            };
            txtBusca.TextChanged += (s, e) => FiltrarChamados();
            panelFiltros.Controls.Add(txtBusca);

            Label lblStatus = new Label
            {
                Text = "Status:",
                Location = new Point(340, 20),
                AutoSize = true
            };
            panelFiltros.Controls.Add(lblStatus);

            cboFiltroStatus = new ComboBox
            {
                Location = new Point(400, 17),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cboFiltroStatus.SelectedIndexChanged += (s, e) => FiltrarChamados();
            panelFiltros.Controls.Add(cboFiltroStatus);

            Label lblCategoria = new Label
            {
                Text = "Categoria:",
                Location = new Point(570, 20),
                AutoSize = true
            };
            panelFiltros.Controls.Add(lblCategoria);

            cboFiltroCategoria = new ComboBox
            {
                Location = new Point(650, 17),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cboFiltroCategoria.SelectedIndexChanged += (s, e) => FiltrarChamados();
            panelFiltros.Controls.Add(cboFiltroCategoria);

            btnRefresh = new Button
            {
                Text = "🔄 Atualizar",
                Location = new Point(820, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await LoadData();
            panelFiltros.Controls.Add(btnRefresh);

            this.Controls.Add(panelFiltros);

            // DataGridView
            dgvChamados = new DataGridView
            {
                Location = new Point(20, 170),
                Size = new Size(1050, 380),
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

            dgvChamados.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvChamados.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvChamados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvChamados.ColumnHeadersHeight = 35;
            dgvChamados.EnableHeadersVisualStyles = false;

            dgvChamados.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvChamados.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvChamados.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvChamados.DoubleClick += BtnVerDetalhes_Click;

            this.Controls.Add(dgvChamados);

            // Botões inferiores
            btnNovo = new Button
            {
                Text = "➕ Novo Chamado",
                Location = new Point(20, 565),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNovo.FlatAppearance.BorderSize = 0;
            btnNovo.Click += BtnNovo_Click;
            this.Controls.Add(btnNovo);

            btnVerDetalhes = new Button
            {
                Text = "👁 Ver Detalhes",
                Location = new Point(180, 565),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerDetalhes.FlatAppearance.BorderSize = 0;
            btnVerDetalhes.Click += BtnVerDetalhes_Click;
            this.Controls.Add(btnVerDetalhes);
        }

        private async Task LoadData()
        {
            try
            {
                btnRefresh.Enabled = false;
                btnRefresh.Text = "Carregando...";

                // Carregar chamados, status e categorias em paralelo
                var chamadosTask = ApiService.GetAsync<List<ChamadoResponseDto>>("/api/Chamados");
                var statusTask = ApiService.GetAsync<List<StatusResponseDto>>("/api/Status");
                var categoriasTask = ApiService.GetAsync<List<CategoriaResponseDto>>("/api/Categorias");

                await Task.WhenAll(chamadosTask, statusTask, categoriasTask);

                var chamadosApi = await chamadosTask;
                todosStatus = await statusTask;
                todasCategorias = await categoriasTask;

                // Converter de ChamadoResponseDto (aninhado) para ChamadoListDto (flat)
                todosChamados = chamadosApi.Select(c => new ChamadoListDto
                {
                    IdChamado = c.IdChamado,
                    Titulo = c.Titulo,
                    Categoria = c.Categoria?.Nome ?? "N/A",
                    Prioridade = c.Prioridade?.Nivel ?? "N/A",
                    Status = c.Status?.Nome ?? "N/A",
                    UsuarioCriador = c.UsuarioCriador?.Nome ?? "N/A",
                    Tecnico = c.UsuarioTecnico?.Nome ?? "Não atribuído",
                    DataAbertura = c.DataAbertura,
                    DataFechamento = c.DataFechamento
                }).ToList();

                // Preencher combos de filtro
                cboFiltroStatus.Items.Clear();
                cboFiltroStatus.Items.Add("Todos");
                foreach (var status in todosStatus)
                {
                    cboFiltroStatus.Items.Add(status.Nome);
                }
                cboFiltroStatus.SelectedIndex = 0;

                cboFiltroCategoria.Items.Clear();
                cboFiltroCategoria.Items.Add("Todas");
                foreach (var categoria in todasCategorias)
                {
                    cboFiltroCategoria.Items.Add(categoria.Nome);
                }
                cboFiltroCategoria.SelectedIndex = 0;

                FiltrarChamados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar chamados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
                btnRefresh.Text = "🔄 Atualizar";
            }
        }

        private void FiltrarChamados()
        {
            if (todosChamados == null) return;

            var chamadosFiltrados = todosChamados.AsEnumerable();

            // Filtro de busca por título
            if (!string.IsNullOrWhiteSpace(txtBusca.Text))
            {
                string busca = txtBusca.Text.ToLower();
                chamadosFiltrados = chamadosFiltrados.Where(c =>
                    c.Titulo.ToLower().Contains(busca) ||
                    c.IdChamado.ToString().Contains(busca)
                );
            }

            // Filtro por Status
            if (cboFiltroStatus.SelectedIndex > 0)
            {
                string statusSelecionado = cboFiltroStatus.SelectedItem.ToString();
                chamadosFiltrados = chamadosFiltrados.Where(c => c.Status == statusSelecionado);
            }

            // Filtro por Categoria
            if (cboFiltroCategoria.SelectedIndex > 0)
            {
                string categoriaSelecionada = cboFiltroCategoria.SelectedItem.ToString();
                chamadosFiltrados = chamadosFiltrados.Where(c => c.Categoria == categoriaSelecionada);
            }

            var lista = chamadosFiltrados.ToList();
            dgvChamados.DataSource = lista;

            lblTotal.Text = $"Total: {lista.Count} chamado(s)";

            if (dgvChamados.Columns.Count > 0)
            {
                dgvChamados.Columns["IdChamado"].HeaderText = "ID";
                dgvChamados.Columns["IdChamado"].Width = 60;
                dgvChamados.Columns["Titulo"].HeaderText = "Título";
                dgvChamados.Columns["Titulo"].Width = 200;
                dgvChamados.Columns["Categoria"].HeaderText = "Categoria";
                dgvChamados.Columns["Categoria"].Width = 120;
                dgvChamados.Columns["Prioridade"].HeaderText = "Prioridade";
                dgvChamados.Columns["Prioridade"].Width = 100;
                dgvChamados.Columns["Status"].HeaderText = "Status";
                dgvChamados.Columns["Status"].Width = 120;
                dgvChamados.Columns["UsuarioCriador"].HeaderText = "Criado Por";
                dgvChamados.Columns["UsuarioCriador"].Width = 150;
                dgvChamados.Columns["Tecnico"].HeaderText = "Técnico";
                dgvChamados.Columns["Tecnico"].Width = 150;
                dgvChamados.Columns["DataAbertura"].HeaderText = "Data Abertura";
                dgvChamados.Columns["DataAbertura"].Width = 130;
                dgvChamados.Columns["DataAbertura"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dgvChamados.Columns["DataFechamento"].HeaderText = "Data Fechamento";
                dgvChamados.Columns["DataFechamento"].Width = 130;
                dgvChamados.Columns["DataFechamento"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dgvChamados.Columns["DataFechamento"].DefaultCellStyle.NullValue = "-";
            }
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            var form = new ChamadoEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnVerDetalhes_Click(object sender, EventArgs e)
        {
            if (dgvChamados.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um chamado para ver os detalhes.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var chamadoSelecionado = (ChamadoListDto)dgvChamados.SelectedRows[0].DataBoundItem;
            var form = new ChamadoDetalhesForm(chamadoSelecionado.IdChamado);
            form.ShowDialog();
            LoadData(); // Recarregar após fechar detalhes (pode ter havido alterações)
        }
    }
}
