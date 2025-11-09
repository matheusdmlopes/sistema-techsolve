using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;

namespace PIM.Desktop.Forms
{
    public partial class SetoresListForm : Form
    {
        private DataGridView dgvSetores;
        private TextBox txtBusca;
        private Button btnNovo;
        private Button btnEditar;
        private Button btnExcluir;
        private Button btnAtualizar;
        private List<SetorResponseDto> todosSetores;

        public SetoresListForm()
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
                Text = "Gestão de Setores",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(30, 10)
            };
            this.Controls.Add(lblTitle);

            // Painel de busca
            Panel panelBusca = new Panel
            {
                BackColor = Color.White,
                Location = new Point(30, 60),
                Size = new Size(900, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblBusca = new Label
            {
                Text = "Buscar:",
                Location = new Point(15, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            panelBusca.Controls.Add(lblBusca);

            txtBusca = new TextBox
            {
                Location = new Point(15, 30),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtBusca.TextChanged += (s, e) => FiltrarSetores();
            panelBusca.Controls.Add(txtBusca);

            Button btnLimpar = new Button
            {
                Text = "Limpar",
                Location = new Point(425, 28),
                Size = new Size(80, 28),
                Cursor = Cursors.Hand
            };
            btnLimpar.Click += (s, e) => txtBusca.Clear();
            panelBusca.Controls.Add(btnLimpar);

            this.Controls.Add(panelBusca);

            // Botões de ação
            btnNovo = new Button
            {
                Text = "Novo Setor",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 40),
                Location = new Point(30, 135),
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
                Size = new Size(110, 40),
                Location = new Point(180, 135),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += BtnEditar_Click;
            this.Controls.Add(btnEditar);

            btnExcluir = new Button
            {
                Text = "Excluir",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(231, 76, 60),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 40),
                Location = new Point(300, 135),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnExcluir.FlatAppearance.BorderSize = 0;
            btnExcluir.Click += BtnExcluir_Click;
            this.Controls.Add(btnExcluir);

            btnAtualizar = new Button
            {
                Text = "Atualizar",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(127, 140, 141),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(420, 135),
                Cursor = Cursors.Hand
            };
            btnAtualizar.FlatAppearance.BorderSize = 0;
            btnAtualizar.Click += (s, e) => LoadData();
            this.Controls.Add(btnAtualizar);

            // DataGridView
            dgvSetores = new DataGridView
            {
                Location = new Point(30, 190),
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

            dgvSetores.SelectionChanged += (s, e) =>
            {
                bool hasSelection = dgvSetores.SelectedRows.Count > 0;
                btnEditar.Enabled = hasSelection;
                btnExcluir.Enabled = hasSelection;
            };

            dgvSetores.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    BtnEditar_Click(s, e);
            };

            this.Controls.Add(dgvSetores);

            // Label de contagem
            Label lblInfo = new Label
            {
                Name = "lblInfo",
                Text = "Total: 0 setores",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(30, 550)
            };
            this.Controls.Add(lblInfo);
        }

        private async void LoadData()
        {
            try
            {
                btnAtualizar.Enabled = false;
                btnAtualizar.Text = "Carregando...";

                todosSetores = await ApiService.GetAsync<List<SetorResponseDto>>("/api/Setores");

                FiltrarSetores();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar setores:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAtualizar.Enabled = true;
                btnAtualizar.Text = "Atualizar";
            }
        }

        private void FiltrarSetores()
        {
            if (todosSetores == null)
                return;

            var setoresFiltrados = todosSetores.AsEnumerable();

            // Filtro de busca
            if (!string.IsNullOrWhiteSpace(txtBusca.Text))
            {
                string busca = txtBusca.Text.ToLower();
                setoresFiltrados = setoresFiltrados.Where(s =>
                    s.Nome.ToLower().Contains(busca) ||
                    (s.Descricao != null && s.Descricao.ToLower().Contains(busca)));
            }

            var lista = setoresFiltrados.ToList();
            PreencherGrid(lista);

            // Atualizar contador
            var lblInfo = this.Controls.Find("lblInfo", false).FirstOrDefault() as Label;
            if (lblInfo != null)
            {
                lblInfo.Text = $"Total: {lista.Count} setor(es)";
            }
        }

        private void PreencherGrid(List<SetorResponseDto> setores)
        {
            dgvSetores.DataSource = null;
            dgvSetores.Columns.Clear();

            if (setores == null || setores.Count == 0)
            {
                return;
            }

            dgvSetores.DataSource = setores;

            // Configurar colunas
            dgvSetores.Columns["Id"].HeaderText = "ID";
            dgvSetores.Columns["Id"].Width = 80;
            dgvSetores.Columns["Nome"].HeaderText = "Nome";
            dgvSetores.Columns["Nome"].Width = 250;
            dgvSetores.Columns["Descricao"].HeaderText = "Descrição";
            dgvSetores.Columns["Descricao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            SetorEditForm editForm = new SetorEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvSetores.SelectedRows.Count == 0)
                return;

            var setor = (SetorResponseDto)dgvSetores.SelectedRows[0].DataBoundItem;
            SetorEditForm editForm = new SetorEditForm(setor.Id);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private async void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvSetores.SelectedRows.Count == 0)
                return;

            var setor = (SetorResponseDto)dgvSetores.SelectedRows[0].DataBoundItem;

            DialogResult resultado = MessageBox.Show(
                $"Deseja realmente excluir o setor '{setor.Nome}'?\n\n" +
                "ATENÇÃO: Esta ação não pode ser desfeita e pode afetar usuários vinculados a este setor.",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    bool sucesso = await ApiService.DeleteAsync($"/api/Setores/{setor.Id}");

                    if (sucesso)
                    {
                        MessageBox.Show("Setor excluído com sucesso!",
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível excluir o setor.",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir setor:\n{ex.Message}",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
