using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;

namespace PIM.Desktop.Forms
{
    public partial class UsuariosListForm : Form
    {
        private DataGridView dgvUsuarios;
        private TextBox txtBusca;
        private ComboBox cboFiltroSetor;
        private ComboBox cboFiltroPerfil;
        private ComboBox cboFiltroStatus;
        private Button btnNovo;
        private Button btnEditar;
        private Button btnAtualizar;
        private List<UsuarioListDto> todosUsuarios;
        private List<SetorDto> setores;
        private List<PerfilDto> perfis;

        public UsuariosListForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.AutoScroll = true;

            // Título
            Label lblTitle = new Label
            {
                Text = "Gestão de Usuários",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(30, 10)
            };
            this.Controls.Add(lblTitle);

            // Painel de filtros
            Panel panelFiltros = new Panel
            {
                BackColor = Color.White,
                Location = new Point(30, 60),
                Size = new Size(900, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Campo de busca
            Label lblBusca = new Label
            {
                Text = "Buscar:",
                Location = new Point(15, 15),
                AutoSize = true
            };
            panelFiltros.Controls.Add(lblBusca);

            txtBusca = new TextBox
            {
                Location = new Point(15, 35),
                Size = new Size(250, 25)
            };
            txtBusca.TextChanged += (s, e) => FiltrarUsuarios();
            panelFiltros.Controls.Add(txtBusca);

            // Filtro Setor
            Label lblSetor = new Label
            {
                Text = "Setor:",
                Location = new Point(280, 15),
                AutoSize = true
            };
            panelFiltros.Controls.Add(lblSetor);

            cboFiltroSetor = new ComboBox
            {
                Location = new Point(280, 35),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboFiltroSetor.SelectedIndexChanged += (s, e) => FiltrarUsuarios();
            panelFiltros.Controls.Add(cboFiltroSetor);

            // Filtro Perfil
            Label lblPerfil = new Label
            {
                Text = "Perfil:",
                Location = new Point(445, 15),
                AutoSize = true
            };
            panelFiltros.Controls.Add(lblPerfil);

            cboFiltroPerfil = new ComboBox
            {
                Location = new Point(445, 35),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboFiltroPerfil.SelectedIndexChanged += (s, e) => FiltrarUsuarios();
            panelFiltros.Controls.Add(cboFiltroPerfil);

            // Filtro Status
            Label lblStatus = new Label
            {
                Text = "Status:",
                Location = new Point(610, 15),
                AutoSize = true
            };
            panelFiltros.Controls.Add(lblStatus);

            cboFiltroStatus = new ComboBox
            {
                Location = new Point(610, 35),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboFiltroStatus.Items.AddRange(new object[] { "Todos", "Ativos", "Inativos" });
            cboFiltroStatus.SelectedIndex = 0;
            cboFiltroStatus.SelectedIndexChanged += (s, e) => FiltrarUsuarios();
            panelFiltros.Controls.Add(cboFiltroStatus);

            // Botão Limpar Filtros
            Button btnLimpar = new Button
            {
                Text = "Limpar",
                Location = new Point(745, 35),
                Size = new Size(80, 25),
                Cursor = Cursors.Hand
            };
            btnLimpar.Click += (s, e) => LimparFiltros();
            panelFiltros.Controls.Add(btnLimpar);

            this.Controls.Add(panelFiltros);

            // Botões de ação
            btnNovo = new Button
            {
                Text = "Novo Usuário",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(30, 155),
                Cursor = Cursors.Hand
            };
            btnNovo.FlatAppearance.BorderSize = 0;
            btnNovo.Click += BtnNovo_Click;
            this.Controls.Add(btnNovo);

            btnEditar = new Button
            {
                Text = "Editar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(190, 155),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += BtnEditar_Click;
            this.Controls.Add(btnEditar);

            btnAtualizar = new Button
            {
                Text = "Atualizar",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(127, 140, 141),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(320, 155),
                Cursor = Cursors.Hand
            };
            btnAtualizar.FlatAppearance.BorderSize = 0;
            btnAtualizar.Click += (s, e) => LoadData();
            this.Controls.Add(btnAtualizar);

            // DataGridView
            dgvUsuarios = new DataGridView
            {
                Location = new Point(30, 210),
                Size = new Size(900, 350),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false
            };

            dgvUsuarios.SelectionChanged += (s, e) =>
            {
                btnEditar.Enabled = dgvUsuarios.SelectedRows.Count > 0;
            };

            dgvUsuarios.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    BtnEditar_Click(s, e);
            };

            this.Controls.Add(dgvUsuarios);
        }

        private async void LoadData()
        {
            try
            {
                btnAtualizar.Enabled = false;
                btnAtualizar.Text = "Carregando...";

                // Carregar usuários da API
                var usuariosApi = await ApiService.GetAsync<List<UsuarioApiResponse>>("/api/Usuarios");

                // Converter para UsuarioListDto
                todosUsuarios = usuariosApi.Select(u => new UsuarioListDto
                {
                    IdUsuario = u.Id,
                    NomeCompleto = u.NomeCompleto,
                    Email = u.Email,
                    Telefone = u.Telefone,
                    Setor = u.Setor?.Nome ?? "Sem setor",
                    PerfilAcesso = u.Perfil?.Nome ?? "Sem perfil",
                    Ativo = u.Ativo
                }).ToList();

                // Carregar setores para filtro
                var setoresApi = await ApiService.GetAsync<List<SetorResponseDto>>("/api/Setores");
                setores = setoresApi.Select(s => new SetorDto
                {
                    IdSetor = s.Id,
                    NomeSetor = s.Nome
                }).ToList();
                cboFiltroSetor.Items.Clear();
                cboFiltroSetor.Items.Add("Todos");
                cboFiltroSetor.Items.AddRange(setores.Select(s => s.NomeSetor).ToArray());
                cboFiltroSetor.SelectedIndex = 0;

                // Carregar perfis para filtro (lista estática - não há endpoint na API)
                perfis = new List<PerfilDto>
                {
                    new PerfilDto { IdPerfil = 1, NomePerfil = "Administrador" },
                    new PerfilDto { IdPerfil = 2, NomePerfil = "Técnico" },
                    new PerfilDto { IdPerfil = 3, NomePerfil = "Usuário" }
                };
                cboFiltroPerfil.Items.Clear();
                cboFiltroPerfil.Items.Add("Todos");
                cboFiltroPerfil.Items.AddRange(perfis.Select(p => p.NomePerfil).ToArray());
                cboFiltroPerfil.SelectedIndex = 0;

                FiltrarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAtualizar.Enabled = true;
                btnAtualizar.Text = "Atualizar";
            }
        }

        private void FiltrarUsuarios()
        {
            if (todosUsuarios == null)
                return;

            var usuariosFiltrados = todosUsuarios.AsEnumerable();

            // Filtro de busca
            if (!string.IsNullOrWhiteSpace(txtBusca.Text))
            {
                string busca = txtBusca.Text.ToLower();
                usuariosFiltrados = usuariosFiltrados.Where(u =>
                    u.NomeCompleto.ToLower().Contains(busca) ||
                    u.Email.ToLower().Contains(busca));
            }

            // Filtro de setor
            if (cboFiltroSetor.SelectedIndex > 0)
            {
                string setor = cboFiltroSetor.SelectedItem.ToString();
                usuariosFiltrados = usuariosFiltrados.Where(u => u.Setor == setor);
            }

            // Filtro de perfil
            if (cboFiltroPerfil.SelectedIndex > 0)
            {
                string perfil = cboFiltroPerfil.SelectedItem.ToString();
                usuariosFiltrados = usuariosFiltrados.Where(u => u.PerfilAcesso == perfil);
            }

            // Filtro de status
            if (cboFiltroStatus.SelectedIndex == 1) // Ativos
                usuariosFiltrados = usuariosFiltrados.Where(u => u.Ativo);
            else if (cboFiltroStatus.SelectedIndex == 2) // Inativos
                usuariosFiltrados = usuariosFiltrados.Where(u => !u.Ativo);

            PreencherGrid(usuariosFiltrados.ToList());
        }

        private void PreencherGrid(List<UsuarioListDto> usuarios)
        {
            dgvUsuarios.DataSource = null;
            dgvUsuarios.Columns.Clear();

            if (usuarios == null || usuarios.Count == 0)
            {
                return;
            }

            dgvUsuarios.DataSource = usuarios;

            // Configurar colunas
            dgvUsuarios.Columns["IdUsuario"].HeaderText = "ID";
            dgvUsuarios.Columns["IdUsuario"].Width = 50;
            dgvUsuarios.Columns["NomeCompleto"].HeaderText = "Nome Completo";
            dgvUsuarios.Columns["Email"].HeaderText = "E-mail";
            dgvUsuarios.Columns["Telefone"].HeaderText = "Telefone";
            dgvUsuarios.Columns["Telefone"].Width = 120;
            dgvUsuarios.Columns["Setor"].HeaderText = "Setor";
            dgvUsuarios.Columns["Setor"].Width = 120;
            dgvUsuarios.Columns["PerfilAcesso"].HeaderText = "Perfil";
            dgvUsuarios.Columns["PerfilAcesso"].Width = 120;
            dgvUsuarios.Columns["Ativo"].HeaderText = "Ativo";
            dgvUsuarios.Columns["Ativo"].Width = 60;

            // Estilizar coluna Ativo
            dgvUsuarios.Columns["Ativo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void LimparFiltros()
        {
            txtBusca.Clear();
            cboFiltroSetor.SelectedIndex = 0;
            cboFiltroPerfil.SelectedIndex = 0;
            cboFiltroStatus.SelectedIndex = 0;
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            UsuarioEditForm editForm = new UsuarioEditForm(setores, perfis);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData(); // Recarregar lista
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
                return;

            var usuario = (UsuarioListDto)dgvUsuarios.SelectedRows[0].DataBoundItem;
            UsuarioEditForm editForm = new UsuarioEditForm(setores, perfis, usuario.IdUsuario);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData(); // Recarregar lista
            }
        }
    }
}
