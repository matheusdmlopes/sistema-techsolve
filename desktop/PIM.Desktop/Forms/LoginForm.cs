using System;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;
using PIM.Desktop.Utils;

namespace PIM.Desktop.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Configurar evento do botão Entrar
            this.Button1.Click += Button1_Click;

            // Configurar Enter no campo de senha para fazer login
            this.TextBox2.KeyPress += TextBox2_KeyPress;
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            // Validação básica dos campos
            string email = TextBox1.Text.Trim();
            string senha = TextBox2.Text;

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Por favor, informe o e-mail.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TextBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, informe a senha.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TextBox2.Focus();
                return;
            }

            // Desabilitar botão durante o login
            Button1.Enabled = false;
            Button1.Text = "Autenticando...";

            try
            {
                // Chamar a API para fazer login
                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Senha = senha
                };

                LoginResponse response = await ApiService.PostAsync<LoginRequest, LoginResponse>(
                    "/api/Auth/login", loginRequest);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    // Verificar se o usuário é Admin ou Administrador
                    if (response.Usuario.Perfil != null &&
                        (response.Usuario.Perfil.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                         response.Usuario.Perfil.Equals("Administrador", StringComparison.OrdinalIgnoreCase)))
                    {
                        // Armazenar dados da sessão
                        SessionManager.Token = response.Token;
                        SessionManager.UsuarioId = response.Usuario.IdUsuario;
                        SessionManager.UsuarioNome = response.Usuario.Nome;
                        SessionManager.UsuarioEmail = response.Usuario.Email;
                        SessionManager.PerfilAcesso = response.Usuario.Perfil;
                        SessionManager.IdPerfil = response.Usuario.IdPerfil;
                        SessionManager.Setor = response.Usuario.Setor;

                        // Configurar o token no ApiService para as próximas requisições
                        ApiService.SetAuthToken(response.Token);

                        // Abrir a tela principal
                        this.Hide();

                        MainForm mainForm = new MainForm();
                        mainForm.FormClosed += (s, args) => this.Close();
                        mainForm.Show();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Acesso negado. Este sistema é exclusivo para administradores.\n" +
                            $"Seu perfil: {response.Usuario.Perfil ?? "Desconhecido"}",
                            "Acesso Negado",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // Limpar campos
                        TextBox2.Clear();
                        TextBox1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Erro ao processar resposta da API.",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao fazer login:\n\n{ex.Message}\n\n" +
                    $"Verifique se a API está em execução em: {AppConfig.ApiBaseUrl}",
                    "Erro de Conexão",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Reabilitar botão
                Button1.Enabled = true;
                Button1.Text = "ENTRAR";
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Funcionalidade de recuperação de senha será implementada em breve.",
                "Em Desenvolvimento", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Pressionar Enter no campo de senha = clicar no botão Entrar
            if (e.KeyChar == (char)Keys.Enter)
            {
                Button1_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
