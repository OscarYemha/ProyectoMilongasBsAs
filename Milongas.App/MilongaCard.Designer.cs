namespace Milongas.App
{
    partial class MilongaCard
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            PicImagen = new PictureBox();
            LblTipo = new Label();
            LblNombre = new Label();
            LblHorario = new Label();
            LblUbicacion = new Label();
            LblClaseDistancia = new Label();
            ((System.ComponentModel.ISupportInitialize)PicImagen).BeginInit();
            SuspendLayout();
            // 
            // PicImagen
            // 
            PicImagen.Location = new Point(10, 12);
            PicImagen.Name = "PicImagen";
            PicImagen.Size = new Size(80, 80);
            PicImagen.SizeMode = PictureBoxSizeMode.Zoom;
            PicImagen.TabIndex = 0;
            PicImagen.TabStop = false;
            // 
            // LblTipo
            // 
            LblTipo.AutoSize = true;
            LblTipo.Location = new Point(96, 12);
            LblTipo.Name = "LblTipo";
            LblTipo.Size = new Size(38, 15);
            LblTipo.TabIndex = 1;
            LblTipo.Text = "label1";
            // 
            // LblNombre
            // 
            LblNombre.AutoSize = true;
            LblNombre.Location = new Point(96, 32);
            LblNombre.Name = "LblNombre";
            LblNombre.Size = new Size(38, 15);
            LblNombre.TabIndex = 2;
            LblNombre.Text = "label2";
            // 
            // LblHorario
            // 
            LblHorario.AutoSize = true;
            LblHorario.Location = new Point(96, 47);
            LblHorario.Name = "LblHorario";
            LblHorario.Size = new Size(38, 15);
            LblHorario.TabIndex = 3;
            LblHorario.Text = "label3";
            // 
            // LblUbicacion
            // 
            LblUbicacion.AutoSize = true;
            LblUbicacion.Location = new Point(96, 62);
            LblUbicacion.Name = "LblUbicacion";
            LblUbicacion.Size = new Size(38, 15);
            LblUbicacion.TabIndex = 4;
            LblUbicacion.Text = "label4";
            // 
            // LblClaseDistancia
            // 
            LblClaseDistancia.AutoSize = true;
            LblClaseDistancia.Location = new Point(96, 77);
            LblClaseDistancia.Name = "LblClaseDistancia";
            LblClaseDistancia.Size = new Size(38, 15);
            LblClaseDistancia.TabIndex = 5;
            LblClaseDistancia.Text = "label5";
            // 
            // MilongaCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 224, 192);
            Controls.Add(LblClaseDistancia);
            Controls.Add(LblUbicacion);
            Controls.Add(LblHorario);
            Controls.Add(LblNombre);
            Controls.Add(LblTipo);
            Controls.Add(PicImagen);
            Name = "MilongaCard";
            Size = new Size(420, 105);
            ((System.ComponentModel.ISupportInitialize)PicImagen).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox PicImagen;
        private Label LblTipo;
        private Label LblNombre;
        private Label LblHorario;
        private Label LblUbicacion;
        private Label LblClaseDistancia;
    }
}
