namespace Milongas.App
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BtnCargar = new Button();
            CmbFecha = new ComboBox();
            TxtBuscar = new TextBox();
            DgvMilongas = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)DgvMilongas).BeginInit();
            SuspendLayout();
            // 
            // BtnCargar
            // 
            BtnCargar.Location = new Point(193, 69);
            BtnCargar.Name = "BtnCargar";
            BtnCargar.Size = new Size(100, 23);
            BtnCargar.TabIndex = 0;
            BtnCargar.Text = "Cargar agenda";
            BtnCargar.UseVisualStyleBackColor = true;
            BtnCargar.Click += BtnCargar_Click;
            // 
            // CmbFecha
            // 
            CmbFecha.FormattingEnabled = true;
            CmbFecha.Location = new Point(58, 126);
            CmbFecha.Name = "CmbFecha";
            CmbFecha.Size = new Size(387, 23);
            CmbFecha.TabIndex = 1;
            CmbFecha.SelectedIndexChanged += TxtBuscar_TextChanged;
            // 
            // TxtBuscar
            // 
            TxtBuscar.Location = new Point(58, 188);
            TxtBuscar.Name = "TxtBuscar";
            TxtBuscar.Size = new Size(387, 23);
            TxtBuscar.TabIndex = 2;
            TxtBuscar.TextChanged += TxtBuscar_TextChanged;
            // 
            // DgvMilongas
            // 
            DgvMilongas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvMilongas.Location = new Point(58, 250);
            DgvMilongas.Name = "DgvMilongas";
            DgvMilongas.Size = new Size(387, 188);
            DgvMilongas.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(DgvMilongas);
            Controls.Add(TxtBuscar);
            Controls.Add(CmbFecha);
            Controls.Add(BtnCargar);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)DgvMilongas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnCargar;
        private ComboBox CmbFecha;
        private TextBox TxtBuscar;
        private DataGridView DgvMilongas;
    }
}
