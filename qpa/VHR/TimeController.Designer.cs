
namespace VHR
{
    partial class TimeController
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
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblTitDesde = new System.Windows.Forms.Label();
            this.trackHoraDesde = new System.Windows.Forms.TrackBar();
            this.trackHoraHasta = new System.Windows.Forms.TrackBar();
            this.lblHoraDesde = new System.Windows.Forms.Label();
            this.lblHoraHasta = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFicha = new System.Windows.Forms.TextBox();
            this.txtLineas = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbLineas = new System.Windows.Forms.ComboBox();
            this.btnCargarPuntos = new System.Windows.Forms.Button();
            this.lstConsola = new System.Windows.Forms.ListBox();
            this.btnReintentar = new System.Windows.Forms.Button();
            this.lblErrorLineas = new System.Windows.Forms.Label();
            this.lblFlechaErrorLineas = new System.Windows.Forms.Label();
            this.lblFlechaErrorFicha = new System.Windows.Forms.Label();
            this.lblErrorFicha = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackHoraDesde)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHoraHasta)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpDesde
            // 
            this.dtpDesde.CalendarMonthBackground = System.Drawing.Color.Silver;
            this.dtpDesde.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDesde.Location = new System.Drawing.Point(96, 70);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(394, 26);
            this.dtpDesde.TabIndex = 3;
            // 
            // lblTitDesde
            // 
            this.lblTitDesde.AutoSize = true;
            this.lblTitDesde.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitDesde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblTitDesde.Location = new System.Drawing.Point(6, 73);
            this.lblTitDesde.Name = "lblTitDesde";
            this.lblTitDesde.Size = new System.Drawing.Size(56, 20);
            this.lblTitDesde.TabIndex = 1;
            this.lblTitDesde.Text = "&Desde";
            // 
            // trackHoraDesde
            // 
            this.trackHoraDesde.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackHoraDesde.Location = new System.Drawing.Point(88, 103);
            this.trackHoraDesde.Maximum = 1440;
            this.trackHoraDesde.Name = "trackHoraDesde";
            this.trackHoraDesde.Size = new System.Drawing.Size(472, 45);
            this.trackHoraDesde.TabIndex = 4;
            this.trackHoraDesde.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackHoraDesde.ValueChanged += new System.EventHandler(this.TrackHoraDesde_ValueChanged);
            // 
            // trackHoraHasta
            // 
            this.trackHoraHasta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackHoraHasta.LargeChange = 1;
            this.trackHoraHasta.Location = new System.Drawing.Point(88, 134);
            this.trackHoraHasta.Maximum = 1440;
            this.trackHoraHasta.Name = "trackHoraHasta";
            this.trackHoraHasta.Size = new System.Drawing.Size(472, 45);
            this.trackHoraHasta.TabIndex = 5;
            this.trackHoraHasta.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackHoraHasta.Value = 1440;
            this.trackHoraHasta.ValueChanged += new System.EventHandler(this.TrackHoraHasta_ValueChanged);
            // 
            // lblHoraDesde
            // 
            this.lblHoraDesde.AutoSize = true;
            this.lblHoraDesde.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoraDesde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblHoraDesde.Location = new System.Drawing.Point(6, 104);
            this.lblHoraDesde.Name = "lblHoraDesde";
            this.lblHoraDesde.Size = new System.Drawing.Size(71, 20);
            this.lblHoraDesde.TabIndex = 4;
            this.lblHoraDesde.Text = "00:00:00";
            // 
            // lblHoraHasta
            // 
            this.lblHoraHasta.AutoSize = true;
            this.lblHoraHasta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoraHasta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblHoraHasta.Location = new System.Drawing.Point(6, 134);
            this.lblHoraHasta.Name = "lblHoraHasta";
            this.lblHoraHasta.Size = new System.Drawing.Size(71, 20);
            this.lblHoraHasta.TabIndex = 5;
            this.lblHoraHasta.Text = "00:00:00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "&Ficha";
            // 
            // txtFicha
            // 
            this.txtFicha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFicha.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFicha.Location = new System.Drawing.Point(96, 6);
            this.txtFicha.Name = "txtFicha";
            this.txtFicha.Size = new System.Drawing.Size(195, 26);
            this.txtFicha.TabIndex = 0;
            // 
            // txtLineas
            // 
            this.txtLineas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLineas.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLineas.Location = new System.Drawing.Point(96, 38);
            this.txtLineas.Name = "txtLineas";
            this.txtLineas.Size = new System.Drawing.Size(195, 26);
            this.txtLineas.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label2.Location = new System.Drawing.Point(6, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "&Líneas";
            // 
            // cmbLineas
            // 
            this.cmbLineas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLineas.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbLineas.FormattingEnabled = true;
            this.cmbLineas.Location = new System.Drawing.Point(95, 37);
            this.cmbLineas.Name = "cmbLineas";
            this.cmbLineas.Size = new System.Drawing.Size(224, 28);
            this.cmbLineas.TabIndex = 2;
            // 
            // btnCargarPuntos
            // 
            this.btnCargarPuntos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCargarPuntos.FlatAppearance.BorderSize = 0;
            this.btnCargarPuntos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargarPuntos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarPuntos.Location = new System.Drawing.Point(10, 177);
            this.btnCargarPuntos.Name = "btnCargarPuntos";
            this.btnCargarPuntos.Size = new System.Drawing.Size(281, 40);
            this.btnCargarPuntos.TabIndex = 20;
            this.btnCargarPuntos.Text = "&Cargar datos y Analizar";
            this.btnCargarPuntos.UseVisualStyleBackColor = false;
            this.btnCargarPuntos.Click += new System.EventHandler(this.BtnCargarPuntos_Click);
            // 
            // lstConsola
            // 
            this.lstConsola.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstConsola.BackColor = System.Drawing.Color.Black;
            this.lstConsola.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstConsola.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstConsola.ForeColor = System.Drawing.Color.Silver;
            this.lstConsola.FormattingEnabled = true;
            this.lstConsola.ItemHeight = 18;
            this.lstConsola.Location = new System.Drawing.Point(10, 227);
            this.lstConsola.Name = "lstConsola";
            this.lstConsola.Size = new System.Drawing.Size(574, 234);
            this.lstConsola.TabIndex = 21;
            // 
            // btnReintentar
            // 
            this.btnReintentar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnReintentar.FlatAppearance.BorderSize = 0;
            this.btnReintentar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReintentar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReintentar.Location = new System.Drawing.Point(10, 472);
            this.btnReintentar.Name = "btnReintentar";
            this.btnReintentar.Size = new System.Drawing.Size(281, 40);
            this.btnReintentar.TabIndex = 22;
            this.btnReintentar.Text = "Reintentar";
            this.btnReintentar.UseVisualStyleBackColor = false;
            this.btnReintentar.Click += new System.EventHandler(this.BtnReintentar_Click);
            // 
            // lblErrorLineas
            // 
            this.lblErrorLineas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblErrorLineas.Location = new System.Drawing.Point(348, 38);
            this.lblErrorLineas.Name = "lblErrorLineas";
            this.lblErrorLineas.Size = new System.Drawing.Size(142, 26);
            this.lblErrorLineas.TabIndex = 24;
            this.lblErrorLineas.Text = "Ingrese uno o mas números separados por coma";
            // 
            // lblFlechaErrorLineas
            // 
            this.lblFlechaErrorLineas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblFlechaErrorLineas.ForeColor = System.Drawing.Color.Black;
            this.lblFlechaErrorLineas.Location = new System.Drawing.Point(325, 38);
            this.lblFlechaErrorLineas.Name = "lblFlechaErrorLineas";
            this.lblFlechaErrorLineas.Size = new System.Drawing.Size(25, 26);
            this.lblFlechaErrorLineas.TabIndex = 25;
            this.lblFlechaErrorLineas.Text = "<<";
            // 
            // lblFlechaErrorFicha
            // 
            this.lblFlechaErrorFicha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblFlechaErrorFicha.ForeColor = System.Drawing.Color.Black;
            this.lblFlechaErrorFicha.Location = new System.Drawing.Point(325, 6);
            this.lblFlechaErrorFicha.Name = "lblFlechaErrorFicha";
            this.lblFlechaErrorFicha.Size = new System.Drawing.Size(25, 26);
            this.lblFlechaErrorFicha.TabIndex = 27;
            this.lblFlechaErrorFicha.Text = "<<";
            // 
            // lblErrorFicha
            // 
            this.lblErrorFicha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblErrorFicha.Location = new System.Drawing.Point(348, 6);
            this.lblErrorFicha.Name = "lblErrorFicha";
            this.lblErrorFicha.Size = new System.Drawing.Size(142, 26);
            this.lblErrorFicha.TabIndex = 26;
            this.lblErrorFicha.Text = "Ingrese uno o mas números separados por coma";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(566, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 33);
            this.label3.TabIndex = 28;
            this.label3.Text = "<<";
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(566, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 26);
            this.label4.TabIndex = 29;
            this.label4.Text = "<<";
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label5.Location = new System.Drawing.Point(496, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 90);
            this.label5.TabIndex = 30;
            this.label5.Text = "0 minutos solicitados";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label5.Visible = false;
            // 
            // TimeController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCargarPuntos);
            this.Controls.Add(this.lblFlechaErrorFicha);
            this.Controls.Add(this.lblErrorFicha);
            this.Controls.Add(this.lblFlechaErrorLineas);
            this.Controls.Add(this.lblErrorLineas);
            this.Controls.Add(this.btnReintentar);
            this.Controls.Add(this.lstConsola);
            this.Controls.Add(this.txtLineas);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFicha);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblHoraHasta);
            this.Controls.Add(this.lblHoraDesde);
            this.Controls.Add(this.trackHoraHasta);
            this.Controls.Add(this.trackHoraDesde);
            this.Controls.Add(this.lblTitDesde);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.cmbLineas);
            this.MinimumSize = new System.Drawing.Size(475, 105);
            this.Name = "TimeController";
            this.Size = new System.Drawing.Size(594, 522);
            ((System.ComponentModel.ISupportInitialize)(this.trackHoraDesde)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHoraHasta)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblTitDesde;
        private System.Windows.Forms.TrackBar trackHoraDesde;
        private System.Windows.Forms.TrackBar trackHoraHasta;
        private System.Windows.Forms.Label lblHoraDesde;
        private System.Windows.Forms.Label lblHoraHasta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFicha;
        private System.Windows.Forms.TextBox txtLineas;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbLineas;
        private System.Windows.Forms.Button btnCargarPuntos;
        private System.Windows.Forms.ListBox lstConsola;
        private System.Windows.Forms.Button btnReintentar;
        private System.Windows.Forms.Label lblErrorLineas;
        private System.Windows.Forms.Label lblFlechaErrorLineas;
        private System.Windows.Forms.Label lblFlechaErrorFicha;
        private System.Windows.Forms.Label lblErrorFicha;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
