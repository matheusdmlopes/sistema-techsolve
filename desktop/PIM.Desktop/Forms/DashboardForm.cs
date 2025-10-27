using System;
using System.Drawing;
using System.Windows.Forms;
using PIM.Desktop.Models;
using PIM.Desktop.Services;

namespace PIM.Desktop.Forms
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadDashboardData();
        }

        private void InitializeCustomComponents()
        {
            this.AutoScroll = true;
            this.BackColor = Color.FromArgb(236, 240, 241);
        }

        private async void LoadDashboardData()
        {
            try
            {
                // Buscar estatísticas da API
                var stats = await ApiService.GetAsync<DashboardStats>("/api/Dashboard/estatisticas");

                if (stats != null)
                {
                    CreateDashboardCards(stats);
                }
                else
                {
                    ShowErrorMessage("Não foi possível carregar as estatísticas.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Erro ao carregar dashboard: {ex.Message}");
                CreateDashboardCards(new DashboardStats()); // Mostrar cards vazios
            }
        }

        private void CreateDashboardCards(DashboardStats stats)
        {
            this.Controls.Clear();

            // Título
            Label lblTitle = new Label
            {
                Text = "Dashboard - Visão Geral",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(30, 30)
            };
            this.Controls.Add(lblTitle);

            // Subtítulo
            Label lblSubtitle = new Label
            {
                Text = $"Atualizado em: {DateTime.Now:dd/MM/yyyy HH:mm}",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(30, 65)
            };
            this.Controls.Add(lblSubtitle);

            // Cards de estatísticas
            int startY = 110;
            int cardWidth = 250;
            int cardHeight = 120;
            int spacing = 20;

            // Card 1: Total de Chamados
            CreateStatCard("Total de Chamados", stats.TotalChamados.ToString(),
                Color.FromArgb(52, 152, 219), 30, startY, cardWidth, cardHeight);

            // Card 2: Chamados Abertos
            CreateStatCard("Chamados Abertos", stats.ChamadosAbertos.ToString(),
                Color.FromArgb(230, 126, 34), 30 + cardWidth + spacing, startY, cardWidth, cardHeight);

            // Card 3: Em Andamento
            CreateStatCard("Em Andamento", stats.ChamadosEmAndamento.ToString(),
                Color.FromArgb(241, 196, 15), 30 + (cardWidth + spacing) * 2, startY, cardWidth, cardHeight);

            // Linha 2
            startY += cardHeight + spacing;

            // Card 4: Resolvidos
            CreateStatCard("Resolvidos", stats.ChamadosResolvidos.ToString(),
                Color.FromArgb(46, 204, 113), 30, startY, cardWidth, cardHeight);

            // Card 5: Total de Usuários
            CreateStatCard("Total de Usuários", stats.TotalUsuarios.ToString(),
                Color.FromArgb(155, 89, 182), 30 + cardWidth + spacing, startY, cardWidth, cardHeight);

            // Card 6: Técnicos
            CreateStatCard("Técnicos Ativos", stats.TotalTecnicos.ToString(),
                Color.FromArgb(52, 73, 94), 30 + (cardWidth + spacing) * 2, startY, cardWidth, cardHeight);

            // Card de Satisfação (maior)
            startY += cardHeight + spacing + 20;
            CreateSatisfactionCard(stats.MediaSatisfacao, 30, startY);

            // Botão Atualizar
            Button btnRefresh = new Button
            {
                Text = "🔄 Atualizar",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 45),
                Location = new Point(30, startY + 180),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadDashboardData();
            this.Controls.Add(btnRefresh);
        }

        private void CreateStatCard(string title, string value, Color color, int x, int y, int width, int height)
        {
            Panel card = new Panel
            {
                BackColor = Color.White,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Barra colorida superior
            Panel topBar = new Panel
            {
                BackColor = color,
                Location = new Point(0, 0),
                Size = new Size(width, 5),
                Dock = DockStyle.Top
            };
            card.Controls.Add(topBar);

            // Título
            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(15, 20),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            // Valor
            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(15, 50),
                AutoSize = true
            };
            card.Controls.Add(lblValue);

            this.Controls.Add(card);
        }

        private void CreateSatisfactionCard(double media, int x, int y)
        {
            Panel card = new Panel
            {
                BackColor = Color.White,
                Location = new Point(x, y),
                Size = new Size(540, 140),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Barra colorida
            Color color = media >= 4 ? Color.FromArgb(46, 204, 113) :
                         media >= 3 ? Color.FromArgb(241, 196, 15) :
                         Color.FromArgb(231, 76, 60);

            Panel topBar = new Panel
            {
                BackColor = color,
                Location = new Point(0, 0),
                Size = new Size(540, 5),
                Dock = DockStyle.Top
            };
            card.Controls.Add(topBar);

            // Título
            Label lblTitle = new Label
            {
                Text = "Média de Satisfação",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 20),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            // Valor da média
            Label lblValue = new Label
            {
                Text = media > 0 ? media.ToString("F1") : "N/A",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(20, 55),
                AutoSize = true
            };
            card.Controls.Add(lblValue);

            // Estrelas (visual simples)
            Label lblStars = new Label
            {
                Text = GetStarsText(media),
                Font = new Font("Segoe UI", 24, FontStyle.Regular),
                ForeColor = Color.FromArgb(241, 196, 15),
                Location = new Point(150, 70),
                AutoSize = true
            };
            card.Controls.Add(lblStars);

            // Descrição
            string descricao = media >= 4 ? "Excelente" :
                              media >= 3 ? "Bom" :
                              media >= 2 ? "Regular" :
                              media > 0 ? "Precisa Melhorar" : "Sem Avaliações";

            Label lblDesc = new Label
            {
                Text = descricao,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(150, 105),
                AutoSize = true
            };
            card.Controls.Add(lblDesc);

            this.Controls.Add(card);
        }

        private string GetStarsText(double media)
        {
            int fullStars = (int)Math.Floor(media);
            string stars = "";

            for (int i = 0; i < 5; i++)
            {
                stars += i < fullStars ? "★" : "☆";
            }

            return stars;
        }

        private void ShowErrorMessage(string message)
        {
            Label lblError = new Label
            {
                Text = $"⚠️ {message}",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(30, 110),
                AutoSize = true
            };
            this.Controls.Add(lblError);
        }
    }
}
