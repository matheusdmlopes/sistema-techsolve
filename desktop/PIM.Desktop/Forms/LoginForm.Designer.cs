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
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.Button1 = new System.Windows.Forms.Button();
            this.LinkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(761, 287);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(66, 20);
            this.Label1.TabIndex = 7;
            this.Label1.Text = "LOGIN";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(557, 343);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(50, 16);
            this.Label2.TabIndex = 6;
            this.Label2.Text = "E-MAIL";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(557, 432);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(54, 16);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "SENHA";
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(554, 374);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(480, 22);
            this.TextBox1.TabIndex = 9;
            // 
            // TextBox2
            // 
            this.TextBox2.Location = new System.Drawing.Point(554, 467);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.Size = new System.Drawing.Size(480, 22);
            this.TextBox2.TabIndex = 8;
            // 
            // Button1
            // 
            this.Button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button1.Location = new System.Drawing.Point(743, 508);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(102, 43);
            this.Button1.TabIndex = 10;
            this.Button1.Text = "Entrar";
            this.Button1.UseVisualStyleBackColor = true;
            // 
            // LinkLabel1
            // 
            this.LinkLabel1.ActiveLinkColor = System.Drawing.Color.Gray;
            this.LinkLabel1.AutoSize = true;
            this.LinkLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LinkLabel1.LinkColor = System.Drawing.SystemColors.WindowFrame;
            this.LinkLabel1.Location = new System.Drawing.Point(693, 554);
            this.LinkLabel1.Name = "LinkLabel1";
            this.LinkLabel1.Size = new System.Drawing.Size(195, 16);
            this.LinkLabel1.TabIndex = 11;
            this.LinkLabel1.TabStop = true;
            this.LinkLabel1.Text = "esqueceu a senha? Clique aqui";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LinkLabel1);
            this.panel1.Controls.Add(this.Button1);
            this.panel1.Controls.Add(this.TextBox2);
            this.panel1.Controls.Add(this.TextBox1);
            this.panel1.Controls.Add(this.Label3);
            this.panel1.Controls.Add(this.Label2);
            this.panel1.Controls.Add(this.Label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1902, 1033);
            this.panel1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "TechSolve";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox TextBox1;
        internal System.Windows.Forms.TextBox TextBox2;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.LinkLabel LinkLabel1;
        private System.Windows.Forms.Panel panel1;
    }
}

