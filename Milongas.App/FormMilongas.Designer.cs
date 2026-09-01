namespace Milongas.App
{
    partial class FormMilongas
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
            CmbFecha = new ComboBox();
            TxtBuscar = new TextBox();
            CmbBarrio = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            CmbClase = new ComboBox();
            FlpMilongas = new FlowLayoutPanel();
            LblCargando = new Label();
            SuspendLayout();
            // 
            // CmbFecha
            // 
            CmbFecha.FormattingEnabled = true;
            CmbFecha.Location = new Point(58, 41);
            CmbFecha.Name = "CmbFecha";
            CmbFecha.Size = new Size(496, 23);
            CmbFecha.TabIndex = 1;
            CmbFecha.SelectedIndexChanged += TxtBuscar_TextChanged;
            // 
            // TxtBuscar
            // 
            TxtBuscar.Location = new Point(58, 70);
            TxtBuscar.Name = "TxtBuscar";
            TxtBuscar.Size = new Size(496, 23);
            TxtBuscar.TabIndex = 2;
            TxtBuscar.TextChanged += TxtBuscar_TextChanged;
            // 
            // CmbBarrio
            // 
            CmbBarrio.FormattingEnabled = true;
            CmbBarrio.Location = new Point(58, 119);
            CmbBarrio.Name = "CmbBarrio";
            CmbBarrio.Size = new Size(496, 23);
            CmbBarrio.TabIndex = 6;
            CmbBarrio.SelectedIndexChanged += CmbBarrio_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(58, 101);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 7;
            label2.Text = "Barrio:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(58, 160);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 8;
            label3.Text = "Clase:";
            // 
            // CmbClase
            // 
            CmbClase.FormattingEnabled = true;
            CmbClase.Location = new Point(58, 178);
            CmbClase.Name = "CmbClase";
            CmbClase.Size = new Size(496, 23);
            CmbClase.TabIndex = 9;
            CmbClase.SelectedIndexChanged += CmbClase_SelectedIndexChanged;
            // 
            // FlpMilongas
            // 
            FlpMilongas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FlpMilongas.AutoScroll = true;
            FlpMilongas.FlowDirection = FlowDirection.TopDown;
            FlpMilongas.Location = new Point(58, 219);
            FlpMilongas.Name = "FlpMilongas";
            FlpMilongas.Size = new Size(496, 495);
            FlpMilongas.TabIndex = 10;
            FlpMilongas.WrapContents = false;
            // 
            // LblCargando
            // 
            LblCargando.AutoSize = true;
            LblCargando.BackColor = Color.Transparent;
            LblCargando.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LblCargando.ForeColor = Color.DimGray;
            LblCargando.Location = new Point(220,250);
            LblCargando.Name = "LblCargando";
            LblCargando.Size = new Size(171, 21);
            LblCargando.TabIndex = 11;
            LblCargando.Text = "Cargando milongas...";
            LblCargando.TextAlign = ContentAlignment.MiddleCenter;
            LblCargando.Visible = false;
            // 
            // FormMilongas
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 904);
            Controls.Add(LblCargando);
            Controls.Add(FlpMilongas);
            Controls.Add(CmbClase);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(CmbBarrio);
            Controls.Add(TxtBuscar);
            Controls.Add(CmbFecha);
            Name = "FormMilongas";
            Text = "Agenda de Milongas";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ComboBox CmbFecha;
        private TextBox TxtBuscar;
        private ComboBox CmbBarrio;
        private Label label2;
        private Label label3;
        private ComboBox CmbClase;
        private FlowLayoutPanel FlpMilongas;
        private Label LblCargando;
    }
}
