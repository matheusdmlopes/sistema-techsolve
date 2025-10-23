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
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();

            // Configurar eventos do menu
            this.ChamadosDeTIToolStripMenuItem1.Click += ChamadosDeTIToolStripMenuItem1_Click;
            this.FormuláriosToolStripMenuItem.Click += FormuláriosToolStripMenuItem_Click;

            // Exibir informações do usuário no label
            if (SessionManager.IsAuthenticated)
            {
                this.label6.Text = $"Bem-vindo, {SessionManager.UsuarioEmail}";
            }

            // Configurar evento de fechamento
            this.FormClosing += HomeForm_FormClosing;
        }

        private void ChamadosDeTIToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tela de Chamados de TI será implementada em breve.",
                "Em Desenvolvimento", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FormuláriosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abrir tela de cadastro
            CadastroForm cadastroForm = new CadastroForm();
            cadastroForm.ShowDialog(); // ShowDialog = modal (bloqueia a tela anterior)
        }

        private void EMailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidade de E-mail será implementada em breve.",
                "Em Desenvolvimento", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void eMATENDIMENTOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidade de Atendimento será implementada em breve.",
                "Em Desenvolvimento", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Perguntar se deseja sair
            DialogResult result = MessageBox.Show(
                "Deseja realmente sair do TechSolve?",
                "Confirmar Saída",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Cancela o fechamento
            }
            else
            {
                // Limpar sessão ao sair
                SessionManager.Clear();
            }
        }
    }
}
