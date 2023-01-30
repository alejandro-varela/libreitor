
namespace VHR
{
    partial class FrmMain
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

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.picMain = new System.Windows.Forms.PictureBox();
            this.panMediaController = new System.Windows.Forms.Panel();
            this.cmbLargoSubRango = new System.Windows.Forms.ComboBox();
            this.chkCentrar = new System.Windows.Forms.CheckBox();
            this.lblFechaActual = new System.Windows.Forms.Label();
            this.picActividad = new System.Windows.Forms.PictureBox();
            this.chkVerHoraEnCruz = new System.Windows.Forms.CheckBox();
            this.chkVerCruz = new System.Windows.Forms.CheckBox();
            this.btnMinus1 = new System.Windows.Forms.Button();
            this.btnPlus1 = new System.Windows.Forms.Button();
            this.btnPlus10 = new System.Windows.Forms.Button();
            this.btnMinus10 = new System.Windows.Forms.Button();
            this.lblHoraActual = new System.Windows.Forms.Label();
            this.btnPlus30 = new System.Windows.Forms.Button();
            this.btnMinus30 = new System.Windows.Forms.Button();
            this.lblFondoCeleste = new System.Windows.Forms.Label();
            this.panSidePanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitOpacidadIntervaloFijo = new System.Windows.Forms.Label();
            this.scrollOpacidadIntervaloFijo = new System.Windows.Forms.HScrollBar();
            this.lblColorIntervaloFijo = new System.Windows.Forms.Label();
            this.chkIntervaloFijo = new System.Windows.Forms.CheckBox();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnShowHideSidePanel = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lvwRecos = new System.Windows.Forms.ListView();
            this.chBan = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chEstilo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panBasicData = new System.Windows.Forms.Panel();
            this.dtpHoraDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpHoraHasta = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.lblTitHasta = new System.Windows.Forms.Label();
            this.btnAceptarDatosBasicos = new System.Windows.Forms.Button();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.txtFichas = new System.Windows.Forms.TextBox();
            this.txtLineas = new System.Windows.Forms.TextBox();
            this.cmbLineas = new System.Windows.Forms.ComboBox();
            this.cmbModoHistorico = new System.Windows.Forms.ComboBox();
            this.lblTitDesde = new System.Windows.Forms.Label();
            this.lblTitModoHistorico = new System.Windows.Forms.Label();
            this.lblTitLineas = new System.Windows.Forms.Label();
            this.lblTitFicha = new System.Windows.Forms.Label();
            this.basicDataController = new VHR.TimeController();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            this.panMediaController.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picActividad)).BeginInit();
            this.panSidePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panBasicData.SuspendLayout();
            this.SuspendLayout();
            // 
            // picMain
            // 
            this.picMain.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.picMain, "picMain");
            this.picMain.Name = "picMain";
            this.picMain.TabStop = false;
            // 
            // panMediaController
            // 
            this.panMediaController.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panMediaController.Controls.Add(this.cmbLargoSubRango);
            this.panMediaController.Controls.Add(this.chkCentrar);
            this.panMediaController.Controls.Add(this.lblFechaActual);
            this.panMediaController.Controls.Add(this.picActividad);
            this.panMediaController.Controls.Add(this.chkVerHoraEnCruz);
            this.panMediaController.Controls.Add(this.chkVerCruz);
            this.panMediaController.Controls.Add(this.btnMinus1);
            this.panMediaController.Controls.Add(this.btnPlus1);
            this.panMediaController.Controls.Add(this.btnPlus10);
            this.panMediaController.Controls.Add(this.btnMinus10);
            this.panMediaController.Controls.Add(this.lblHoraActual);
            this.panMediaController.Controls.Add(this.btnPlus30);
            this.panMediaController.Controls.Add(this.btnMinus30);
            this.panMediaController.Controls.Add(this.lblFondoCeleste);
            resources.ApplyResources(this.panMediaController, "panMediaController");
            this.panMediaController.Name = "panMediaController";
            // 
            // cmbLargoSubRango
            // 
            this.cmbLargoSubRango.BackColor = System.Drawing.Color.Aqua;
            this.cmbLargoSubRango.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbLargoSubRango, "cmbLargoSubRango");
            this.cmbLargoSubRango.FormattingEnabled = true;
            this.cmbLargoSubRango.Items.AddRange(new object[] {
            resources.GetString("cmbLargoSubRango.Items"),
            resources.GetString("cmbLargoSubRango.Items1"),
            resources.GetString("cmbLargoSubRango.Items2"),
            resources.GetString("cmbLargoSubRango.Items3"),
            resources.GetString("cmbLargoSubRango.Items4"),
            resources.GetString("cmbLargoSubRango.Items5"),
            resources.GetString("cmbLargoSubRango.Items6"),
            resources.GetString("cmbLargoSubRango.Items7"),
            resources.GetString("cmbLargoSubRango.Items8"),
            resources.GetString("cmbLargoSubRango.Items9"),
            resources.GetString("cmbLargoSubRango.Items10"),
            resources.GetString("cmbLargoSubRango.Items11"),
            resources.GetString("cmbLargoSubRango.Items12"),
            resources.GetString("cmbLargoSubRango.Items13"),
            resources.GetString("cmbLargoSubRango.Items14"),
            resources.GetString("cmbLargoSubRango.Items15"),
            resources.GetString("cmbLargoSubRango.Items16"),
            resources.GetString("cmbLargoSubRango.Items17"),
            resources.GetString("cmbLargoSubRango.Items18"),
            resources.GetString("cmbLargoSubRango.Items19"),
            resources.GetString("cmbLargoSubRango.Items20"),
            resources.GetString("cmbLargoSubRango.Items21"),
            resources.GetString("cmbLargoSubRango.Items22"),
            resources.GetString("cmbLargoSubRango.Items23"),
            resources.GetString("cmbLargoSubRango.Items24"),
            resources.GetString("cmbLargoSubRango.Items25"),
            resources.GetString("cmbLargoSubRango.Items26")});
            this.cmbLargoSubRango.Name = "cmbLargoSubRango";
            this.cmbLargoSubRango.SelectedIndexChanged += new System.EventHandler(this.CmbLargoSubRango_SelectedIndexChanged);
            // 
            // chkCentrar
            // 
            resources.ApplyResources(this.chkCentrar, "chkCentrar");
            this.chkCentrar.ForeColor = System.Drawing.Color.Silver;
            this.chkCentrar.Name = "chkCentrar";
            this.chkCentrar.UseVisualStyleBackColor = true;
            this.chkCentrar.CheckedChanged += new System.EventHandler(this.ChkCentrar_CheckedChanged);
            // 
            // lblFechaActual
            // 
            resources.ApplyResources(this.lblFechaActual, "lblFechaActual");
            this.lblFechaActual.ForeColor = System.Drawing.Color.Cyan;
            this.lblFechaActual.Name = "lblFechaActual";
            // 
            // picActividad
            // 
            this.picActividad.BackColor = System.Drawing.Color.Black;
            this.picActividad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.picActividad, "picActividad");
            this.picActividad.Name = "picActividad";
            this.picActividad.TabStop = false;
            // 
            // chkVerHoraEnCruz
            // 
            resources.ApplyResources(this.chkVerHoraEnCruz, "chkVerHoraEnCruz");
            this.chkVerHoraEnCruz.ForeColor = System.Drawing.Color.Silver;
            this.chkVerHoraEnCruz.Name = "chkVerHoraEnCruz";
            this.chkVerHoraEnCruz.UseVisualStyleBackColor = true;
            this.chkVerHoraEnCruz.CheckedChanged += new System.EventHandler(this.ChkVerHoraEnCruz_CheckedChanged);
            // 
            // chkVerCruz
            // 
            resources.ApplyResources(this.chkVerCruz, "chkVerCruz");
            this.chkVerCruz.Checked = true;
            this.chkVerCruz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVerCruz.ForeColor = System.Drawing.Color.Silver;
            this.chkVerCruz.Name = "chkVerCruz";
            this.chkVerCruz.UseVisualStyleBackColor = true;
            this.chkVerCruz.CheckedChanged += new System.EventHandler(this.ChkVerCruz_CheckedChanged);
            // 
            // btnMinus1
            // 
            this.btnMinus1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnMinus1.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnMinus1, "btnMinus1");
            this.btnMinus1.ForeColor = System.Drawing.Color.Cyan;
            this.btnMinus1.Name = "btnMinus1";
            this.btnMinus1.UseVisualStyleBackColor = false;
            this.btnMinus1.Click += new System.EventHandler(this.BtnMinus1_Click);
            // 
            // btnPlus1
            // 
            this.btnPlus1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnPlus1.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnPlus1, "btnPlus1");
            this.btnPlus1.ForeColor = System.Drawing.Color.Cyan;
            this.btnPlus1.Name = "btnPlus1";
            this.btnPlus1.UseVisualStyleBackColor = false;
            this.btnPlus1.Click += new System.EventHandler(this.BtnPlus1_Click);
            // 
            // btnPlus10
            // 
            this.btnPlus10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnPlus10.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnPlus10, "btnPlus10");
            this.btnPlus10.ForeColor = System.Drawing.Color.Cyan;
            this.btnPlus10.Name = "btnPlus10";
            this.btnPlus10.UseVisualStyleBackColor = false;
            this.btnPlus10.Click += new System.EventHandler(this.BtnPlus10_Click);
            // 
            // btnMinus10
            // 
            this.btnMinus10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnMinus10.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnMinus10, "btnMinus10");
            this.btnMinus10.ForeColor = System.Drawing.Color.Cyan;
            this.btnMinus10.Name = "btnMinus10";
            this.btnMinus10.UseVisualStyleBackColor = false;
            this.btnMinus10.Click += new System.EventHandler(this.BtnMinus10_Click);
            // 
            // lblHoraActual
            // 
            this.lblHoraActual.BackColor = System.Drawing.Color.Cyan;
            resources.ApplyResources(this.lblHoraActual, "lblHoraActual");
            this.lblHoraActual.Name = "lblHoraActual";
            // 
            // btnPlus30
            // 
            this.btnPlus30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnPlus30.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnPlus30, "btnPlus30");
            this.btnPlus30.ForeColor = System.Drawing.Color.Cyan;
            this.btnPlus30.Name = "btnPlus30";
            this.btnPlus30.UseVisualStyleBackColor = false;
            this.btnPlus30.Click += new System.EventHandler(this.BtnPlus30_Click);
            // 
            // btnMinus30
            // 
            this.btnMinus30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnMinus30.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnMinus30, "btnMinus30");
            this.btnMinus30.ForeColor = System.Drawing.Color.Cyan;
            this.btnMinus30.Name = "btnMinus30";
            this.btnMinus30.UseVisualStyleBackColor = false;
            this.btnMinus30.Click += new System.EventHandler(this.BtnMinus30_Click);
            // 
            // lblFondoCeleste
            // 
            this.lblFondoCeleste.BackColor = System.Drawing.Color.Cyan;
            resources.ApplyResources(this.lblFondoCeleste, "lblFondoCeleste");
            this.lblFondoCeleste.Name = "lblFondoCeleste";
            // 
            // panSidePanel
            // 
            this.panSidePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panSidePanel.Controls.Add(this.panel1);
            this.panSidePanel.Controls.Add(this.btnShowHideSidePanel);
            this.panSidePanel.Controls.Add(this.button2);
            this.panSidePanel.Controls.Add(this.button1);
            this.panSidePanel.Controls.Add(this.lvwRecos);
            resources.ApplyResources(this.panSidePanel, "panSidePanel");
            this.panSidePanel.Name = "panSidePanel";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblTitOpacidadIntervaloFijo);
            this.panel1.Controls.Add(this.scrollOpacidadIntervaloFijo);
            this.panel1.Controls.Add(this.lblColorIntervaloFijo);
            this.panel1.Controls.Add(this.chkIntervaloFijo);
            this.panel1.Controls.Add(this.dateTimePicker2);
            this.panel1.Controls.Add(this.dateTimePicker1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lblTitOpacidadIntervaloFijo
            // 
            resources.ApplyResources(this.lblTitOpacidadIntervaloFijo, "lblTitOpacidadIntervaloFijo");
            this.lblTitOpacidadIntervaloFijo.ForeColor = System.Drawing.Color.Silver;
            this.lblTitOpacidadIntervaloFijo.Name = "lblTitOpacidadIntervaloFijo";
            // 
            // scrollOpacidadIntervaloFijo
            // 
            resources.ApplyResources(this.scrollOpacidadIntervaloFijo, "scrollOpacidadIntervaloFijo");
            this.scrollOpacidadIntervaloFijo.Name = "scrollOpacidadIntervaloFijo";
            // 
            // lblColorIntervaloFijo
            // 
            this.lblColorIntervaloFijo.BackColor = System.Drawing.Color.SpringGreen;
            resources.ApplyResources(this.lblColorIntervaloFijo, "lblColorIntervaloFijo");
            this.lblColorIntervaloFijo.Name = "lblColorIntervaloFijo";
            // 
            // chkIntervaloFijo
            // 
            resources.ApplyResources(this.chkIntervaloFijo, "chkIntervaloFijo");
            this.chkIntervaloFijo.Checked = true;
            this.chkIntervaloFijo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIntervaloFijo.ForeColor = System.Drawing.Color.Silver;
            this.chkIntervaloFijo.Name = "chkIntervaloFijo";
            this.chkIntervaloFijo.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker2
            // 
            resources.ApplyResources(this.dateTimePicker2, "dateTimePicker2");
            this.dateTimePicker2.CalendarMonthBackground = System.Drawing.Color.Cyan;
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.ShowUpDown = true;
            this.dateTimePicker2.Value = new System.DateTime(2022, 1, 1, 0, 0, 0, 0);
            // 
            // dateTimePicker1
            // 
            resources.ApplyResources(this.dateTimePicker1, "dateTimePicker1");
            this.dateTimePicker1.CalendarMonthBackground = System.Drawing.Color.Cyan;
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.ShowUpDown = true;
            this.dateTimePicker1.Value = new System.DateTime(2022, 1, 1, 0, 0, 0, 0);
            // 
            // btnShowHideSidePanel
            // 
            this.btnShowHideSidePanel.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnShowHideSidePanel, "btnShowHideSidePanel");
            this.btnShowHideSidePanel.ForeColor = System.Drawing.Color.Cyan;
            this.btnShowHideSidePanel.Name = "btnShowHideSidePanel";
            this.btnShowHideSidePanel.UseVisualStyleBackColor = true;
            this.btnShowHideSidePanel.Click += new System.EventHandler(this.BtnShowHideSidePanel_Click);
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button2, "button2");
            this.button2.ForeColor = System.Drawing.Color.Cyan;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button1, "button1");
            this.button1.ForeColor = System.Drawing.Color.Cyan;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lvwRecos
            // 
            this.lvwRecos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lvwRecos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwRecos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chBan,
            this.chEstilo});
            this.lvwRecos.HideSelection = false;
            this.lvwRecos.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("lvwRecos.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("lvwRecos.Items1"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("lvwRecos.Items2"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("lvwRecos.Items3"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("lvwRecos.Items4")))});
            resources.ApplyResources(this.lvwRecos, "lvwRecos");
            this.lvwRecos.Name = "lvwRecos";
            this.lvwRecos.UseCompatibleStateImageBehavior = false;
            this.lvwRecos.View = System.Windows.Forms.View.Details;
            // 
            // chBan
            // 
            resources.ApplyResources(this.chBan, "chBan");
            // 
            // chEstilo
            // 
            resources.ApplyResources(this.chEstilo, "chEstilo");
            // 
            // panBasicData
            // 
            this.panBasicData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panBasicData.Controls.Add(this.dtpHoraDesde);
            this.panBasicData.Controls.Add(this.dtpHoraHasta);
            this.panBasicData.Controls.Add(this.dtpFechaHasta);
            this.panBasicData.Controls.Add(this.lblTitHasta);
            this.panBasicData.Controls.Add(this.btnAceptarDatosBasicos);
            this.panBasicData.Controls.Add(this.dtpFechaDesde);
            this.panBasicData.Controls.Add(this.txtFichas);
            this.panBasicData.Controls.Add(this.txtLineas);
            this.panBasicData.Controls.Add(this.cmbLineas);
            this.panBasicData.Controls.Add(this.cmbModoHistorico);
            this.panBasicData.Controls.Add(this.lblTitDesde);
            this.panBasicData.Controls.Add(this.lblTitModoHistorico);
            this.panBasicData.Controls.Add(this.lblTitLineas);
            this.panBasicData.Controls.Add(this.lblTitFicha);
            resources.ApplyResources(this.panBasicData, "panBasicData");
            this.panBasicData.Name = "panBasicData";
            // 
            // dtpHoraDesde
            // 
            this.dtpHoraDesde.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            resources.ApplyResources(this.dtpHoraDesde, "dtpHoraDesde");
            this.dtpHoraDesde.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraDesde.Name = "dtpHoraDesde";
            this.dtpHoraDesde.ShowUpDown = true;
            this.dtpHoraDesde.Value = new System.DateTime(2023, 1, 24, 0, 0, 0, 0);
            // 
            // dtpHoraHasta
            // 
            resources.ApplyResources(this.dtpHoraHasta, "dtpHoraHasta");
            this.dtpHoraHasta.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraHasta.Name = "dtpHoraHasta";
            this.dtpHoraHasta.ShowUpDown = true;
            this.dtpHoraHasta.Value = new System.DateTime(2023, 1, 24, 0, 0, 0, 0);
            // 
            // dtpFechaHasta
            // 
            resources.ApplyResources(this.dtpFechaHasta, "dtpFechaHasta");
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            // 
            // lblTitHasta
            // 
            resources.ApplyResources(this.lblTitHasta, "lblTitHasta");
            this.lblTitHasta.ForeColor = System.Drawing.Color.Silver;
            this.lblTitHasta.Name = "lblTitHasta";
            // 
            // btnAceptarDatosBasicos
            // 
            resources.ApplyResources(this.btnAceptarDatosBasicos, "btnAceptarDatosBasicos");
            this.btnAceptarDatosBasicos.Name = "btnAceptarDatosBasicos";
            this.btnAceptarDatosBasicos.UseVisualStyleBackColor = true;
            this.btnAceptarDatosBasicos.Click += new System.EventHandler(this.BtnAceptarDatosBasicos_Click);
            // 
            // dtpFechaDesde
            // 
            resources.ApplyResources(this.dtpFechaDesde, "dtpFechaDesde");
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            // 
            // txtFichas
            // 
            this.txtFichas.BackColor = System.Drawing.Color.Cyan;
            this.txtFichas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtFichas, "txtFichas");
            this.txtFichas.Name = "txtFichas";
            // 
            // txtLineas
            // 
            this.txtLineas.BackColor = System.Drawing.Color.Cyan;
            this.txtLineas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtLineas, "txtLineas");
            this.txtLineas.Name = "txtLineas";
            // 
            // cmbLineas
            // 
            this.cmbLineas.BackColor = System.Drawing.Color.Aqua;
            this.cmbLineas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbLineas, "cmbLineas");
            this.cmbLineas.FormattingEnabled = true;
            this.cmbLineas.Name = "cmbLineas";
            this.cmbLineas.SelectedIndexChanged += new System.EventHandler(this.CmbLineas_SelectedIndexChanged);
            // 
            // cmbModoHistorico
            // 
            this.cmbModoHistorico.BackColor = System.Drawing.Color.Aqua;
            this.cmbModoHistorico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbModoHistorico, "cmbModoHistorico");
            this.cmbModoHistorico.FormattingEnabled = true;
            this.cmbModoHistorico.Items.AddRange(new object[] {
            resources.GetString("cmbModoHistorico.Items"),
            resources.GetString("cmbModoHistorico.Items1")});
            this.cmbModoHistorico.Name = "cmbModoHistorico";
            // 
            // lblTitDesde
            // 
            resources.ApplyResources(this.lblTitDesde, "lblTitDesde");
            this.lblTitDesde.ForeColor = System.Drawing.Color.Silver;
            this.lblTitDesde.Name = "lblTitDesde";
            // 
            // lblTitModoHistorico
            // 
            resources.ApplyResources(this.lblTitModoHistorico, "lblTitModoHistorico");
            this.lblTitModoHistorico.ForeColor = System.Drawing.Color.Silver;
            this.lblTitModoHistorico.Name = "lblTitModoHistorico";
            // 
            // lblTitLineas
            // 
            resources.ApplyResources(this.lblTitLineas, "lblTitLineas");
            this.lblTitLineas.ForeColor = System.Drawing.Color.Silver;
            this.lblTitLineas.Name = "lblTitLineas";
            // 
            // lblTitFicha
            // 
            resources.ApplyResources(this.lblTitFicha, "lblTitFicha");
            this.lblTitFicha.ForeColor = System.Drawing.Color.Silver;
            this.lblTitFicha.Name = "lblTitFicha";
            // 
            // basicDataController
            // 
            this.basicDataController.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            resources.ApplyResources(this.basicDataController, "basicDataController");
            this.basicDataController.Name = "basicDataController";
            // 
            // FrmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.Controls.Add(this.panBasicData);
            this.Controls.Add(this.panSidePanel);
            this.Controls.Add(this.panMediaController);
            this.Controls.Add(this.basicDataController);
            this.Controls.Add(this.picMain);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "FrmMain";
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            this.panMediaController.ResumeLayout(false);
            this.panMediaController.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picActividad)).EndInit();
            this.panSidePanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panBasicData.ResumeLayout(false);
            this.panBasicData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private TimeController basicDataController;
        private System.Windows.Forms.PictureBox picMain;
        private System.Windows.Forms.Panel panMediaController;
        private System.Windows.Forms.CheckBox chkVerHoraEnCruz;
        private System.Windows.Forms.CheckBox chkVerCruz;
        private System.Windows.Forms.Button btnMinus1;
        private System.Windows.Forms.Button btnPlus1;
        private System.Windows.Forms.Button btnPlus10;
        private System.Windows.Forms.Button btnMinus10;
        private System.Windows.Forms.Label lblHoraActual;
        private System.Windows.Forms.Button btnPlus30;
        private System.Windows.Forms.Button btnMinus30;
        private System.Windows.Forms.Label lblFondoCeleste;
        private System.Windows.Forms.PictureBox picActividad;
        private System.Windows.Forms.Label lblFechaActual;
        private System.Windows.Forms.Panel panSidePanel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView lvwRecos;
        private System.Windows.Forms.Button btnShowHideSidePanel;
        private System.Windows.Forms.ColumnHeader chBan;
        private System.Windows.Forms.ColumnHeader chEstilo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitOpacidadIntervaloFijo;
        private System.Windows.Forms.HScrollBar scrollOpacidadIntervaloFijo;
        private System.Windows.Forms.Label lblColorIntervaloFijo;
        private System.Windows.Forms.CheckBox chkIntervaloFijo;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox chkCentrar;
        private System.Windows.Forms.ComboBox cmbLargoSubRango;
        private System.Windows.Forms.Panel panBasicData;
        private System.Windows.Forms.ComboBox cmbModoHistorico;
        private System.Windows.Forms.Label lblTitDesde;
        private System.Windows.Forms.Label lblTitModoHistorico;
        private System.Windows.Forms.Label lblTitLineas;
        private System.Windows.Forms.Label lblTitFicha;
        private System.Windows.Forms.TextBox txtLineas;
        private System.Windows.Forms.ComboBox cmbLineas;
        private System.Windows.Forms.TextBox txtFichas;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.Button btnAceptarDatosBasicos;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.Label lblTitHasta;
        private System.Windows.Forms.DateTimePicker dtpHoraHasta;
        private System.Windows.Forms.DateTimePicker dtpHoraDesde;
    }
}

