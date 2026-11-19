namespace Aseguradora.Cotizador.WinForms
{
    partial class FrmCotizacionAuto
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.lblSuma = new System.Windows.Forms.Label();
            this.txtSumaAsegurada = new System.Windows.Forms.TextBox();
            this.lblTipo = new System.Windows.Forms.Label();
            this.cboTipoVehiculo = new System.Windows.Forms.ComboBox();
            this.lblAnio = new System.Windows.Forms.Label();
            this.txtAnioVehiculo = new System.Windows.Forms.TextBox();
            this.lblNacimiento = new System.Windows.Forms.Label();
            this.dtpNacimiento = new System.Windows.Forms.DateTimePicker();
            this.lblSiniestros = new System.Windows.Forms.Label();
            this.nudSiniestros = new System.Windows.Forms.NumericUpDown();
            this.lblZona = new System.Windows.Forms.Label();
            this.cboZona = new System.Windows.Forms.ComboBox();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.lblPrimaNetaTitulo = new System.Windows.Forms.Label();
            this.lblPrimaNeta = new System.Windows.Forms.Label();
            this.lblIvaTitulo = new System.Windows.Forms.Label();
            this.lblIva = new System.Windows.Forms.Label();
            this.lblTotalTitulo = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudSiniestros)).BeginInit();
            this.SuspendLayout();
            //
            // lblSuma
            //
            this.lblSuma.AutoSize = true;
            this.lblSuma.Location = new System.Drawing.Point(20, 23);
            this.lblSuma.Name = "lblSuma";
            this.lblSuma.Text = "Suma asegurada (US$):";
            //
            // txtSumaAsegurada
            //
            this.txtSumaAsegurada.Location = new System.Drawing.Point(200, 20);
            this.txtSumaAsegurada.Name = "txtSumaAsegurada";
            this.txtSumaAsegurada.Size = new System.Drawing.Size(180, 20);
            this.txtSumaAsegurada.TabIndex = 0;
            //
            // lblTipo
            //
            this.lblTipo.AutoSize = true;
            this.lblTipo.Location = new System.Drawing.Point(20, 58);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Text = "Tipo de vehículo:";
            //
            // cboTipoVehiculo
            //
            this.cboTipoVehiculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoVehiculo.Location = new System.Drawing.Point(200, 55);
            this.cboTipoVehiculo.Name = "cboTipoVehiculo";
            this.cboTipoVehiculo.Size = new System.Drawing.Size(180, 21);
            this.cboTipoVehiculo.TabIndex = 1;
            //
            // lblAnio
            //
            this.lblAnio.AutoSize = true;
            this.lblAnio.Location = new System.Drawing.Point(20, 93);
            this.lblAnio.Name = "lblAnio";
            this.lblAnio.Text = "Año del vehículo:";
            //
            // txtAnioVehiculo
            //
            this.txtAnioVehiculo.Location = new System.Drawing.Point(200, 90);
            this.txtAnioVehiculo.Name = "txtAnioVehiculo";
            this.txtAnioVehiculo.Size = new System.Drawing.Size(80, 20);
            this.txtAnioVehiculo.TabIndex = 2;
            //
            // lblNacimiento
            //
            this.lblNacimiento.AutoSize = true;
            this.lblNacimiento.Location = new System.Drawing.Point(20, 128);
            this.lblNacimiento.Name = "lblNacimiento";
            this.lblNacimiento.Text = "Nacimiento del conductor:";
            //
            // dtpNacimiento
            //
            this.dtpNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNacimiento.Location = new System.Drawing.Point(200, 125);
            this.dtpNacimiento.Name = "dtpNacimiento";
            this.dtpNacimiento.Size = new System.Drawing.Size(120, 20);
            this.dtpNacimiento.TabIndex = 3;
            //
            // lblSiniestros
            //
            this.lblSiniestros.AutoSize = true;
            this.lblSiniestros.Location = new System.Drawing.Point(20, 163);
            this.lblSiniestros.Name = "lblSiniestros";
            this.lblSiniestros.Text = "Siniestros (últimos 3 años):";
            //
            // nudSiniestros
            //
            this.nudSiniestros.Location = new System.Drawing.Point(200, 160);
            this.nudSiniestros.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            this.nudSiniestros.Name = "nudSiniestros";
            this.nudSiniestros.Size = new System.Drawing.Size(60, 20);
            this.nudSiniestros.TabIndex = 4;
            //
            // lblZona
            //
            this.lblZona.AutoSize = true;
            this.lblZona.Location = new System.Drawing.Point(20, 198);
            this.lblZona.Name = "lblZona";
            this.lblZona.Text = "Zona de circulación:";
            //
            // cboZona
            //
            this.cboZona.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZona.Location = new System.Drawing.Point(200, 195);
            this.cboZona.Name = "cboZona";
            this.cboZona.Size = new System.Drawing.Size(180, 21);
            this.cboZona.TabIndex = 5;
            //
            // btnCalcular
            //
            this.btnCalcular.Location = new System.Drawing.Point(200, 235);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(180, 30);
            this.btnCalcular.TabIndex = 6;
            this.btnCalcular.Text = "Calcular prima";
            this.btnCalcular.UseVisualStyleBackColor = true;
            this.btnCalcular.Click += new System.EventHandler(this.btnCalcular_Click);
            //
            // Resultados
            //
            this.lblPrimaNetaTitulo.AutoSize = true;
            this.lblPrimaNetaTitulo.Location = new System.Drawing.Point(20, 290);
            this.lblPrimaNetaTitulo.Name = "lblPrimaNetaTitulo";
            this.lblPrimaNetaTitulo.Text = "Prima neta:";
            this.lblPrimaNeta.AutoSize = true;
            this.lblPrimaNeta.Location = new System.Drawing.Point(200, 290);
            this.lblPrimaNeta.Name = "lblPrimaNeta";
            this.lblPrimaNeta.Text = "0.00";

            this.lblIvaTitulo.AutoSize = true;
            this.lblIvaTitulo.Location = new System.Drawing.Point(20, 315);
            this.lblIvaTitulo.Name = "lblIvaTitulo";
            this.lblIvaTitulo.Text = "IVA (13%):";
            this.lblIva.AutoSize = true;
            this.lblIva.Location = new System.Drawing.Point(200, 315);
            this.lblIva.Name = "lblIva";
            this.lblIva.Text = "0.00";

            this.lblTotalTitulo.AutoSize = true;
            this.lblTotalTitulo.Location = new System.Drawing.Point(20, 340);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Text = "Total a pagar:";
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(200, 340);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Text = "0.00";
            //
            // FrmCotizacionAuto
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 380);
            this.Controls.Add(this.lblSuma);
            this.Controls.Add(this.txtSumaAsegurada);
            this.Controls.Add(this.lblTipo);
            this.Controls.Add(this.cboTipoVehiculo);
            this.Controls.Add(this.lblAnio);
            this.Controls.Add(this.txtAnioVehiculo);
            this.Controls.Add(this.lblNacimiento);
            this.Controls.Add(this.dtpNacimiento);
            this.Controls.Add(this.lblSiniestros);
            this.Controls.Add(this.nudSiniestros);
            this.Controls.Add(this.lblZona);
            this.Controls.Add(this.cboZona);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.lblPrimaNetaTitulo);
            this.Controls.Add(this.lblPrimaNeta);
            this.Controls.Add(this.lblIvaTitulo);
            this.Controls.Add(this.lblIva);
            this.Controls.Add(this.lblTotalTitulo);
            this.Controls.Add(this.lblTotal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmCotizacionAuto";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cotizador de Seguro de Auto — Aseguradora Ficticia S.A.";
            this.Load += new System.EventHandler(this.FrmCotizacionAuto_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudSiniestros)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblSuma;
        private System.Windows.Forms.TextBox txtSumaAsegurada;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cboTipoVehiculo;
        private System.Windows.Forms.Label lblAnio;
        private System.Windows.Forms.TextBox txtAnioVehiculo;
        private System.Windows.Forms.Label lblNacimiento;
        private System.Windows.Forms.DateTimePicker dtpNacimiento;
        private System.Windows.Forms.Label lblSiniestros;
        private System.Windows.Forms.NumericUpDown nudSiniestros;
        private System.Windows.Forms.Label lblZona;
        private System.Windows.Forms.ComboBox cboZona;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.Label lblPrimaNetaTitulo;
        private System.Windows.Forms.Label lblPrimaNeta;
        private System.Windows.Forms.Label lblIvaTitulo;
        private System.Windows.Forms.Label lblIva;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotal;
    }
}
