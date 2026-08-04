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
            CmbOrden = new ComboBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)DgvMilongas).BeginInit();
            SuspendLayout();
            // 
            // BtnCargar
            // 
            BtnCargar.Location = new Point(194, 29);
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
            CmbFecha.Location = new Point(58, 79);
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
            // CmbOrden
            // 
            CmbOrden.FormattingEnabled = true;
            CmbOrden.Location = new Point(58, 144);
            CmbOrden.Name = "CmbOrden";
            CmbOrden.Size = new Size(387, 23);
            CmbOrden.TabIndex = 4;
            CmbOrden.SelectedIndexChanged += CmbOrden_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(58, 126);
            label1.Name = "label1";
            label1.Size = new Size(74, 15);
            label1.TabIndex = 5;
            label1.Text = "Ordenar por:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(CmbOrden);
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
        private ComboBox CmbOrden;
        private Label label1;
    }
}
