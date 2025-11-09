namespace PIM.Desktop.Forms
{
    partial class LoginForm
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelLoginCard = new System.Windows.Forms.Panel();
            this.LinkLabel1 = new System.Windows.Forms.LinkLabel();
            this.Button1 = new System.Windows.Forms.Button();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.panelMain.SuspendLayout();
            this.panelLoginCard.SuspendLayout();
            this.SuspendLayout();
            //
            // panelMain
            //
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.panelMain.Controls.Add(this.panelLoginCard);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1280, 720);
            this.panelMain.TabIndex = 0;
            //
            // panelLoginCard
            //
            this.panelLoginCard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelLoginCard.BackColor = System.Drawing.Color.White;
            this.panelLoginCard.Controls.Add(this.LinkLabel1);
            this.panelLoginCard.Controls.Add(this.Button1);
            this.panelLoginCard.Controls.Add(this.TextBox2);
            this.panelLoginCard.Controls.Add(this.Label3);
            this.panelLoginCard.Controls.Add(this.TextBox1);
            this.panelLoginCard.Controls.Add(this.Label2);
            this.panelLoginCard.Controls.Add(this.Label1);
            this.panelLoginCard.Controls.Add(this.lblSubtitle);
            this.panelLoginCard.Location = new System.Drawing.Point(390, 160);
            this.panelLoginCard.Name = "panelLoginCard";
            this.panelLoginCard.Size = new System.Drawing.Size(500, 500);
            this.panelLoginCard.TabIndex = 0;
            //
            // LinkLabel1
            //
            this.LinkLabel1.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.LinkLabel1.AutoSize = true;
            this.LinkLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LinkLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LinkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.LinkLabel1.Location = new System.Drawing.Point(165, 450);
            this.LinkLabel1.Name = "LinkLabel1";
            this.LinkLabel1.Size = new System.Drawing.Size(170, 20);
            this.LinkLabel1.TabIndex = 4;
            this.LinkLabel1.TabStop = true;
            this.LinkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LinkLabel1.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            //
            // Button1
            //
            this.Button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.Button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button1.FlatAppearance.BorderSize = 0;
            this.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button1.ForeColor = System.Drawing.Color.White;
            this.Button1.Location = new System.Drawing.Point(80, 380);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(340, 50);
            this.Button1.TabIndex = 3;
            this.Button1.Text = "ENTRAR";
            this.Button1.UseVisualStyleBackColor = false;
            //
            // TextBox2
            //
            this.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBox2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox2.Location = new System.Drawing.Point(80, 315);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.Size = new System.Drawing.Size(340, 32);
            this.TextBox2.TabIndex = 2;
            this.TextBox2.UseSystemPasswordChar = true;
            //
            // Label3
            //
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.Label3.Location = new System.Drawing.Point(76, 290);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(55, 20);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Senha:";
            //
            // TextBox1
            //
            this.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBox1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox1.Location = new System.Drawing.Point(80, 235);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(340, 32);
            this.TextBox1.TabIndex = 1;
            //
            // Label2
            //
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.Label2.Location = new System.Drawing.Point(76, 210);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(60, 20);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "E-mail:";
            //
            // Label1
            //
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.Label1.Location = new System.Drawing.Point(130, 80);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(241, 54);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "TechSolve";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.lblSubtitle.Location = new System.Drawing.Point(115, 135);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(271, 23);
            this.lblSubtitle.TabIndex = 0;
            this.lblSubtitle.Text = "Sistema de Gestão de Chamados";
            //
            // LoginForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.panelMain);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TechSolve - Login";
            this.panelMain.ResumeLayout(false);
            this.panelLoginCard.ResumeLayout(false);
            this.panelLoginCard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelLoginCard;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label lblSubtitle;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox TextBox1;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox TextBox2;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.LinkLabel LinkLabel1;
    }
}

