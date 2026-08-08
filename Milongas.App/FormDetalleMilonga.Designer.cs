namespace Milongas.App
{
    partial class FormDetalleMilonga
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
            PicImagen = new PictureBox();
            LblTipo = new Label();
            LblNombre = new Label();
            LblEstado = new Label();
            LblOrganizadores = new Label();
            LblHorario = new Label();
            LblSalon = new Label();
            LblDireccion = new Label();
            LblBarrio = new Label();
            LblClase = new Label();
            LblDistancia = new Label();
            LblReserva = new Label();
            LblDescripcion = new Label();
            PicFoto = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PicImagen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PicFoto).BeginInit();
            SuspendLayout();
            // 
            // PicImagen
            // 
            PicImagen.Location = new Point(183, 24);
            PicImagen.Name = "PicImagen";
            PicImagen.Size = new Size(134, 80);
            PicImagen.SizeMode = PictureBoxSizeMode.Zoom;
            PicImagen.TabIndex = 0;
            PicImagen.TabStop = false;
            // 
            // LblTipo
            // 
            LblTipo.AutoSize = true;
            LblTipo.Location = new Point(183, 107);
            LblTipo.Name = "LblTipo";
            LblTipo.Size = new Size(38, 15);
            LblTipo.TabIndex = 1;
            LblTipo.Text = "label1";
            // 
            // LblNombre
            // 
            LblNombre.AutoSize = true;
            LblNombre.Location = new Point(183, 122);
            LblNombre.Name = "LblNombre";
            LblNombre.Size = new Size(38, 15);
            LblNombre.TabIndex = 2;
            LblNombre.Text = "label2";
            // 
            // LblEstado
            // 
            LblEstado.AutoSize = true;
            LblEstado.Location = new Point(183, 138);
            LblEstado.Name = "LblEstado";
            LblEstado.Size = new Size(38, 15);
            LblEstado.TabIndex = 3;
            LblEstado.Text = "label3";
            // 
            // LblOrganizadores
            // 
            LblOrganizadores.AutoSize = true;
            LblOrganizadores.Location = new Point(183, 228);
            LblOrganizadores.Name = "LblOrganizadores";
            LblOrganizadores.Size = new Size(38, 15);
            LblOrganizadores.TabIndex = 4;
            LblOrganizadores.Text = "label4";
            // 
            // LblHorario
            // 
            LblHorario.AutoSize = true;
            LblHorario.Location = new Point(183, 153);
            LblHorario.Name = "LblHorario";
            LblHorario.Size = new Size(38, 15);
            LblHorario.TabIndex = 5;
            LblHorario.Text = "label5";
            // 
            // LblSalon
            // 
            LblSalon.AutoSize = true;
            LblSalon.Location = new Point(183, 168);
            LblSalon.Name = "LblSalon";
            LblSalon.Size = new Size(38, 15);
            LblSalon.TabIndex = 6;
            LblSalon.Text = "label6";
            // 
            // LblDireccion
            // 
            LblDireccion.AutoSize = true;
            LblDireccion.Location = new Point(183, 183);
            LblDireccion.Name = "LblDireccion";
            LblDireccion.Size = new Size(38, 15);
            LblDireccion.TabIndex = 7;
            LblDireccion.Text = "label7";
            // 
            // LblBarrio
            // 
            LblBarrio.AutoSize = true;
            LblBarrio.Location = new Point(183, 198);
            LblBarrio.Name = "LblBarrio";
            LblBarrio.Size = new Size(38, 15);
            LblBarrio.TabIndex = 8;
            LblBarrio.Text = "label8";
            // 
            // LblClase
            // 
            LblClase.AutoSize = true;
            LblClase.Location = new Point(183, 213);
            LblClase.Name = "LblClase";
            LblClase.Size = new Size(38, 15);
            LblClase.TabIndex = 9;
            LblClase.Text = "label9";
            // 
            // LblDistancia
            // 
            LblDistancia.AutoSize = true;
            LblDistancia.Location = new Point(183, 243);
            LblDistancia.Name = "LblDistancia";
            LblDistancia.Size = new Size(44, 15);
            LblDistancia.TabIndex = 10;
            LblDistancia.Text = "label10";
            // 
            // LblReserva
            // 
            LblReserva.AutoSize = true;
            LblReserva.Location = new Point(183, 258);
            LblReserva.Name = "LblReserva";
            LblReserva.Size = new Size(44, 15);
            LblReserva.TabIndex = 11;
            LblReserva.Text = "label11";
            // 
            // LblDescripcion
            // 
            LblDescripcion.AutoSize = true;
            LblDescripcion.Location = new Point(183, 273);
            LblDescripcion.Name = "LblDescripcion";
            LblDescripcion.Size = new Size(44, 15);
            LblDescripcion.TabIndex = 12;
            LblDescripcion.Text = "label12";
            // 
            // PicFoto
            // 
            PicFoto.Location = new Point(183, 291);
            PicFoto.Name = "PicFoto";
            PicFoto.Size = new Size(206, 123);
            PicFoto.SizeMode = PictureBoxSizeMode.Zoom;
            PicFoto.TabIndex = 13;
            PicFoto.TabStop = false;
            // 
            // FormDetalleMilonga
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(PicFoto);
            Controls.Add(LblDescripcion);
            Controls.Add(LblReserva);
            Controls.Add(LblDistancia);
            Controls.Add(LblClase);
            Controls.Add(LblBarrio);
            Controls.Add(LblDireccion);
            Controls.Add(LblSalon);
            Controls.Add(LblHorario);
            Controls.Add(LblOrganizadores);
            Controls.Add(LblEstado);
            Controls.Add(LblNombre);
            Controls.Add(LblTipo);
            Controls.Add(PicImagen);
            Name = "FormDetalleMilonga";
            Text = "FormDetalleMilonga";
            ((System.ComponentModel.ISupportInitialize)PicImagen).EndInit();
            ((System.ComponentModel.ISupportInitialize)PicFoto).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox PicImagen;
        private Label LblTipo;
        private Label LblNombre;
        private Label LblEstado;
        private Label LblOrganizadores;
        private Label LblHorario;
        private Label LblSalon;
        private Label LblDireccion;
        private Label LblBarrio;
        private Label LblClase;
        private Label LblDistancia;
        private Label LblReserva;
        private Label LblDescripcion;
        private PictureBox PicFoto;
    }
}