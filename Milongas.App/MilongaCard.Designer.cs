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
            LblModalidadEntrada = new Label();
            LblEventoEspecial = new Label();
            LblCancelada = new Label();
            LblEstado = new Label();
            ((System.ComponentModel.ISupportInitialize)PicImagen).BeginInit();
            SuspendLayout();
            // 
            // PicImagen
            // 
            PicImagen.Location = new Point(10, 22);
            PicImagen.Name = "PicImagen";
            PicImagen.Size = new Size(80, 80);
            PicImagen.SizeMode = PictureBoxSizeMode.Zoom;
            PicImagen.TabIndex = 0;
            PicImagen.TabStop = false;
            // 
            // LblTipo
            // 
            LblTipo.AutoSize = true;
            LblTipo.Font = new Font("Segoe UI", 8F);
            LblTipo.ForeColor = Color.DimGray;
            LblTipo.Location = new Point(96, 12);
            LblTipo.Name = "LblTipo";
            LblTipo.Size = new Size(0, 13);
            LblTipo.TabIndex = 7;
            // 
            // LblNombre
            // 
            LblNombre.AutoSize = true;
            LblNombre.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            LblNombre.Location = new Point(96, 16);
            LblNombre.Name = "LblNombre";
            LblNombre.Size = new Size(0, 21);
            LblNombre.TabIndex = 6;
            // 
            // LblHorario
            // 
            LblHorario.AutoSize = true;
            LblHorario.Location = new Point(96, 28);
            LblHorario.Name = "LblHorario";
            LblHorario.Size = new Size(38, 15);
            LblHorario.TabIndex = 3;
            LblHorario.Text = "label3";
            // 
            // LblUbicacion
            // 
            LblUbicacion.AutoSize = true;
            LblUbicacion.Location = new Point(96, 43);
            LblUbicacion.Name = "LblUbicacion";
            LblUbicacion.Size = new Size(38, 15);
            LblUbicacion.TabIndex = 4;
            LblUbicacion.Text = "label4";
            // 
            // LblClaseDistancia
            // 
            LblClaseDistancia.AutoSize = true;
            LblClaseDistancia.Location = new Point(96, 58);
            LblClaseDistancia.Name = "LblClaseDistancia";
            LblClaseDistancia.Size = new Size(38, 15);
            LblClaseDistancia.TabIndex = 5;
            LblClaseDistancia.Text = "label5";
            // 
            // LblModalidadEntrada
            // 
            LblModalidadEntrada.AutoSize = true;
            LblModalidadEntrada.Location = new Point(0, 0);
            LblModalidadEntrada.Name = "LblModalidadEntrada";
            LblModalidadEntrada.Size = new Size(38, 15);
            LblModalidadEntrada.TabIndex = 6;
            LblModalidadEntrada.Text = "label1";
            // 
            // LblEventoEspecial
            // 
            LblEventoEspecial.AutoSize = true;
            LblEventoEspecial.Location = new Point(0, 0);
            LblEventoEspecial.Name = "LblEventoEspecial";
            LblEventoEspecial.Size = new Size(38, 15);
            LblEventoEspecial.TabIndex = 7;
            LblEventoEspecial.Text = "label2";
            // 
            // LblCancelada
            // 
            LblCancelada.AutoSize = true;
            LblCancelada.BackColor = Color.Firebrick;
            LblCancelada.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            LblCancelada.ForeColor = Color.White;
            LblCancelada.Location = new Point(300, 22);
            LblCancelada.Name = "LblCancelada";
            LblCancelada.Padding = new Padding(6, 3, 6, 3);
            LblCancelada.Size = new Size(88, 21);
            LblCancelada.TabIndex = 9;
            LblCancelada.Text = "CANCELADO";
            LblCancelada.Visible = false;
            // 
            // LblEstado
            // 
            LblEstado.AutoSize = true;
            LblEstado.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            LblEstado.Location = new Point(96, 53);
            LblEstado.Name = "LblEstado";
            LblEstado.Padding = new Padding(5, 2, 5, 2);
            LblEstado.Size = new Size(10, 17);
            LblEstado.TabIndex = 10;
            LblEstado.Visible = false;
            // 
            // MilongaCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(PicImagen);
            Controls.Add(LblTipo);
            Controls.Add(LblNombre);
            Controls.Add(LblHorario);
            Controls.Add(LblUbicacion);
            Controls.Add(LblClaseDistancia);
            Controls.Add(LblModalidadEntrada);
            Controls.Add(LblEventoEspecial);
            Controls.Add(LblCancelada);
            Controls.Add(LblEstado);
            Name = "MilongaCard";
            Size = new Size(420, 125);
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
        private Label LblModalidadEntrada;
        private Label LblEventoEspecial;
        private Label LblCancelada;
        private Label LblEstado;
    }
}
