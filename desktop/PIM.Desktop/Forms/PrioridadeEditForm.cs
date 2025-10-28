using System;
using System.Drawing;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;

namespace PIM.Desktop.Forms
{
    public partial class PrioridadeEditForm : Form
    {
        private int? idPrioridade;
        private TextBox txtNivel;
        private TextBox txtDescricao;
        private Button btnSalvar;
        private Button btnCancelar;

        public PrioridadeEditForm(int? idPrioridade = null)
        {
            this.idPrioridade = idPrioridade;

            InitializeComponent();
            InitializeCustomComponents();

            if (idPrioridade.HasValue)
                LoadPrioridade();
        }

        private void InitializeCustomComponents()
        {
            this.Text = idPrioridade.HasValue ? "Editar Prioridade" : "Nova Prioridade";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            Label lblTitle = new Label
            {
                Text = idPrioridade.HasValue ? "Editar Prioridade" : "Nova Prioridade",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(30, 20)
            };
            this.Controls.Add(lblTitle);

            int yPos = 70;

            Label lblNivel = new Label
            {
                Text = "Nível da Prioridade:*",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(30, yPos),
                AutoSize = true
            };
            this.Controls.Add(lblNivel);

            txtNivel = new TextBox
            {
                Location = new Point(30, yPos + 25),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtNivel);
            yPos += 70;

            Label lblDescricao = new Label
            {
                Text = "Descrição:",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(30, yPos),
                AutoSize = true
            };
            this.Controls.Add(lblDescricao);

            txtDescricao = new TextBox
            {
                Location = new Point(30, yPos + 25),
                Size = new Size(420, 80),
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(txtDescricao);
            yPos += 120;

            btnSalvar = new Button
            {
                Text = "💾 Salvar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(210, yPos),
                Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += BtnSalvar_Click;
            this.Controls.Add(btnSalvar);

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(127, 140, 141),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(340, yPos),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancelar);

            Label lblObrigatorio = new Label
            {
                Text = "* Campos obrigatórios",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(30, yPos + 15),
                AutoSize = true
            };
            this.Controls.Add(lblObrigatorio);
        }

        private async void LoadPrioridade()
        {
            try
            {
                var prioridade = await ApiService.GetAsync<PrioridadeResponseDto>($"/api/Prioridades/{idPrioridade}");

                if (prioridade != null)
                {
                    txtNivel.Text = prioridade.Nivel;
                    txtDescricao.Text = prioridade.Descricao;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar prioridade:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNivel.Text))
            {
                MessageBox.Show("O nível da prioridade é obrigatório.",
                    "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNivel.Focus();
                return;
            }

            try
            {
                btnSalvar.Enabled = false;
                btnSalvar.Text = "Salvando...";

                if (idPrioridade.HasValue)
                {
                    var request = new PrioridadeUpdateRequest
                    {
                        Nivel = txtNivel.Text.Trim(),
                        Descricao = txtDescricao.Text.Trim()
                    };

                    var response = await ApiService.PutAsync<PrioridadeUpdateRequest, PrioridadeResponseDto>(
                        $"/api/Prioridades/{idPrioridade}", request);

                    MessageBox.Show("Prioridade atualizada com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var request = new PrioridadeCreateRequest
                    {
                        Nivel = txtNivel.Text.Trim(),
                        Descricao = txtDescricao.Text.Trim()
                    };

                    var response = await ApiService.PostAsync<PrioridadeCreateRequest, PrioridadeResponseDto>(
                        "/api/Prioridades", request);

                    MessageBox.Show("Prioridade criada com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar prioridade:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSalvar.Enabled = true;
                btnSalvar.Text = "💾 Salvar";
            }
        }
    }
}
