using System;
using System.Drawing;
using System.Windows.Forms;
using PIM.Desktop.Services;
using PIM.Desktop.Utils;

namespace PIM.Desktop.Forms
{
    public partial class MainForm : Form
    {
        private Panel panelMenu;
        private Panel panelTopBar;
        private Panel panelContent;
        private Label lblUsuario;
        private Button btnLogout;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadDashboard(); // Carregar dashboard por padrão
        }

        private void InitializeCustomComponents()
        {
            // Configurar o form principal
            this.Text = "TechSolve - Sistema Administrativo";
            this.Size = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1024, 600);

            // Criar painel superior (TopBar)
            CreateTopBar();

            // Criar menu lateral
            CreateSideMenu();

            // Criar área de conteúdo
            CreateContentArea();
        }

        private void CreateTopBar()
        {
            panelTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 152, 219) // Azul TechSolve
            };

            // Logo/Título
            Label lblLogo = new Label
            {
                Text = "TechSolve",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Nome do usuário
            lblUsuario = new Label
            {
                Text = $"Olá, {SessionManager.UsuarioNome}",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = true
            };
            lblUsuario.Location = new Point(this.Width - lblUsuario.Width - 150, 20);

            // Botão Logout
            btnLogout = new Button
            {
                Text = "Sair",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(231, 76, 60), // Vermelho
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Location = new Point(this.Width - 110, 12);
            btnLogout.Click += BtnLogout_Click;

            panelTopBar.Controls.Add(lblLogo);
            panelTopBar.Controls.Add(lblUsuario);
            panelTopBar.Controls.Add(btnLogout);

            this.Controls.Add(panelTopBar);

            // Ajustar posições ao redimensionar
            this.Resize += (s, e) =>
            {
                lblUsuario.Location = new Point(this.Width - 250, 20);
                btnLogout.Location = new Point(this.Width - 110, 12);
            };
        }

        private void CreateSideMenu()
        {
            panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(44, 62, 80) // Azul escuro
            };

            int yPosition = 20;

            // Botão Dashboard
            Button btnDashboard = CreateMenuButton("🏠 Dashboard", yPosition);
            btnDashboard.Click += (s, e) => LoadDashboard();
            panelMenu.Controls.Add(btnDashboard);
            yPosition += 50;

            // Botão Usuários
            Button btnUsuarios = CreateMenuButton("👥 Usuários", yPosition);
            btnUsuarios.Click += (s, e) => LoadUsuarios();
            panelMenu.Controls.Add(btnUsuarios);
            yPosition += 50;

            // Label Configurações
            Label lblConfig = new Label
            {
                Text = "CONFIGURAÇÕES",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(20, yPosition),
                AutoSize = true
            };
            panelMenu.Controls.Add(lblConfig);
            yPosition += 30;

            // Botão Setores
            Button btnSetores = CreateMenuButton("  📁 Setores", yPosition);
            btnSetores.Click += (s, e) => LoadForm(new Label { Text = "Setores - Em Desenvolvimento", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16) });
            panelMenu.Controls.Add(btnSetores);
            yPosition += 50;

            // Botão Categorias
            Button btnCategorias = CreateMenuButton("  🏷️ Categorias", yPosition);
            btnCategorias.Click += (s, e) => LoadForm(new Label { Text = "Categorias - Em Desenvolvimento", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16) });
            panelMenu.Controls.Add(btnCategorias);
            yPosition += 50;

            // Botão Prioridades
            Button btnPrioridades = CreateMenuButton("  ⚡ Prioridades", yPosition);
            btnPrioridades.Click += (s, e) => LoadForm(new Label { Text = "Prioridades - Em Desenvolvimento", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16) });
            panelMenu.Controls.Add(btnPrioridades);
            yPosition += 50;

            // Botão Status
            Button btnStatus = CreateMenuButton("  🔄 Status", yPosition);
            btnStatus.Click += (s, e) => LoadForm(new Label { Text = "Status - Em Desenvolvimento", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16) });
            panelMenu.Controls.Add(btnStatus);
            yPosition += 70;

            // Botão Chamados
            Button btnChamados = CreateMenuButton("📋 Chamados", yPosition);
            btnChamados.Click += (s, e) => LoadForm(new Label { Text = "Chamados - Em Desenvolvimento", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16) });
            panelMenu.Controls.Add(btnChamados);
            yPosition += 50;

            // Botão Relatórios
            Button btnRelatorios = CreateMenuButton("📊 Relatórios", yPosition);
            btnRelatorios.Click += (s, e) => LoadForm(new Label { Text = "Relatórios - Em Desenvolvimento", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16) });
            panelMenu.Controls.Add(btnRelatorios);

            this.Controls.Add(panelMenu);
        }

        private Button CreateMenuButton(string text, int yPosition)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(44, 62, 80),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(250, 45),
                Location = new Point(0, yPosition),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 73, 94);

            return btn;
        }

        private void CreateContentArea()
        {
            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(236, 240, 241),
                Padding = new Padding(20)
            };

            this.Controls.Add(panelContent);
        }

        private void LoadDashboard()
        {
            DashboardForm dashboardForm = new DashboardForm
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            LoadForm(dashboardForm);
        }

        private void LoadUsuarios()
        {
            UsuariosListForm usuariosForm = new UsuariosListForm
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            LoadForm(usuariosForm);
        }

        private void LoadForm(Control childControl)
        {
            // Limpar conteúdo anterior
            panelContent.Controls.Clear();

            // Se for um Form, mostrar ele
            if (childControl is Form)
            {
                Form childForm = (Form)childControl;
                childForm.Show();
            }

            // Adicionar novo controle
            childControl.Dock = DockStyle.Fill;
            panelContent.Controls.Add(childControl);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Deseja realmente sair do sistema?",
                "Confirmar Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Limpar sessão
                SessionManager.Clear();
                ApiService.SetAuthToken(null);

                // Voltar para tela de login
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.FormClosed += (s, args) => this.Close();
                loginForm.Show();
            }
        }
    }
}
