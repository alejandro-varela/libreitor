using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Comun;
using Newtonsoft.Json;

// alpha masking
// https://stackoverflow.com/questions/3654220/alpha-masking-in-c-sharp-system-drawing

// bajar puntos históricos
// por nombre: http://vm-coches:5000/HistoriaCochesDriveupAnteriores?diasMenos=1&formato=csv
// por ip    : http://192.168.201.74:5000/HistoriaCochesDriveupAnteriores?diasMenos=1&formato=csv

// hacer lupa geométrica, una lupa con tamaño variable que al pasar por arriba del mapa resalte la vista de tiempo con las partes en donde estan esos puntos

// f 4334 22 ago
// f 3152 25 ago (mejorar algoritmo de actividad)
// f 3666 25 ago

// f 3786 13 sept (línea 203) a las 12 de la noche se corta antes de llegar a la punta
// f 3829 13 sept es un colectivo de la línea 41 !!!
// f 4187 13 sept (línea 203) faltan puntos hay un pozo
// f 3853 13 sept (línea 203)
//     03:50 se inventó un recorrido nuevo, se usa la punta O exitosamente
//     08:17 desaparecen puntos

namespace VHR
{
    public partial class FrmMain : Form
    {
        public delegate void PintorCapa(Graphics graphics);

        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public TimeSpan Intervalo
        {
            get
            {
                var timeSpan = Hasta - Desde;

                if (timeSpan.TotalSeconds < 0)
                {
                    return TimeSpan.FromSeconds(0);
                }

                return timeSpan;
            }
        }

        /////////////////////////////////////////////////////////////
        // panel media...

        BasicData _basicData = null;
        
        public int OffSetSegundosDesde { get; set; } = 0;

        public void CambiarOffsetTiempo(int segs)
        {
            var newVal = OffSetSegundosDesde + segs;

            if (newVal < 0)
            {
                newVal = 0;
            }

            if (newVal > Convert.ToInt32(Intervalo.TotalSeconds))
            {
                newVal = Convert.ToInt32(Intervalo.TotalSeconds); // 86400
            }

            OffSetSegundosDesde = newVal;
        }

        // Puntos del subrango
        // [  . . . . a ]
        // |--cola--|
        // |--duración--|
        // a = actual

        public DateTime SubRangoHoraActual
        {
            get
            {
                return _basicData.Desde.AddSeconds(OffSetSegundosDesde);
            }
        }

        public DateTime SubRangoColaHasta
        {
            get
            {
                return SubRangoHoraActual.AddMinutes(-1 * GetLargoMinutosSubRango());
            }
        }

        int GetLargoMinutosSubRango()
        {
            string safeText = string.IsNullOrEmpty(cmbLargoSubRango.Text) ?
                "0" : cmbLargoSubRango.Text
            ;
            int largoMinutos = int.Parse(safeText);
            return largoMinutos;
        }

        private void BtnMinus1_Click(object sender, EventArgs e)
        {
            CambiarOffsetTiempo(-1 * 60);
            ActualizarTodo();
        }

        private void BtnMinus10_Click(object sender, EventArgs e)
        {
            CambiarOffsetTiempo(-10 * 60);
            ActualizarTodo();
        }

        private void BtnMinus30_Click(object sender, EventArgs e)
        {
            CambiarOffsetTiempo(-30 * 60);
            ActualizarTodo();
        }

        private void BtnPlus1_Click(object sender, EventArgs e)
        {
            CambiarOffsetTiempo(1 * 60);
            ActualizarTodo();
        }

        private void BtnPlus10_Click(object sender, EventArgs e)
        {
            CambiarOffsetTiempo(10 * 60);
            ActualizarTodo();
        }

        private void BtnPlus30_Click(object sender, EventArgs e)
        {
            CambiarOffsetTiempo(30 * 60);
            ActualizarTodo();
        }

        private void ChkVerCruz_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarTodo();
        }

