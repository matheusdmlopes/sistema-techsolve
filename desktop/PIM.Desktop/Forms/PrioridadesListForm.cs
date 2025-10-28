using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;

namespace PIM.Desktop.Forms
{
    public partial class PrioridadesListForm : Form
    {
        private DataGridView dgvPrioridades;
        private TextBox txtBusca;
        private Button btnNovo;
        private Button btnEditar;
        private Button btnExcluir;
        private Button btnAtualizar;
        private List<PrioridadeResponseDto> todasPrioridades;

        public PrioridadesListForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.AutoScroll = true;

            Label lblTitle = new Label
            {
                Text = "Gestão de Prioridades",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(30, 30)
            };
            this.Controls.Add(lblTitle);

            Panel panelBusca = new Panel
            {
                BackColor = Color.White,
                Location = new Point(30, 80),
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
            txtBusca.TextChanged += (s, e) => FiltrarPrioridades();
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

            btnNovo = new Button
            {
                Text = "➕ Nova Prioridade",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 40),
                Location = new Point(30, 155),
                Cursor = Cursors.Hand
            };
            btnNovo.FlatAppearance.BorderSize = 0;
            btnNovo.Click += BtnNovo_Click;
            this.Controls.Add(btnNovo);

            btnEditar = new Button
            {
                Text = "✏️ Editar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 40),
                Location = new Point(210, 155),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += BtnEditar_Click;
            this.Controls.Add(btnEditar);

            btnExcluir = new Button
            {
                Text = "🗑️ Excluir",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(231, 76, 60),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 40),
                Location = new Point(330, 155),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnExcluir.FlatAppearance.BorderSize = 0;
            btnExcluir.Click += BtnExcluir_Click;
            this.Controls.Add(btnExcluir);

            btnAtualizar = new Button
            {
                Text = "🔄 Atualizar",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(127, 140, 141),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(450, 155),
                Cursor = Cursors.Hand
            };
            btnAtualizar.FlatAppearance.BorderSize = 0;
            btnAtualizar.Click += (s, e) => LoadData();
            this.Controls.Add(btnAtualizar);

            dgvPrioridades = new DataGridView
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

            dgvPrioridades.SelectionChanged += (s, e) =>
            {
                bool hasSelection = dgvPrioridades.SelectedRows.Count > 0;
                btnEditar.Enabled = hasSelection;
                btnExcluir.Enabled = hasSelection;
            };

            dgvPrioridades.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    BtnEditar_Click(s, e);
            };

            this.Controls.Add(dgvPrioridades);

            Label lblInfo = new Label
            {
                Name = "lblInfo",
                Text = "Total: 0 prioridades",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(30, 570)
            };
            this.Controls.Add(lblInfo);
        }

        private async void LoadData()
        {
            try
            {
                btnAtualizar.Enabled = false;
                btnAtualizar.Text = "Carregando...";

                todasPrioridades = await ApiService.GetAsync<List<PrioridadeResponseDto>>("/api/Prioridades");

                FiltrarPrioridades();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar prioridades:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAtualizar.Enabled = true;
                btnAtualizar.Text = "🔄 Atualizar";
            }
        }

        private void FiltrarPrioridades()
        {
            if (todasPrioridades == null)
                return;

            var prioridadesFiltradas = todasPrioridades.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtBusca.Text))
            {
                string busca = txtBusca.Text.ToLower();
                prioridadesFiltradas = prioridadesFiltradas.Where(p =>
                    p.Nivel.ToLower().Contains(busca) ||
                    (p.Descricao != null && p.Descricao.ToLower().Contains(busca)));
            }

            var lista = prioridadesFiltradas.ToList();
            PreencherGrid(lista);

            var lblInfo = this.Controls.Find("lblInfo", false).FirstOrDefault() as Label;
            if (lblInfo != null)
            {
                lblInfo.Text = $"Total: {lista.Count} prioridade(s)";
            }
        }

        private void PreencherGrid(List<PrioridadeResponseDto> prioridades)
        {
            dgvPrioridades.DataSource = null;
            dgvPrioridades.Columns.Clear();

            if (prioridades == null || prioridades.Count == 0)
            {
                return;
            }

            dgvPrioridades.DataSource = prioridades;

            dgvPrioridades.Columns["IdPrioridade"].HeaderText = "ID";
            dgvPrioridades.Columns["IdPrioridade"].Width = 80;
            dgvPrioridades.Columns["Nivel"].HeaderText = "Nível";
            dgvPrioridades.Columns["Nivel"].Width = 200;
            dgvPrioridades.Columns["Descricao"].HeaderText = "Descrição";
            dgvPrioridades.Columns["Descricao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            PrioridadeEditForm editForm = new PrioridadeEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvPrioridades.SelectedRows.Count == 0)
                return;

            var prioridade = (PrioridadeResponseDto)dgvPrioridades.SelectedRows[0].DataBoundItem;
            PrioridadeEditForm editForm = new PrioridadeEditForm(prioridade.IdPrioridade);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private async void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvPrioridades.SelectedRows.Count == 0)
                return;

            var prioridade = (PrioridadeResponseDto)dgvPrioridades.SelectedRows[0].DataBoundItem;

            DialogResult resultado = MessageBox.Show(
                $"Deseja realmente excluir a prioridade '{prioridade.Nivel}'?\n\n" +
                "ATENÇÃO: Esta ação não pode ser desfeita e pode afetar chamados vinculados a esta prioridade.",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    bool sucesso = await ApiService.DeleteAsync($"/api/Prioridades/{prioridade.IdPrioridade}");

                    if (sucesso)
                    {
                        MessageBox.Show("Prioridade excluída com sucesso!",
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível excluir a prioridade.",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir prioridade:\n{ex.Message}",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
