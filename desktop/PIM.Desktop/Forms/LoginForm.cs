using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            // Configurar evento do LinkLabel (esqueceu a senha)
            this.LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;

            // Configurar Enter no campo de senha para fazer login
            this.TextBox2.KeyPress += TextBox2_KeyPress;
        }

        private void Button1_Click(object sender, EventArgs e)
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

            // Por enquanto, simular login bem-sucedido
            // TODO: Integrar com ApiService futuramente
            SessionManager.UsuarioEmail = email;
            SessionManager.UsuarioNome = "Usuário Teste";
            SessionManager.Token = "fake-token-for-testing";

            // Abrir a tela principal
            this.Hide();
            HomeForm homeForm = new HomeForm();
            homeForm.FormClosed += (s, args) => this.Close();
            homeForm.Show();
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