        private void ChkVerHoraEnCruz_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarTodo();
        }

        //
        /////////////////////////////////////////////////////////////

        // modo (picobus o driveup )
        string DameModoHistorico()
        {
            return cmbModoHistorico.SelectedItem.ToString().ToLower();
        }


        // granularidad
        int _granularidad = 20;
        Topes2D _topes2D = null;

        // para hacer zoom el pic
        public object _lockZoom = new object();
        int zoomCount = 0;
        int MAXZOOMCOUNT = 9;
        int MINZOOMCOUNT = -4;

        // para mover el pic y hacer zoom el pic
        int picMainTopOffset = 0;
        int picMainLeftOffset = 0;

        // para movimientos en el picMain
        int picMainDownX = 0;
        int picMainDownY = 0;
        int picMainCurX = 0;
        int picMainCurY = 0;
        bool picIsMouseDown = false;

        // para movimientos en el picActividad
        int picActividadDownX = 0;
        int picActividadDownY = 0;
        bool picActividadIsMouseDown = false;

        // para guardar la capa base y no repintarla
        Bitmap _capaBaseMain;
        Bitmap _capaBaseActividad;

        // lista de pintores
        List<Pintor> _pintoresMain      = new List<Pintor>();
        List<Pintor> _pintoresActividad = new List<Pintor>();

        // para guardar los datos de los coches
        Dictionary<int, DatosCoche> _datosCoches;

        public FrmMain()
        {
            InitializeComponent();

            Resize   += FrmMain_Resize;
            Shown    += FrmMain_Shown;
            KeyPress += FrmMain_KeyPress;
            KeyDown  += FrmMain_KeyDown;

            dtpFechaDesde.ValueChanged += (sender, evargs) =>
            {
                this.Desde = ConstruirFechaHoraDesde();
            };

            dtpHoraDesde.ValueChanged += (sender, evargs) =>
            {
                this.Desde = ConstruirFechaHoraDesde();
            };

            dtpFechaHasta.ValueChanged += (sender, evargs) =>
            {
                this.Hasta = ConstruirFechaHoraHasta();
            };

            dtpHoraHasta.ValueChanged += (sender, evargs) =>
            {
                this.Hasta = ConstruirFechaHoraHasta();
            };

            picMain.MouseDown                   += PicMain_MouseDown;
            picMain.MouseUp                     += PicMain_MouseUp;
            picMain.MouseMove                   += PicMain_MouseMove;
            picMain.MouseWheel                  += PicMain_MouseWheel;
            picMain.Location                    = new Point(0, 0);
            picMain.Size                        = new Size(0, 0);
            picMain.SizeMode                    = PictureBoxSizeMode.StretchImage;

            panMediaController.Location = new Point(6, 6);

            picActividad.MouseDown              += PicActividad_MouseDown;
            picActividad.MouseUp                += PicActividad_MouseUp;
            picActividad.MouseMove              += PicActividad_MouseMove;
            picActividad.SizeMode               = PictureBoxSizeMode.StretchImage;
            picActividad.BackColor              = Color.FromArgb(64, 64, 64);

            //basicDataController.CargarPuntos    += BasicDataController_OnCargarPuntos;
            //basicDataController.Location = new Point(6, 6);

            panSidePanel.Width = 40 + 6 + 6;
            btnShowHideSidePanel.Text = "<<";

            _pintoresMain.Add(new Pintor("Base"      , PintorCapaBase) { Habilitado = true });
            _pintoresMain.Add(new Pintor("Rango"     , PintorCapaRango) { Habilitado = true });
            _pintoresMain.Add(new Pintor("SubRango"  , PintorCapaSubRango) { Habilitado = true });

            _pintoresActividad.Add(new Pintor("Base", PintorCapaBaseActividad) { Habilitado = true });
            _pintoresActividad.Add(new Pintor("Indicador", PintorIndicadorActividad) { Habilitado = true });

            foreach (var itemX in Program.Lineas)
            {
                cmbLineas.Items.Add(itemX.Key);
            }

            cmbModoHistorico.SelectedIndex = 0;
            cmbLargoSubRango.SelectedIndex = 5;
        }

        DateTime ConstruirFechaHoraDesde()
        {
            return new DateTime(
                year  : dtpFechaDesde.Value.Year,
                month : dtpFechaDesde.Value.Month,
                day   : dtpFechaDesde.Value.Day,
                hour  : dtpHoraDesde.Value.Hour,
                minute: dtpHoraDesde.Value.Minute,
                second: dtpHoraDesde.Value.Second
            );
        }

        DateTime ConstruirFechaHoraHasta()
        {
            return new DateTime(
                year  : dtpFechaHasta.Value.Year,
                month : dtpFechaHasta.Value.Month,
                day   : dtpFechaHasta.Value.Day,
                hour  : dtpHoraHasta.Value.Hour,
                minute: dtpHoraHasta.Value.Minute,
                second: dtpHoraHasta.Value.Second
            );
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            bool ctrl  = e.Control;
            bool shift = e.Shift;
            bool alt   = e.Alt;

            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    if (panMediaController.Visible)
                    {
                        int segs = ctrl ? 600 : 60;
                        CambiarOffsetTiempo(segs);
                        ActualizarTodo();
                        e.Handled = true;
                    }
                    break;
                case Keys.PageUp:
                    if (panMediaController.Visible)
                    {
                        int segs = ctrl ? -600 : -60;
                        CambiarOffsetTiempo(segs);
                        ActualizarTodo();
                        e.Handled = true;
                    }
                    break;
            }
        }

        void IncrementarZoom()
        {
            CambiarZoom(1);
        }

        void DecrementarZoom()
        {
            CambiarZoom(-1);
        }

        void CambiarZoom(int variacion)
        {
            lock (_lockZoom)
            {
                zoomCount += variacion;
                zoomCount = Math.Max(zoomCount, MINZOOMCOUNT);
                zoomCount = Math.Min(zoomCount, MAXZOOMCOUNT);

                // valores que puede tomar zoomCount --> -4,-3,-2,-1, 0, 1, 2, 3, 4
                var zooms = new[] { 
                    0.350, 0.450, 0.500, 0.750, 
                    1, 
                    1.125, 1.250, 1.375, 1.500, 1.625, 1.750, 1.875, 2.000, 3.000, 
                };
                var valZoom = zooms[zoomCount + Math.Abs(MINZOOMCOUNT)];

                // vieja pos del mouse segun el pic
                var oldMouseX = picMainCurX;
                var oldMouseY = picMainCurY;

                // calculo tamaños y posiciones según el zoom
                var oldPicMainWidth = picMain.Width;
                var oldPicMainHeight = picMain.Height;
                var newPicMainWidth = Convert.ToInt32(_capaBaseMain.Width * valZoom);
                var newPicMainHeight = Convert.ToInt32(_capaBaseMain.Height * valZoom);
                var diffPicMainWidth = newPicMainWidth - oldPicMainWidth;
                var diffPicMainHeight = newPicMainHeight - oldPicMainHeight;

                var propX = (double)picMainCurX / (double)oldPicMainWidth;
                var propY = (double)picMainCurY / (double)oldPicMainHeight;

                // nuevo tamaño del picMain
                picMain.Size = new Size(newPicMainWidth, newPicMainHeight);

                // nueva posición del picMain
                var newLeft = (picMainLeftOffset - Convert.ToInt32(diffPicMainWidth * propX));
                var newTop = (picMainTopOffset - Convert.ToInt32(diffPicMainHeight * propY));
                picMain.Location = new Point { X = newLeft, Y = newTop };
                picMainLeftOffset = newLeft;
                picMainTopOffset = newTop;

                // llamo al gc
                GC.Collect();
            }
        }

        private void PicMain_MouseWheel(object sender, MouseEventArgs e)
        {
            CambiarZoom( (e.Delta > 0) ? 1 : -1 );
        }

        private void PicMain_MouseDown(object sender, MouseEventArgs e)
        {
            picMainDownX = e.X;
            picMainDownY = e.Y;
            picIsMouseDown = true;
        }

        private void PicMain_MouseUp(object sender, MouseEventArgs e)
        {
            picIsMouseDown = false;
        }

        private void PicMain_MouseMove(object sender, MouseEventArgs e)
        {
            picMainCurX = e.X;
            picMainCurY = e.Y;

            if (picIsMouseDown)
            {
                var diffX = picMainDownX - e.X;
                var diffY = picMainDownY - e.Y;

                picMain.Left -= diffX;
                picMainLeftOffset -= diffX;

                picMain.Top -= diffY;
                picMainTopOffset -= diffY;
            }

            GC.Collect();
        }

        private void PicActividad_MouseDown(object sender, MouseEventArgs e)
        {
            picActividadIsMouseDown = true;
            PicActividad_MoverAMinutoArbitrario_SegunX(e.X);
        }

        private void PicActividad_MouseUp(object sender, MouseEventArgs e)
        {
            picActividadIsMouseDown = false;
        }

        private void PicActividad_MouseMove(object sender, MouseEventArgs e)
        {
            if (picActividadIsMouseDown)
            {
                PicActividad_MoverAMinutoArbitrario_SegunX(e.X);
            }
        }

        void PicActividad_MoverAMinutoArbitrario_SegunX(int x)
        {
            var anchoPic = picActividad.Width;
            var delta = Intervalo.TotalMinutes / anchoPic;
            var segs = Convert.ToInt32(x * delta) * 60;
            OffSetSegundosDesde = segs;
            CambiarOffsetTiempo(0);
            ActualizarTodo();
        }

        private void FrmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            int inc = 5;
            int veces = 10;

            if ('s' == e.KeyChar)
            {
                for (int i = 0; i < veces; i++)
                {
                    picMainTopOffset -= inc;
                    picMain.Top -= inc;
                    Refresh();
                }
            }
            else if ('w' == e.KeyChar)
            {
                for (int i = 0; i < veces; i++)
                {
                    picMainTopOffset += inc;
                    picMain.Top += inc;
                    Refresh();
                }
            }
            else if ('d' == e.KeyChar)
            {
                for (int i = 0; i < veces; i++)
                {
                    picMainLeftOffset -= inc;
                    picMain.Left -= inc;
                    Refresh();
                }
            }
            else if ('a' == e.KeyChar)
            {
                for (int i = 0; i < veces; i++)
                {
                    picMainLeftOffset += inc;
                    picMain.Left += inc;
                    Refresh();
                }
            }
            else if ('+' == e.KeyChar)
            {
                IncrementarZoom();
            }
            else if ('-' == e.KeyChar)
            {
                DecrementarZoom();
            }

        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            FrmMain_Resize(sender, e);
        }

        private Bitmap CrearBitmap(Bitmap capaBase, IEnumerable<Pintor> pintores)
        {
            var newBitmap = new Bitmap(capaBase.Width, capaBase.Height);
            var graphics  = Graphics.FromImage(newBitmap);

            foreach (var pintorX in pintores)
            {
                pintorX.Accion.Invoke(graphics);
            }

            return newBitmap;
        }

        // En el resize tuve que simular la prop Anchor
        // porque no me andaba con el control tipo MediaController
        // De paso lo hago con todos los controles
        private void FrmMain_Resize(object sender, EventArgs e)
        {
            var realWidth               = ClientRectangle.Width;
            var realHeight              = ClientRectangle.Height;
            int padding                 = 4;

            picMain.Top                 = picMainTopOffset;
            picMain.Left                = picMainLeftOffset;

            //basicDataController.Top     = padding;
            //basicDataController.Left    = padding;
            //basicDataController.Width   = realWidth - (basicDataController.Left * 2);

            panBasicData.Left           = padding;
            panBasicData.Top            = padding;
            panBasicData.Width          = realWidth - (panBasicData.Left * 2);

            panMediaController.Left     = padding;
            panMediaController.Top      = /*padding + */ panBasicData.Height + padding;
            panMediaController.Width    = realWidth - (panMediaController.Left * 2);

            picActividad.Left           = padding;
            picActividad.Top            = padding;
            picActividad.Width          = panMediaController.Width - (picActividad.Left * 2);

            panSidePanel.Top  = panMediaController.Height + padding + panBasicData.Height + padding * 2;
            panSidePanel.Left = realWidth - panSidePanel.Width - padding;
            panSidePanel.Height = realHeight - panMediaController.Height - padding - panBasicData.Height - padding * 3;
        }

        private async void BtnAceptarDatosBasicos_Click(object sender, EventArgs e)
        {
            bool ok = await BasicDataController_OnCargarPuntos(this, new EventArgs());

            if (ok)
            {
                // cosas visibles...
                panMediaController.Visible = true;
                panSidePanel.Visible = true;
            }
        }

        List<int> DameNumerosSeparadosPorComas(string s)
        {
            var partes = s.Split(',');
            var ret = new List<int>();

            foreach (var parteX in partes)
            {
                var num = int.Parse(parteX.Trim());
                ret.Add(num);
            }

            return ret;
        }

        private bool SonNumerosSeparadosPorComas(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            var partes = text.Split(',');

            bool EsNumero (string s)
            {
                foreach (char c in s)
                {
                    if (c < '0' || c > '9')
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var parteX in partes)
            {
                if (parteX.Trim().Length == 0)
                {
                    return false;
                }

                if (!EsNumero(parteX.Trim()))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> BasicDataController_OnCargarPuntos(object sender, EventArgs e)
        {
            //var fichas = basicDataController.Fichas;
            //var lineas = basicDataController.Lineas;
            //var desde  = basicDataController.Desde;
            //var hasta  = basicDataController.Hasta;

            List<int> lineas = null;

            if (SonNumerosSeparadosPorComas(txtLineas.Text))
            {
                lineas = DameNumerosSeparadosPorComas(txtLineas.Text);
            }
            else
            {
                Program.ShowError("Ingrese una línea válida", "Error: Línea");
                return false;
            }

            List<int> fichas = null;

            if (SonNumerosSeparadosPorComas(txtFichas.Text))
            {
                fichas = DameNumerosSeparadosPorComas(txtFichas.Text);
            }
            else
            {
                Program.ShowError("Ingrese una ficha válida", "Error: Ficha");
                return false;
            }

            DateTime desde = ConstruirFechaHoraDesde();
            DateTime hasta = ConstruirFechaHoraHasta();

            if (hasta < desde)
            {
                Program.ShowError("La fecha hasta no puede ser menor que la fecha desde", "Error: fecha desde hasta");
                return false;
            }

            if (desde >= DateTime.Now)
            {
                var xDiasFuturo = Convert.ToInt32(desde.Subtract(DateTime.Now).TotalDays);
                var xFechaCorta = desde.ToShortDateString();
                Program.ShowError($"Faltan {xDiasFuturo}~ dias para llegar a la fecha {xFechaCorta}", "Error: fecha desde futura");
                return false;
            }

            var basicData = new BasicData()
            {
                Fichas = fichas,
                Lineas = lineas,
                Desde  = desde,
                Hasta  = hasta,
            };

            var retCrearDatosCoche = CrearDatosCoches(basicData);
            _datosCoches = retCrearDatosCoche.Item1;

            var retCrearCapaBase = CrearCapaBase(basicData);
            _capaBaseMain = retCrearCapaBase.Item1;
            var errorCrearCapaBase = retCrearCapaBase.Item2;

            if (errorCrearCapaBase != null)
            {
                Program.ShowError(errorCrearCapaBase.Mensaje, "Error: Creando la capa base");
                return false;
            }

            _basicData = basicData;

            picMain.Top    = picMainTopOffset = 0;
            picMain.Left   = picMainLeftOffset = 0;
            picMain.Image  = CrearBitmap(_capaBaseMain, _pintoresMain);
            picMain.Width  = _capaBaseMain.Width;
            picMain.Height = _capaBaseMain.Height;

            _capaBaseActividad = CrearImagenActividad(_datosCoches);
            picActividad.Image = CrearBitmap(_capaBaseActividad, _pintoresActividad);

            // título
            var sFichas = string.Join(", ", basicData.Fichas.ToArray());
            var sDesde = basicData.Desde.ToString("dd/MM");
            Text = $"F{sFichas}  {sDesde}";

            // labels del control de media
            lblHoraActual.Text  = basicData.Desde.AddSeconds(OffSetSegundosDesde).ToString("HH:mm");
            lblFechaActual.Text = basicData.Desde.AddSeconds(OffSetSegundosDesde).ToString("dd MMM").Replace(".", string.Empty);

            return true;
        }

        private Bitmap CrearImagenActividad(Dictionary<int, DatosCoche> datosCoches)
        {
            Bitmap bmp = new Bitmap(
                Convert.ToInt32(Math.Ceiling(Intervalo.TotalMinutes)), 
                picActividad.Height
            );
            Graphics g = Graphics.FromImage(bmp);

            var pen100 = new Pen(Color.BlueViolet, 1);
            var pen60 = new Pen(Color.FromArgb(60, Color.BlueViolet), 1);
            var pen40 = new Pen(Color.FromArgb(20, Color.BlueViolet), 1);

            foreach (var key in datosCoches.Keys)
            {
                var datosCoche = datosCoches[key];
                if (datosCoche.PuntosHistoricos != null && datosCoche.PuntosHistoricos.Any())
                {
                    var puntoAnterior = datosCoche.PuntosHistoricos.First();
                    foreach (var puntoHistoricoX in datosCoche.PuntosHistoricos.Skip(1))
                    {
                        var dist    = Haversine.GetDist(puntoAnterior, puntoHistoricoX);
                        //var offsetX = puntoHistoricoX.Fecha.Hour * 60 + puntoHistoricoX.Fecha.Minute;
                        var offsetX = Convert.ToInt32((puntoHistoricoX.Fecha - Desde).TotalMinutes);

                        if (dist >= 120) { g.DrawLine(pen100, offsetX, 0, offsetX, picActividad.Height); }
                        if (dist >= 60) { g.DrawLine(pen60, offsetX, 0, offsetX, picActividad.Height); }
                        if (dist >= 40) { g.DrawLine(pen40, offsetX, 0, offsetX, picActividad.Height); }
                    }
                }
            }
            
            return bmp;
        }

        private Tuple<Dictionary<int, DatosCoche>, Error> CrearDatosCoches(BasicData basicData)
        {
            var data = new Dictionary<int, DatosCoche>();

            foreach (var ficha in basicData.Fichas)
            {
                // TODO: console print
                //basicDataController.ConsolePrint($"Obteniendo puntos ficha: {ficha}");
                Refresh();
                var datosCoche = GetDatosCoches(DameModoHistorico(), ficha, basicData.Desde, basicData.Hasta);
                data.Add(ficha, datosCoche);
            }

            return new Tuple<Dictionary<int, DatosCoche>, Error>(data, null);
        }

        private DatosCoche GetDatosCoches(string modo, int ficha, DateTime desde, DateTime hasta)
        {
            var prefijoNombreArchivo = modo;
            int indexFicha = 0;
            int indexLat   = 0;
            int indexLng   = 0;
            int indexFecha = 0;
            
            // segun modo...
            if (modo == "picobus")
            {
                // 0     1       2         3         4                   5
                // Ficha;Id     ;Lat      ;Lng      ;FechaLlegadaLocal  ;FechaDeProduccion
                // 3148 ;3000699;-34.54093;-58.82272;2022-08-22 00:00:00;2022-08-22 00:00:00
                indexFicha = 0;
                indexLat   = 2;
                indexLng   = 3;
                indexFecha = 4; // fecha de llegada
            }
            else if (modo == "driveup")
            {
                // 0     1           2           3                   4                   5
                // Ficha;Lat        ;Lng        ;FechaLocal         ;Recordedat         ;FechaLlegadaLocal
                // 3824 ;-34.4354233;-58.8181466;2022-08-09 23:59:59;2022-08-10 02:59:59;2022-08-10 00:00:00
                indexFicha = 0;
                indexLat   = 1;
                indexLng   = 2;
                indexFecha = 3; // fecha local
            }

            var puntosHistoricos = new List<PuntoHistorico>();

            foreach (var parDesdeHasta in DateTimeHelper.GetFromToDayPairs(desde, hasta))
            {
                var nombreArchivo = $"{prefijoNombreArchivo}_{parDesdeHasta.Desde.Year:0000}_{parDesdeHasta.Desde.Month:00}_{parDesdeHasta.Desde.Day:00}.csv";

                // encontrar el archivo de ese día...
                var dirArchivos = $"./datos/historicos/y{parDesdeHasta.Desde.Year:0000}/mo{parDesdeHasta.Desde.Month:00}/";
                var pathArchivo = Path.Combine(dirArchivos, nombreArchivo).Replace('\\', '/');

                // si no lo encuentro devuelvo datos vacios:
                // TODO: informar error
                if (string.IsNullOrEmpty(nombreArchivo) || // no hay nombre archivo
                    !File.Exists(pathArchivo)) // el archivo no existe
                {
                    return new DatosCoche
                    {
                        Ficha = ficha,
                        PuntosHistoricos = new List<PuntoHistorico>(),
                    };
                }

                foreach (var lineX in File.ReadLines(pathArchivo).Skip(1))
                {
                    var partes = lineX.Split(';');
                    var fichaX = int.Parse(partes[indexFicha]);
                    if (fichaX != ficha)
                    {
                        continue;
                    }
                    var latX = double.Parse(partes[indexLat], CultureInfo.InvariantCulture);
                    var lngX = double.Parse(partes[indexLng], CultureInfo.InvariantCulture);
                    var fechaX = DateTime.Parse(partes[indexFecha]);

                    var puntoHistorico = new PuntoHistorico
                    {
                        Alt = 0,
                        Lat = latX,
                        Lng = lngX,
                        Fecha = fechaX,
                    };

                    if (puntoHistorico.Fecha >= desde
                        && 
                        puntoHistorico.Fecha < hasta)
                    {
                        puntosHistoricos.Add(puntoHistorico);
                    }
                }
            }

            // recorrerlo filtrando por ficha y fechas...
            // si ese predicado da true, poner el punto histórico
            return new DatosCoche
            {
                Ficha = ficha,
                PuntosHistoricos = puntosHistoricos,
            };
        }

        private Tuple<Bitmap, Error> CrearCapaBase(BasicData basicData)
        {
            // buscar todos los puntos de los recorridos...
            List<RecorridoLinBan> recorridosTeoricos;
            List<Punto> puntosTeoricos;

            try
            {
                recorridosTeoricos = RecorridoLinBan.LeerRecorridosPorArchivos(
                    "./datos/teoricos/",
                    basicData.Lineas.ToArray(),
                    basicData.Desde
                );

                puntosTeoricos = recorridosTeoricos
                    .SelectMany(rec => rec.Puntos)
                    .Select    (p => (Punto) p)
                    .ToList    ()
                ;
            }
            catch (Exception exx)
            {
                return new Tuple<Bitmap, Error>( null, new Error { Mensaje = exx.Message } );
            }

            //// trabajo de bordes
            Punto ptttMinLat = puntosTeoricos[0];
            Punto ptttMaxLat = puntosTeoricos[0];
            Punto ptttMinLng = puntosTeoricos[0];
            Punto ptttMaxLng = puntosTeoricos[0];

            foreach (var pttt in puntosTeoricos)
            {
                if (pttt.Lat < ptttMinLat.Lat) { ptttMinLat = pttt; }
                if (pttt.Lat > ptttMaxLat.Lat) { ptttMaxLat = pttt; }
                if (pttt.Lng < ptttMinLng.Lng) { ptttMinLng = pttt; }
                if (pttt.Lng > ptttMaxLng.Lng) { ptttMaxLng = pttt; }
            }

            puntosTeoricos.Add(new Punto { Lat = ptttMinLat.Lat - 0.025, Lng = ptttMinLat.Lng });
            puntosTeoricos.Add(new Punto { Lat = ptttMaxLat.Lat + 0.025, Lng = ptttMaxLat.Lng });
            puntosTeoricos.Add(new Punto { Lat = ptttMinLng.Lat, Lng = ptttMinLng.Lng - 0.025 });
            puntosTeoricos.Add(new Punto { Lat = ptttMaxLng.Lat, Lng = ptttMaxLng.Lng + 0.025 });
            //// trabajo de bordes <fin>

            _topes2D = Topes2D.CreateFromPuntos(puntosTeoricos);
            var alturaGranular = _topes2D.GetAlturaGranular(_granularidad);
            var anchoGranular  = _topes2D.GetAnchoGranular (_granularidad);

            // creo el mapa de bits base
            var capaBase = new Bitmap(anchoGranular, alturaGranular);
            var g = Graphics.FromImage(capaBase);

            // dibujo los recorridos
            var coloresPorLineas = new Dictionary<int, Color>
            {
                //{ 159, Color.FromArgb(50, Color.Green) },
                //{ 163, Color.FromArgb(50, Color.Cyan) },
                //{ 165, Color.FromArgb(50, Color.Orange) },
                //{ 166, Color.FromArgb(50, Color.Aquamarine) },
                //{ 167, Color.FromArgb(50, Color.LimeGreen) },

                { 159, Color.FromArgb(255, Color.Gray) },
                { 163, Color.FromArgb(255, Color.Gray) },
                { 165, Color.FromArgb(255, Color.Gray) },
                { 166, Color.FromArgb(255, Color.Gray) },
                { 167, Color.FromArgb(255, Color.Gray) },
            };

            //basicDataController.ConsolePrint("Dibujando recorridos...");
            foreach (var recoX in recorridosTeoricos)
            {
                Brush brushx;

                if (coloresPorLineas.ContainsKey(recoX.Linea))
                {
                    brushx = new SolidBrush(coloresPorLineas[recoX.Linea]);
                }
                else
                { 
                    brushx = new SolidBrush(Color.FromArgb(255, Color.Gray));
                }

                //basicDataController.ConsolePrint($"\t{recoX.Linea} {recoX.Bandera}");

                foreach (var puntoX in recoX.Puntos.HacerGranular(_granularidad))
                {
                    var casillero = Casillero.Create(_topes2D, puntoX, _granularidad);

                    g.FillRectangle(
                        brushx, 
                        new Rectangle { 
                            X = casillero.IndexHorizontal-1, 
                            Y = casillero.IndexVertical-1, 
                            Height = 3, 
                            Width  = 3 
                        }
                    );
                }
            }

            // dibujo las puntas de lineas
            var puntasDeLinea = PuntasDeLinea.GetPuntasNombradas(recorridosTeoricos, radio: 500).ToList();
            var penPunta = new Pen(Color.FromArgb(255, Color.GreenYellow), 1);
            var bruPunta = new SolidBrush(Color.FromArgb(50, Color.GreenYellow));
            var bruFuente = new SolidBrush(Color.Black);
            foreach (var puntaX in puntasDeLinea)
            {
                var casillero = Casillero.Create(_topes2D, puntaX.Punto, _granularidad);
                var radioEnPixels  = Convert.ToInt32( puntaX.Radio / _granularidad );
                var rectPunta = new Rectangle(
                    x: casillero.IndexHorizontal - radioEnPixels,
                    y: casillero.IndexVertical - radioEnPixels,
                    width : radioEnPixels * 2,
                    height: radioEnPixels * 2
                );
                var rectCentroPunta = new Rectangle(
                    x: casillero.IndexHorizontal - 1,
                    y: casillero.IndexVertical - 1,
                    width: 3,
                    height: 3
                );
                g.FillEllipse(bruPunta, rectPunta);
                g.DrawEllipse(penPunta, rectPunta);
                //g.DrawEllipse(penPunta, rectCentroPunta);
                g.DrawLine(penPunta, casillero.IndexHorizontal, casillero.IndexVertical - 2, casillero.IndexHorizontal, casillero.IndexVertical + 2);
                g.DrawLine(penPunta, casillero.IndexHorizontal - 2, casillero.IndexVertical, casillero.IndexHorizontal + 2, casillero.IndexVertical);
                g.DrawString(puntaX.Nombre, new Font("Arial", 12, FontStyle.Regular), bruFuente, rectPunta.Location);
                //if (puntaX.Nombre == "F") { int foo = 0; }
            }

            return new Tuple<Bitmap, Error>(capaBase, null);
        }

        void PintorCapaBase(Graphics g)
        {
            g.DrawImage(_capaBaseMain, new Point(0, 0));
            g.DrawString(DameModoHistorico(), new Font("Arial", 10), new SolidBrush(Color.Black), new Point(0, 0));
        }

        void PintorCapaRango(Graphics g)
        {
            // todo se puede memoizar, el resultado es un bitmap
            // siempre y cuando los "parámetros" externos no cambien
            var desde = _basicData.Desde;
            var hasta = _basicData.Hasta;

            // etapa de validación de cambios (TODO:)

            // etapa de cálculo y creación del bitmap
            var miBrush = new SolidBrush(Color.FromArgb(3, Color.LimeGreen));

            foreach (int ficha in _datosCoches.Keys)
            {
                var points = Get_PtsReco_Por_DesdeHasta(ficha, desde, hasta, granular: true);

                foreach (Point p in points)
                {
                    var rect = new Rectangle { 
                        X = p.X - 5, 
                        Y = p.Y - 5, 
                        Height = 11, 
                        Width = 11 
                    };

                    g.FillEllipse(miBrush, rect);
                }
            }
        }

        void PintorCapaSubRango(Graphics g)
        {
            // todo se puede memoizar, el resultado es un bitmap
            // siempre y cuando los "parámetros" externos no cambien
            var desde = SubRangoColaHasta;
            var hasta = SubRangoHoraActual.AddSeconds(60);
            var cruzVisible = chkVerCruz.Checked;
            var horaEnCruzVisible = chkVerHoraEnCruz.Checked;

            // etapa de validación de cambios (TODO:)

            // etapa de cálculo y creación del bitmap
            var miBrushPtsGranulares = new SolidBrush(Color.FromArgb(120, Color.Fuchsia));
            var miBrushPtsCrudos     = new SolidBrush(Color.FromArgb(255, Color.Lime));

            foreach (int ficha in _datosCoches.Keys)
            {

                var ptsHisto = Get_PtsHisto_Por_DesdeHasta(ficha, desde, hasta);

                var ptsRecoGranulares = Convertir_PtsHisto_En_PtsReco(ptsHisto, granular: true);

                foreach (Point px in ptsRecoGranulares)
                {
                    var rect = new Rectangle
                    {
                        X = px.X - 1,
                        Y = px.Y - 1,
                        Height = 3,
                        Width = 3
                    };

                    g.FillEllipse(miBrushPtsGranulares, rect);
                }

                var ptsRecoCrudos = Convertir_PtsHisto_En_PtsReco(ptsHisto, granular: false);

                if (ptsRecoCrudos.Any())
                {
                    var pf = ptsRecoCrudos.Last();
                    var rect = new Rectangle
                    {
                        X = pf.X - 4,
                        Y = pf.Y - 4,
                        Height = 9,
                        Width = 9,
                    };

                    if (horaEnCruzVisible)
                    {
                        string sHoraActual = "00:00:00";
                        if (ptsHisto.Any())
                        {
                            sHoraActual = ptsHisto.Last().Fecha.ToString("HH:mm:ss");
                        }
                        Rectangle rectHoraCruz = new Rectangle
                        {
                            X = pf.X - 100,
                            Y = pf.Y - 100,
                            Width = 200,
                            Height = 20,
                        };
                        g.FillRectangle(new SolidBrush(Color.DimGray), rectHoraCruz);
                        g.DrawString(
                            sHoraActual,
                            new Font("Arial", 16, FontStyle.Regular, GraphicsUnit.Pixel),
                            new SolidBrush(Color.Cyan),
                            new Point(pf.X - 100 + 1, pf.Y - 100 + 1)
                        );
                    }

                    if (cruzVisible)
                    {
                        g.DrawLine(new Pen(Color.WhiteSmoke, 1), new Point(pf.X, pf.Y - 50), new Point(pf.X, pf.Y + 50));
                        g.DrawLine(new Pen(Color.WhiteSmoke, 1), new Point(pf.X - 50, pf.Y), new Point(pf.X + 50, pf.Y));
                        g.DrawRectangle(new Pen(Color.WhiteSmoke, 1), new Rectangle { X = pf.X - 100, Y = pf.Y - 100, Width = 200, Height = 200 });
                    }

                    g.FillEllipse(new SolidBrush(Color.Lime), rect);
                    g.DrawEllipse(new Pen(Color.Fuchsia, 2), rect);
                }

                foreach (Point px in ptsRecoCrudos)
                {
                    var rect = new Rectangle
                    {
                        X = px.X - 2,
                        Y = px.Y - 2,
                        Height = 5,
                        Width = 5 ,
                    };

                    g.FillEllipse(miBrushPtsCrudos, rect);
                }
            }
        }

        void PintorCapaBaseActividad(Graphics g)
        {
            g.DrawImage(_capaBaseActividad, new Point(0, 0));
        }

        void PintorIndicadorActividad(Graphics g)
        {
            var diff   = SubRangoHoraActual - _basicData.Desde;
            var altura = _capaBaseActividad.Height;
            var ancho  = GetLargoMinutosSubRango();

            var x = Convert.ToInt32(diff.TotalMinutes);
            
            var brushCuerpo = new SolidBrush(Color.FromArgb(128, Color.YellowGreen));
            var penRaya     = new Pen(Color.FromArgb(255, Color.YellowGreen));

            var rect = new Rectangle
            {
                X = x - ancho,
                Y = 5,
                Height = altura-10,
                Width = ancho,
            };

            g.FillRectangle(brushCuerpo, rect);
            g.DrawLine(penRaya, x, 0, x, altura);
        }

        IEnumerable<PuntoHistorico> Get_PtsHisto_Por_DesdeHasta(int ficha, DateTime desde, DateTime hasta)
        {
            return _datosCoches[ficha]
                .PuntosHistoricos
                .Where(ph => 
                    ph.Lat != 0 &&
                    ph.Lng != 0 &&
                    ph.Fecha >= desde &&
                    ph.Fecha < hasta
                )
            ;
        }

        IEnumerable<Point> Convertir_PtsHisto_En_PtsReco(IEnumerable<PuntoHistorico> puntosHistoricos, bool granular)
        {
            var iter = granular ?
                CrearPuntosRecorrido(puntosHistoricos).HacerGranular(_granularidad) :
                CrearPuntosRecorrido(puntosHistoricos);

            foreach (var ppx in iter)
            {
                var casppx = Casillero.Create(_topes2D, ppx, _granularidad);
                var x = casppx.IndexHorizontal;
                var y = casppx.IndexVertical;
                yield return new Point(x, y);
            }
        }

        IEnumerable<Point> Get_PtsReco_Por_DesdeHasta(int ficha, DateTime desde, DateTime hasta, bool granular)
        {
            var puntosHistoricos = Get_PtsHisto_Por_DesdeHasta(
                ficha, 
                desde, 
                hasta
            );

            return Convertir_PtsHisto_En_PtsReco(puntosHistoricos, granular);
        }

        private IEnumerable<PuntoRecorrido> CrearPuntosRecorrido(IEnumerable<Punto> puntos)
        {
            int contador = 1;

            foreach (var px in puntos)
            {
                yield return new PuntoRecorrido
                {
                    Cuenta = contador,
                    Lat = px.Lat,
                    Lng = px.Lng,
                };

                contador++;
            }
        }

        void ActualizarTodo()
        {
            if (_capaBaseMain != null && _pintoresMain != null)
            {
                picMain.Image = CrearBitmap(_capaBaseMain, _pintoresMain);
            }

            if (_capaBaseActividad != null && _pintoresActividad != null)
            {
                picActividad.Image = CrearBitmap(_capaBaseActividad, _pintoresActividad);
            }

            /***********************************************

              +-------------------------------+
              |                               |
            +----------------+                |
            |                |                |
            |   x            |                |
            |                |                |
            |                |                |
            +----------------+                |
              |                               |
              +-------------------------------+

             ***********************************************/

            if (_basicData != null)
            {
                lblHoraActual.Text  = _basicData.Desde.AddSeconds(OffSetSegundosDesde).ToString("HH:mm");
                lblFechaActual.Text = _basicData.Desde.AddSeconds(OffSetSegundosDesde).ToString("dd MMM").Replace(".", string.Empty);
            }

            Refresh();
            GC.Collect();
        }

        private void BtnShowHideSidePanel_Click(object sender, EventArgs e)
        {
            if (btnShowHideSidePanel.Text == ">>")
            {
                panSidePanel.Width = 40 + 6 + 6;
                btnShowHideSidePanel.Text = "<<";
            }
            else
            {
                panSidePanel.Width = 400;
                btnShowHideSidePanel.Text = ">>";
            }

            FrmMain_Resize(this, new EventArgs());
        }

        private void ChkCentrar_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarTodo();
        }

        private void CmbLargoSubRango_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarTodo();
        }

        private void CmbLineas_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sLineasKey = cmbLineas.Items[cmbLineas.SelectedIndex]
                .ToString()
                .Trim()
            ;

            txtLineas.Text = Program.Lineas[sLineasKey];
        }
    }

    public class Error
    { 
        public string Mensaje { get; set; }
    }

    public class BasicData
    {
        public List<int> Fichas { get; set; }
        public List<int> Lineas { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
    }
}
