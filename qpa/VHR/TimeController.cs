using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VHR
{
    public partial class TimeController : UserControl
    {
        public enum Estados
        {
            SinDatosNecesarios,
            ConDatosNecesarios,
            CargandoPuntos,
            PuntosCargados,
            ErrorDeCarga
        }

        public delegate Task<bool> CargarPuntosEventHandler(object sender, EventArgs e);
        public delegate void AbrirMapaForzadoEventHandler(object sender, EventArgs e);

        public event CargarPuntosEventHandler CargarPuntos;
        public event AbrirMapaForzadoEventHandler AbrirMapaForzado;

        public Estados EstadoActual { get; private set; } = Estados.SinDatosNecesarios;
        
        public List<int> Fichas
        {
            get
            {
                var ret = new List<int>();
                var ficha = int.Parse(txtFicha.Text.Trim());
                ret.Add(ficha);
                return ret;
            }
        }

        public List<int> Lineas
        {
            get
            {
                var partes = txtLineas.Text.Split(
                    new char[] { ',' }, 
                    StringSplitOptions.RemoveEmptyEntries
                );

                return partes
                    .Where (p => p.Trim().Length > 0)
                    .Select(p => p.Trim())
                    .Select(p => int.Parse(p))
                    .ToList()
                ;
            }
        }

        public DateTime Desde
        {
            get
            {
                var fecha = new DateTime(dtpDesde.Value.Year, dtpDesde.Value.Month, dtpDesde.Value.Day);
                return fecha.AddMinutes(trackHoraDesde.Value);
            }
        }

        public DateTime Hasta
        {
            get
            {
                var fecha = new DateTime(dtpDesde.Value.Year, dtpDesde.Value.Month, dtpDesde.Value.Day);
                return fecha.AddMinutes(trackHoraHasta.Value);
            }
        }

        int _valorAnteriorDesde = 0;
        int _valorAnteriorHasta = 0;

        public TimeController()
        {
            InitializeComponent();

            Load += TimeController_Load;
            cmbLineas.SelectedIndexChanged += CmbLineas_SelectedIndexChanged;
            txtFicha.TextChanged += TxtFicha_TextChanged;
            txtLineas.TextChanged += TxtLineas_TextChanged;
        }

        private void TxtFicha_TextChanged(object sender, EventArgs e)
        {
            lblErrorFicha.Visible = !ValidarFicha();
            lblFlechaErrorFicha.Visible = lblErrorFicha.Visible;

            var fichaYLineasOk = ValidarFicha() && ValidarLineas();
            CambiarA(fichaYLineasOk ? Estados.ConDatosNecesarios : Estados.SinDatosNecesarios);
        }

        private void TxtLineas_TextChanged(object sender, EventArgs e)
        {
            lblErrorLineas.Visible = !ValidarLineas();
            lblFlechaErrorLineas.Visible = lblErrorLineas.Visible;

            var fichaYLineasOk = ValidarFicha() && ValidarLineas();
            CambiarA(fichaYLineasOk ? Estados.ConDatosNecesarios : Estados.SinDatosNecesarios);
        }

        private void Actualizar()
        {
            if (EstadoActual == Estados.SinDatosNecesarios)
            {
                HabilitarEntradaDatos();
                Height = 170;
            }
            else if (EstadoActual == Estados.ConDatosNecesarios)
            {
                HabilitarEntradaDatos();
                btnCargarPuntos.Enabled = true;
                Height = 226;
                Visible = true;
            }
            else if (EstadoActual == Estados.CargandoPuntos)
            {
                DeshabilitarEntradaDatos();
                btnCargarPuntos.Enabled = false;
                Height = 472;
            }
            else if (EstadoActual == Estados.PuntosCargados)
            {
                DeshabilitarEntradaDatos();
                Visible = false;
            }
            else if (EstadoActual == Estados.ErrorDeCarga)
            {
                DeshabilitarEntradaDatos();
                Height = 522;
            }
        }

        private void HabilitarEntradaDatos()
        {
            txtFicha.Enabled = true;
            txtLineas.Enabled = true;
            cmbLineas.Enabled = true;
            dtpDesde.Enabled = true;
            trackHoraDesde.Enabled = true;
            trackHoraHasta.Enabled = true;
        }

        private void DeshabilitarEntradaDatos()
        {
            txtFicha.Enabled = false;
            txtLineas.Enabled = false;
            cmbLineas.Enabled = false;
            dtpDesde.Enabled = false;
            trackHoraDesde.Enabled = false;
            trackHoraHasta.Enabled = false;
        }

        private bool EsNumero(string sNumero)
        {
            if (sNumero == null || sNumero == string.Empty)
            {
                return false;
            }

            foreach (char c in sNumero)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }

        private bool EsListaNumeros(string sListaNumeros, char sep)
        {
            var partes = sListaNumeros
                .Split(new char[] { sep }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray()
            ;

            if (partes.Length == 0)
            {
                return false;
            }

            foreach (var parteX in partes)
            {
                if (!EsNumero(parteX))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidarFicha()
        {
            return EsNumero(txtFicha.Text.Trim());
        }

        private bool ValidarLineas()
        {
            return EsListaNumeros(txtLineas.Text.Trim(), ',');
        }

        private void CmbLineas_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cmbLineas.Items[cmbLineas.SelectedIndex];

            if (item == "203")
            {
                txtLineas.Text = "159, 163";
            }
            else if (item == "Grand Bourg")
            {
                txtLineas.Text = "165, 166, 167";
            }
            else if (item == "Todas")
            {
                txtLineas.Text = "159, 163, 165, 166, 167";
            }
            else
            {
                //
            }

            int foo = 0;
        }

        void TimeController_Load(object sender, EventArgs e)
        {
            cmbLineas.Items.Add("");
            cmbLineas.Items.Add("203");
            cmbLineas.Items.Add("Grand Bourg");
            cmbLineas.Items.Add("Todas");

            CambiarA(Estados.SinDatosNecesarios);
        }

        private void TrackHoraDesde_ValueChanged(object sender, EventArgs e)
        {
            var movimientoDeDesde = trackHoraDesde.Value - _valorAnteriorDesde;
            trackHoraHasta.Value = Math.Min(trackHoraHasta.Maximum, _valorAnteriorHasta + movimientoDeDesde);
            _valorAnteriorDesde = trackHoraDesde.Value;

            var dtHoraDesde = new DateTime(2000, 1, 1, 0, 0, 0).AddMinutes(trackHoraDesde.Value);
            var sHoraDesde = dtHoraDesde.ToString("HH:mm:ss");
            lblHoraDesde.Text = sHoraDesde;
        }

        private void TrackHoraHasta_ValueChanged(object sender, EventArgs e)
        {
            if (trackHoraHasta.Value < trackHoraDesde.Value)
            {
                trackHoraHasta.Value = trackHoraDesde.Value;
            }

            _valorAnteriorHasta = trackHoraHasta.Value;
            var dtHoraHasta = new DateTime(2000, 1, 1, 0, 0, 0).AddMinutes(trackHoraHasta.Value);
            var sHoraHasta = dtHoraHasta.ToString("HH:mm:ss");
            lblHoraHasta.Text = sHoraHasta;
        }

        private async void BtnCargarPuntos_Click(object sender, EventArgs e)
        {
            CambiarA(Estados.CargandoPuntos);

            bool resultadoCarga = await CargarPuntos.Invoke(this, new EventArgs());

            if (resultadoCarga)
            {
                CambiarA(Estados.PuntosCargados);
            }
            else
            {
                CambiarA(Estados.ErrorDeCarga);
            }
        }

        public void ConsolePrint(string s)
        {
            lstConsola.Items.Add(s);
            lstConsola.TopIndex = lstConsola.Items.Count - 1;
        }

        private void BtnReintentar_Click(object sender, EventArgs e)
        {
            CambiarA(Estados.ConDatosNecesarios);
        }

        private void BtnAbrirMapaForzado_Click(object sender, EventArgs e)
        {
            AbrirMapaForzado?.Invoke(this, new EventArgs());
        }

        public void CambiarA(Estados estado)
        {
            EstadoActual = estado;
            Actualizar();
        }
    }
}
