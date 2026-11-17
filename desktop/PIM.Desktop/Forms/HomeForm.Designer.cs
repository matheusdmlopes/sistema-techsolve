namespace PIM.Desktop.Forms
{
    partial class HomeForm
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
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ChamadosDeTIToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.GerenciarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FormuláriosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EMailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PainelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.tRIAGEMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eMATENDIMENTOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eMATENDIMENTOToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.listBox4 = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.MenuStrip1.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChamadosDeTIToolStripMenuItem1,
            this.GerenciarToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new System.Drawing.Size(1902, 30);
            this.MenuStrip1.TabIndex = 1;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // ChamadosDeTIToolStripMenuItem1
            // 
            this.ChamadosDeTIToolStripMenuItem1.Name = "ChamadosDeTIToolStripMenuItem1";
            this.ChamadosDeTIToolStripMenuItem1.Size = new System.Drawing.Size(130, 26);
            this.ChamadosDeTIToolStripMenuItem1.Text = "Chamados de TI";
            // 
            // GerenciarToolStripMenuItem
            // 
            this.GerenciarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FormuláriosToolStripMenuItem,
            this.EMailToolStripMenuItem,
            this.PainelToolStripMenuItem});
            this.GerenciarToolStripMenuItem.Name = "GerenciarToolStripMenuItem";
            this.GerenciarToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.GerenciarToolStripMenuItem.Text = "Gerenciamento de Chamados";
            // 
            // FormuláriosToolStripMenuItem
            // 
            this.FormuláriosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tRIAGEMToolStripMenuItem,
            this.eMATENDIMENTOToolStripMenuItem,
            this.eMATENDIMENTOToolStripMenuItem1});
            this.FormuláriosToolStripMenuItem.Name = "FormuláriosToolStripMenuItem";
            this.FormuláriosToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.FormuláriosToolStripMenuItem.Text = "EM CONCLUSÃO";
            // 
            // EMailToolStripMenuItem
            // 
            this.EMailToolStripMenuItem.Name = "EMailToolStripMenuItem";
            this.EMailToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.EMailToolStripMenuItem.Text = "RESOLVIDOS";
            this.EMailToolStripMenuItem.Click += new System.EventHandler(this.EMailToolStripMenuItem_Click);
            // 
            // PainelToolStripMenuItem
            // 
            this.PainelToolStripMenuItem.Name = "PainelToolStripMenuItem";
            this.PainelToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.PainelToolStripMenuItem.Text = "ARQUIVADOS";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(662, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Portal TechSolve";
            // 
            // Panel1
            // 
            this.Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.ListBox1);
            this.Panel1.Controls.Add(this.Label2);
            this.Panel1.Location = new System.Drawing.Point(157, 239);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(251, 449);
            this.Panel1.TabIndex = 6;
            // 
            // ListBox1
            // 
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 16;
            this.ListBox1.Items.AddRange(new object[] {
            "Internet Instável",
            "Reparo Notebook",
            "Instalação de Aplicativo",
            "Acesso a rede"});
            this.ListBox1.Location = new System.Drawing.Point(18, 66);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(219, 356);
            this.ListBox1.TabIndex = 1;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(81, 17);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(72, 16);
            this.Label2.TabIndex = 0;
            this.Label2.Text = "ABERTOS";
            // 
            // tRIAGEMToolStripMenuItem
            // 
            this.tRIAGEMToolStripMenuItem.Name = "tRIAGEMToolStripMenuItem";
            this.tRIAGEMToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.tRIAGEMToolStripMenuItem.Text = "ABERTOS";
            // 
            // eMATENDIMENTOToolStripMenuItem
            // 
            this.eMATENDIMENTOToolStripMenuItem.Name = "eMATENDIMENTOToolStripMenuItem";
            this.eMATENDIMENTOToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.eMATENDIMENTOToolStripMenuItem.Text = "EM ANÁLISE";
            this.eMATENDIMENTOToolStripMenuItem.Click += new System.EventHandler(this.eMATENDIMENTOToolStripMenuItem_Click);
            // 
            // eMATENDIMENTOToolStripMenuItem1
            // 
            this.eMATENDIMENTOToolStripMenuItem1.Name = "eMATENDIMENTOToolStripMenuItem1";
            this.eMATENDIMENTOToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.eMATENDIMENTOToolStripMenuItem1.Text = "EM ANDAMENTO";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.listBox2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(458, 239);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(251, 449);
            this.panel2.TabIndex = 7;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 16;
            this.listBox2.Items.AddRange(new object[] {
            "Internet Instável",
            "Reparo Notebook",
            "Instalação de Aplicativo",
            "Acesso a rede"});
            this.listBox2.Location = new System.Drawing.Point(18, 66);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(219, 356);
            this.listBox2.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(81, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "EM ANÁLISE";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.listBox3);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Location = new System.Drawing.Point(759, 239);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(251, 449);
            this.panel3.TabIndex = 8;
            // 
            // listBox3
            // 
            this.listBox3.FormattingEnabled = true;
            this.listBox3.ItemHeight = 16;
            this.listBox3.Items.AddRange(new object[] {
            "Internet Instável",
            "Reparo Notebook",
            "Instalação de Aplicativo",
            "Acesso a rede"});
            this.listBox3.Location = new System.Drawing.Point(18, 66);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(219, 356);
            this.listBox3.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(69, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "EM ANDAMENTO";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.listBox4);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Location = new System.Drawing.Point(1060, 239);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(251, 449);
            this.panel4.TabIndex = 9;
            // 
            // listBox4
            // 
            this.listBox4.FormattingEnabled = true;
            this.listBox4.ItemHeight = 16;
            this.listBox4.Items.AddRange(new object[] {
            "Internet Instável",
            "Reparo Notebook",
            "Instalação de Aplicativo",
            "Acesso a rede"});
            this.listBox4.Location = new System.Drawing.Point(18, 66);
            this.listBox4.Name = "listBox4";
            this.listBox4.Size = new System.Drawing.Size(219, 356);
            this.listBox4.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(69, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "RESOLVIDOS";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(715, 163);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 17);
            this.label6.TabIndex = 10;
            this.label6.Text = "PORTAL";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MenuStrip1);
            this.Name = "Form1";
            this.Text = "TechSolve - Home";
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.MenuStrip MenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem ChamadosDeTIToolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem GerenciarToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem FormuláriosToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem EMailToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem PainelToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.ListBox ListBox1;
        internal System.Windows.Forms.Label Label2;
        private System.Windows.Forms.ToolStripMenuItem tRIAGEMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eMATENDIMENTOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eMATENDIMENTOToolStripMenuItem1;
        internal System.Windows.Forms.Panel panel2;
        internal System.Windows.Forms.ListBox listBox2;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Panel panel3;
        internal System.Windows.Forms.ListBox listBox3;
        internal System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Panel panel4;
        internal System.Windows.Forms.ListBox listBox4;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}

