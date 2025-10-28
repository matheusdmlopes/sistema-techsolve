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
    public partial class ChamadoEditForm : Form
    {
        private TextBox txtTitulo;
        private TextBox txtDescricao;
        private ComboBox cboCategoria;
        private ComboBox cboPrioridade;
        private Button btnSalvar;
        private Button btnCancelar;

        private List<CategoriaResponseDto> categorias;
        private List<PrioridadeResponseDto> prioridades;

        public ChamadoEditForm()
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
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Name = "ChamadoEditForm";
            this.Text = "Novo Chamado";
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(650, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Título
            Label lblTituloPagina = new Label
            {
                Text = "Criar Novo Chamado",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(30, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTituloPagina);

            // Panel de formulário
            Panel panelForm = new Panel
            {
                Location = new Point(30, 70),
                Size = new Size(580, 380),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            int yPos = 20;

            // Título do chamado
            Label lblTitulo = new Label
            {
                Text = "Título:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelForm.Controls.Add(lblTitulo);

            txtTitulo = new TextBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(540, 23),
                Font = new Font("Segoe UI", 9),
                MaxLength = 200
            };
            panelForm.Controls.Add(txtTitulo);

            yPos += 65;

            // Descrição
            Label lblDescricao = new Label
            {
                Text = "Descrição:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelForm.Controls.Add(lblDescricao);

            txtDescricao = new TextBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(540, 120),
                Font = new Font("Segoe UI", 9),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = 2000
            };
            panelForm.Controls.Add(txtDescricao);

            yPos += 160;

            // Categoria
            Label lblCategoria = new Label
            {
                Text = "Categoria:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelForm.Controls.Add(lblCategoria);

            cboCategoria = new ComboBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(250, 23),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panelForm.Controls.Add(cboCategoria);

            // Prioridade
            Label lblPrioridade = new Label
            {
                Text = "Prioridade:",
                Location = new Point(290, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            panelForm.Controls.Add(lblPrioridade);

            cboPrioridade = new ComboBox
            {
                Location = new Point(290, yPos + 25),
                Size = new Size(270, 23),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panelForm.Controls.Add(cboPrioridade);

            yPos += 65;

            // Informação
            Label lblInfo = new Label
            {
                Text = "Nota: O chamado será criado com status 'Aberto' automaticamente.",
                Location = new Point(20, yPos),
                Size = new Size(540, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141)
            };
            panelForm.Controls.Add(lblInfo);

            this.Controls.Add(panelForm);

            // Botões
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(380, 465),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnCancelar);

            btnSalvar = new Button
            {
                Text = "Criar Chamado",
                Location = new Point(500, 465),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += BtnSalvar_Click;
            this.Controls.Add(btnSalvar);

            this.CancelButton = btnCancelar;
        }

        private async void CarregarDados()
        {
            try
            {
                // Carregar categorias e prioridades
                var categoriasTask = ApiService.GetAsync<List<CategoriaResponseDto>>("/api/Categorias");
                var prioridadesTask = ApiService.GetAsync<List<PrioridadeResponseDto>>("/api/Prioridades");

                await Task.WhenAll(categoriasTask, prioridadesTask);

                categorias = await categoriasTask;
                prioridades = await prioridadesTask;

                // Preencher combos
                cboCategoria.DisplayMember = "Nome";
                cboCategoria.ValueMember = "IdCategoria";
                cboCategoria.DataSource = categorias;

                cboPrioridade.DisplayMember = "Nivel";
                cboPrioridade.ValueMember = "IdPrioridade";
                cboPrioridade.DataSource = prioridades;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSalvar_Click(object sender, EventArgs e)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("O título é obrigatório.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescricao.Text) || txtDescricao.Text.Length < 10)
            {
                MessageBox.Show("A descrição deve ter pelo menos 10 caracteres.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescricao.Focus();
                return;
            }

            if (cboCategoria.SelectedValue == null)
            {
                MessageBox.Show("Selecione uma categoria.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboCategoria.Focus();
                return;
            }

            if (cboPrioridade.SelectedValue == null)
            {
                MessageBox.Show("Selecione uma prioridade.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboPrioridade.Focus();
                return;
            }

            try
            {
                btnSalvar.Enabled = false;
                btnSalvar.Text = "Salvando...";

                var request = new CriarChamadoRequest
                {
                    Titulo = txtTitulo.Text.Trim(),
                    Descricao = txtDescricao.Text.Trim(),
                    IdCategoria = (int)cboCategoria.SelectedValue,
                    IdPrioridade = (int)cboPrioridade.SelectedValue
                    // IdUsuarioCriador será extraído do token JWT pela API
                };

                var response = await ApiService.PostAsync<CriarChamadoRequest, ChamadoResponseDto>("/api/Chamados", request);

                MessageBox.Show("Chamado criado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar chamado: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSalvar.Enabled = true;
                btnSalvar.Text = "Criar Chamado";
            }
        }
    }
}
