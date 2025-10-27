using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;

namespace PIM.Desktop.Forms
{
    public partial class UsuarioEditForm : Form
    {
        private int? idUsuario;
        private List<SetorDto> setores;
        private List<PerfilDto> perfis;

        private TextBox txtNome;
        private TextBox txtEmail;
        private TextBox txtTelefone;
        private TextBox txtSenha;
        private ComboBox cboSetor;
        private ComboBox cboPerfil;
        private CheckBox chkAtivo;
        private Button btnSalvar;
        private Button btnCancelar;

        public UsuarioEditForm(List<SetorDto> setores, List<PerfilDto> perfis, int? idUsuario = null)
        {
            this.setores = setores;
            this.perfis = perfis;
            this.idUsuario = idUsuario;

            InitializeComponent();
            InitializeCustomComponents();

            if (idUsuario.HasValue)
                LoadUsuario();
        }

        private void InitializeCustomComponents()
        {
            this.Text = idUsuario.HasValue ? "Editar Usuário" : "Novo Usuário";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Título
            Label lblTitle = new Label
            {
                Text = idUsuario.HasValue ? "Editar Usuário" : "Novo Usuário",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(30, 20)
            };
            this.Controls.Add(lblTitle);

            int yPos = 70;

            // Nome Completo
            AddLabel("Nome Completo:*", 30, yPos);
            txtNome = new TextBox
            {
                Location = new Point(30, yPos + 25),
                Size = new Size(420, 25)
            };
            this.Controls.Add(txtNome);
            yPos += 60;

            // Email
            AddLabel("E-mail:*", 30, yPos);
            txtEmail = new TextBox
            {
                Location = new Point(30, yPos + 25),
                Size = new Size(420, 25)
            };
            this.Controls.Add(txtEmail);
            yPos += 60;

            // Senha (apenas para novo usuário)
            if (!idUsuario.HasValue)
            {
                AddLabel("Senha:*", 30, yPos);
                txtSenha = new TextBox
                {
                    Location = new Point(30, yPos + 25),
                    Size = new Size(420, 25),
                    UseSystemPasswordChar = true
                };
                this.Controls.Add(txtSenha);
                yPos += 60;
            }

            // Telefone
            AddLabel("Telefone:", 30, yPos);
            txtTelefone = new TextBox
            {
                Location = new Point(30, yPos + 25),
                Size = new Size(200, 25)
            };
            this.Controls.Add(txtTelefone);
            yPos += 60;

            // Setor
            AddLabel("Setor:*", 30, yPos);
            cboSetor = new ComboBox
            {
                Location = new Point(30, yPos + 25),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboSetor.DisplayMember = "NomeSetor";
            cboSetor.ValueMember = "IdSetor";
            cboSetor.DataSource = setores;
            this.Controls.Add(cboSetor);

            // Perfil
            AddLabel("Perfil:*", 250, yPos);
            cboPerfil = new ComboBox
            {
                Location = new Point(250, yPos + 25),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboPerfil.DisplayMember = "NomePerfil";
            cboPerfil.ValueMember = "IdPerfil";
            cboPerfil.DataSource = perfis;
            this.Controls.Add(cboPerfil);
            yPos += 60;

            // Checkbox Ativo (apenas para edição)
            if (idUsuario.HasValue)
            {
                chkAtivo = new CheckBox
                {
                    Text = "Usuário ativo",
                    Location = new Point(30, yPos),
                    AutoSize = true,
                    Checked = true
                };
                this.Controls.Add(chkAtivo);
                yPos += 40;
            }

            // Botões
            btnSalvar = new Button
            {
                Text = "💾 Salvar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(210, yPos + 20),
                Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += BtnSalvar_Click;
            this.Controls.Add(btnSalvar);

            btnCancelar = new Button
            {
                Text = "❌ Cancelar",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(231, 76, 60),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(340, yPos + 20),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancelar);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(x, y)
            };
            this.Controls.Add(lbl);
        }

        private async void LoadUsuario()
        {
            try
            {
                var usuario = await ApiService.GetAsync<UsuarioDetailDto>($"/api/Usuarios/{idUsuario}");

                if (usuario != null)
                {
                    txtNome.Text = usuario.NomeCompleto;
                    txtEmail.Text = usuario.Email;
                    txtTelefone.Text = usuario.Telefone;
                    cboSetor.SelectedValue = usuario.IdSetor;
                    cboPerfil.SelectedValue = usuario.IdPerfil;

                    if (chkAtivo != null)
                        chkAtivo.Checked = usuario.Ativo;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuário:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private async void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                btnSalvar.Enabled = false;
                btnSalvar.Text = "Salvando...";

                if (idUsuario.HasValue)
                {
                    // Atualizar usuário existente
                    var request = new AtualizarUsuarioRequest
                    {
                        NomeCompleto = txtNome.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Telefone = txtTelefone.Text.Trim(),
                        IdSetor = (int)cboSetor.SelectedValue,
                        IdPerfil = (int)cboPerfil.SelectedValue,
                        Ativo = chkAtivo.Checked
                    };

                    await ApiService.PutAsync<AtualizarUsuarioRequest, object>(
                        $"/api/Usuarios/{idUsuario}", request);

                    MessageBox.Show("Usuário atualizado com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Criar novo usuário
                    var request = new CriarUsuarioRequest
                    {
                        NomeCompleto = txtNome.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Senha = txtSenha.Text,
                        Telefone = txtTelefone.Text.Trim(),
                        IdSetor = (int)cboSetor.SelectedValue,
                        IdPerfil = (int)cboPerfil.SelectedValue
                    };

                    await ApiService.PostAsync<CriarUsuarioRequest, object>(
                        "/api/Usuarios", request);

                    MessageBox.Show("Usuário criado com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar usuário:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSalvar.Enabled = true;
                btnSalvar.Text = "💾 Salvar";
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome completo.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Informe o e-mail.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!idUsuario.HasValue && string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                MessageBox.Show("Informe a senha.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSenha.Focus();
                return false;
            }

            if (cboSetor.SelectedValue == null)
            {
                MessageBox.Show("Selecione um setor.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboSetor.Focus();
                return false;
            }

            if (cboPerfil.SelectedValue == null)
            {
                MessageBox.Show("Selecione um perfil.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboPerfil.Focus();
                return false;
            }

            return true;
        }
    }
}
