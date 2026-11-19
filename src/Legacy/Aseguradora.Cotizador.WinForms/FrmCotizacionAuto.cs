using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using Aseguradora.Tarifas;

namespace Aseguradora.Cotizador.WinForms
{
    /// <summary>
    /// Cotizador de seguro de auto (ventanilla). En producción desde 2011.
    /// Datos y reglas ficticias para la demo.
    /// </summary>
    public partial class FrmCotizacionAuto : Form
    {
        public FrmCotizacionAuto()
        {
            InitializeComponent();
        }

        private void FrmCotizacionAuto_Load(object sender, EventArgs e)
        {
            cboTipoVehiculo.Items.AddRange(new object[] { "SEDAN", "PICKUP", "MOTO", "MICROBUS" });
            cboZona.Items.AddRange(new object[] { "URBANA", "RURAL" });
            cboTipoVehiculo.SelectedIndex = 0;
            cboZona.SelectedIndex = 0;
        }

        // TODO: mover esto a una clase aparte (2014)
        private void btnCalcular_Click(object sender, EventArgs e)
        {
            decimal suma;
            if (!decimal.TryParse(txtSumaAsegurada.Text, out suma) || suma <= 0)
            {
                MessageBox.Show("Ingrese una suma asegurada válida.", "Cotizador",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int anioVehiculo;
            if (!int.TryParse(txtAnioVehiculo.Text, out anioVehiculo))
            {
                MessageBox.Show("Ingrese el año del vehículo.", "Cotizador",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipo = cboTipoVehiculo.Text;
            string zona = cboZona.Text;
            DateTime nacimiento = dtpNacimiento.Value;
            int siniestros = (int)nudSiniestros.Value;

            // Prima base según tabla de tarifas (módulo VB)
            decimal prima = suma * ModTarifas.ObtenerTasaBase(tipo);

            // Recargo por edad del conductor
            int edad = DateTime.Now.Year - nacimiento.Year;
            if (edad < 25)
            {
                prima = prima * 1.25m;
            }
            else if (edad > 65)
            {
                prima = prima * 1.15m;
            }

            // Recargo por antigüedad del vehículo (manual de suscripción: "10 años o más")
            int antiguedad = DateTime.Now.Year - anioVehiculo;
            if (antiguedad > 10)
            {
                prima = prima * 1.20m;
            }

            // Ajuste por zona
            prima = prima * ModTarifas.FactorZona(zona);

            // Prima mínima de la póliza
            if (prima < 150)
            {
                prima = 150;
            }

            // Bonificación / recargo por siniestralidad (últimos 3 años)
            if (siniestros == 0)
            {
                prima = prima * 0.90m;
            }
            else if (siniestros >= 3)
            {
                prima = prima * 1.30m;
            }

            prima = Math.Round(prima, 2);
            decimal iva = Math.Round(prima * 0.13m, 2);
            decimal total = prima + iva;

            lblPrimaNeta.Text = prima.ToString("N2");
            lblIva.Text = iva.ToString("N2");
            lblTotal.Text = total.ToString("N2");

            GuardarCotizacion(suma, tipo, anioVehiculo, nacimiento, siniestros, zona, prima, iva, total);
        }

        private void GuardarCotizacion(decimal suma, string tipo, int anioVehiculo, DateTime nacimiento,
            int siniestros, string zona, decimal prima, decimal iva, decimal total)
        {
            if (ConfigurationManager.AppSettings["GuardarEnBD"] != "true")
            {
                return;
            }

            try
            {
                string cadena = ConfigurationManager.ConnectionStrings["Cotizaciones"].ConnectionString;
                using (var conexion = new SqlConnection(cadena))
                using (var comando = new SqlCommand(
                    "INSERT INTO Cotizacion (SumaAsegurada, TipoVehiculo, AnioVehiculo, FechaNacimiento, Siniestros, Zona, PrimaNeta, Iva, Total, Fecha) " +
                    "VALUES (@suma, @tipo, @anio, @nac, @sin, @zona, @prima, @iva, @total, GETDATE())", conexion))
                {
                    comando.Parameters.AddWithValue("@suma", suma);
                    comando.Parameters.AddWithValue("@tipo", tipo);
                    comando.Parameters.AddWithValue("@anio", anioVehiculo);
                    comando.Parameters.AddWithValue("@nac", nacimiento);
                    comando.Parameters.AddWithValue("@sin", siniestros);
                    comando.Parameters.AddWithValue("@zona", zona);
                    comando.Parameters.AddWithValue("@prima", prima);
                    comando.Parameters.AddWithValue("@iva", iva);
                    comando.Parameters.AddWithValue("@total", total);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Se ignora a propósito: "el cotizador no puede dejar de cotizar porque la BD falle" (2013)
                Trace.TraceError("No se pudo guardar la cotización: " + ex.Message);
            }
        }
    }
}
