using System;
using System.Drawing;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;

namespace PIM.Desktop.Forms
{
    public partial class SetorEditForm : Form
    {
        private int? idSetor;
        private TextBox txtNome;
        private TextBox txtDescricao;
        private Button btnSalvar;
        private Button btnCancelar;

        public SetorEditForm(int? idSetor = null)
        {
            this.idSetor = idSetor;

            InitializeComponent();
            InitializeCustomComponents();

            if (idSetor.HasValue)
                LoadSetor();
        }

        private void InitializeCustomComponents()
        {
            this.Text = idSetor.HasValue ? "Editar Setor" : "Novo Setor";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Título
            Label lblTitle = new Label
            {
                Text = idSetor.HasValue ? "Editar Setor" : "Novo Setor",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(30, 20)
            };
            this.Controls.Add(lblTitle);

            int yPos = 70;

            // Nome
            Label lblNome = new Label
            {
                Text = "Nome do Setor:*",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(30, yPos),
                AutoSize = true
            };
            this.Controls.Add(lblNome);

            txtNome = new TextBox
            {
                Location = new Point(30, yPos + 25),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtNome);
            yPos += 70;

            // Descrição
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

            // Botões
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

            // Campo obrigatório
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

        private async void LoadSetor()
        {
            try
            {
                var setor = await ApiService.GetAsync<SetorResponseDto>($"/api/Setores/{idSetor}");

                if (setor != null)
                {
                    txtNome.Text = setor.Nome;
                    txtDescricao.Text = setor.Descricao;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar setor:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async void BtnSalvar_Click(object sender, EventArgs e)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O nome do setor é obrigatório.",
                    "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return;
            }

            try
            {
                btnSalvar.Enabled = false;
                btnSalvar.Text = "Salvando...";

                if (idSetor.HasValue)
                {
                    // Atualizar setor existente
                    var request = new SetorUpdateRequest
                    {
                        Nome = txtNome.Text.Trim(),
                        Descricao = txtDescricao.Text.Trim()
                    };

                    var response = await ApiService.PutAsync<SetorUpdateRequest, SetorResponseDto>(
                        $"/api/Setores/{idSetor}", request);

                    MessageBox.Show("Setor atualizado com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Criar novo setor
                    var request = new SetorCreateRequest
                    {
                        Nome = txtNome.Text.Trim(),
                        Descricao = txtDescricao.Text.Trim()
                    };

                    var response = await ApiService.PostAsync<SetorCreateRequest, SetorResponseDto>(
                        "/api/Setores", request);

                    MessageBox.Show("Setor criado com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar setor:\n{ex.Message}",
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
