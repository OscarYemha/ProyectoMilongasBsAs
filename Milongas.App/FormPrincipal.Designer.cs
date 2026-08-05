namespace Milongas.App
{
    partial class FormPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PnlMilongas = new Panel();
            PicMilongas = new PictureBox();
            LblMilongasTitulo = new Label();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();
            panel7 = new Panel();
            PnlMilongas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PicMilongas).BeginInit();
            SuspendLayout();
            // 
            // PnlMilongas
            // 
            PnlMilongas.BackColor = Color.White;
            PnlMilongas.Controls.Add(PicMilongas);
            PnlMilongas.Controls.Add(LblMilongasTitulo);
            PnlMilongas.Location = new Point(76, 73);
            PnlMilongas.Name = "PnlMilongas";
            PnlMilongas.Size = new Size(200, 100);
            PnlMilongas.TabIndex = 0;
            PnlMilongas.Click += PnlMilongas_Click;
            // 
            // PicMilongas
            // 
            PicMilongas.Location = new Point(0, 0);
            PicMilongas.Name = "PicMilongas";
            PicMilongas.Size = new Size(200, 74);
            PicMilongas.SizeMode = PictureBoxSizeMode.Zoom;
            PicMilongas.TabIndex = 1;
            PicMilongas.TabStop = false;
            PicMilongas.Click += PnlMilongas_Click;
            // 
            // LblMilongasTitulo
            // 
            LblMilongasTitulo.AutoSize = true;
            LblMilongasTitulo.Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LblMilongasTitulo.Location = new Point(36, 77);
            LblMilongasTitulo.Name = "LblMilongasTitulo";
            LblMilongasTitulo.Size = new Size(127, 16);
            LblMilongasTitulo.TabIndex = 0;
            LblMilongasTitulo.Text = "Milongas y prácticas";
            LblMilongasTitulo.TextAlign = ContentAlignment.MiddleCenter;
            LblMilongasTitulo.Click += PnlMilongas_Click;
            // 
            // panel2
            // 
            panel2.Location = new Point(332, 73);
            panel2.Name = "panel2";
            panel2.Size = new Size(200, 100);
            panel2.TabIndex = 1;
            // 
            // panel3
            // 
            panel3.Location = new Point(76, 213);
            panel3.Name = "panel3";
            panel3.Size = new Size(200, 100);
            panel3.TabIndex = 2;
            // 
            // panel4
            // 
            panel4.Location = new Point(332, 213);
            panel4.Name = "panel4";
            panel4.Size = new Size(200, 100);
            panel4.TabIndex = 3;
            // 
            // panel5
            // 
            panel5.Location = new Point(76, 350);
            panel5.Name = "panel5";
            panel5.Size = new Size(200, 100);
            panel5.TabIndex = 4;
            // 
            // panel6
            // 
            panel6.Location = new Point(332, 350);
            panel6.Name = "panel6";
            panel6.Size = new Size(200, 100);
            panel6.TabIndex = 5;
            // 
            // panel7
            // 
            panel7.Location = new Point(76, 480);
            panel7.Name = "panel7";
            panel7.Size = new Size(200, 100);
            panel7.TabIndex = 6;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 611);
            Controls.Add(panel7);
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(PnlMilongas);
            Name = "FormPrincipal";
            Text = "FormPrincipal";
            PnlMilongas.ResumeLayout(false);
            PnlMilongas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PicMilongas).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel PnlMilongas;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private Panel panel7;
        private Label LblMilongasTitulo;
        private PictureBox PicMilongas;
    }
}