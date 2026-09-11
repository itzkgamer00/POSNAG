namespace CapaPresentacion.Formularios
{
    partial class Cierre
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblCaja = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblFechaHora = new System.Windows.Forms.Label();
            this.lblMontos = new System.Windows.Forms.Label();
            this.lblObservaciones = new System.Windows.Forms.Label();
            this.txtCaja = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtUsuario = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtFechaHora = new Guna.UI2.WinForms.Guna2TextBox();
            this.dgvMontos = new System.Windows.Forms.DataGridView();
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.btnCerrarCaja = new Guna.UI2.WinForms.Guna2Button();
            this.btnCancelarCierre = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMontos)).BeginInit();
            this.SuspendLayout();
            //
            // panel1
            //
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lblTitulo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(981, 61);
            this.panel1.TabIndex = 0;
            //
            // lblTitulo
            //
            this.lblTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.lblTitulo.Font = new System.Drawing.Font("Bahnschrift", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(3, -2);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(400, 57);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "CIERRE DE CAJA";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblCaja
            //
            this.lblCaja.AutoSize = true;
            this.lblCaja.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaja.Location = new System.Drawing.Point(19, 75);
            this.lblCaja.Name = "lblCaja";
            this.lblCaja.Size = new System.Drawing.Size(60, 25);
            this.lblCaja.TabIndex = 1;
            this.lblCaja.Text = "Caja";
            //
            // lblUsuario
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.Location = new System.Drawing.Point(504, 75);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(88, 25);
            this.lblUsuario.TabIndex = 2;
            this.lblUsuario.Text = "Usuario";
            //
            // lblFechaHora
            //
            this.lblFechaHora.AutoSize = true;
            this.lblFechaHora.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaHora.Location = new System.Drawing.Point(19, 165);
            this.lblFechaHora.Name = "lblFechaHora";
            this.lblFechaHora.Size = new System.Drawing.Size(180, 25);
            this.lblFechaHora.TabIndex = 3;
            this.lblFechaHora.Text = "Fecha y Hora de Cierre";
            //
            // lblMontos
            //
            this.lblMontos.AutoSize = true;
            this.lblMontos.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMontos.Location = new System.Drawing.Point(19, 255);
            this.lblMontos.Name = "lblMontos";
            this.lblMontos.Size = new System.Drawing.Size(300, 25);
            this.lblMontos.TabIndex = 4;
            this.lblMontos.Text = "Conteo Final por Moneda";
            //
            // lblObservaciones
            //
            this.lblObservaciones.AutoSize = true;
            this.lblObservaciones.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObservaciones.Location = new System.Drawing.Point(19, 458);
            this.lblObservaciones.Name = "lblObservaciones";
            this.lblObservaciones.Size = new System.Drawing.Size(180, 25);
            this.lblObservaciones.TabIndex = 5;
            this.lblObservaciones.Text = "Observaciones";
            //
            // txtCaja
            //
            this.txtCaja.BackColor = System.Drawing.Color.Transparent;
            this.txtCaja.BorderRadius = 6;
            this.txtCaja.DefaultText = "";
            this.txtCaja.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCaja.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCaja.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCaja.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCaja.FillColor = System.Drawing.Color.Silver;
            this.txtCaja.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCaja.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCaja.Location = new System.Drawing.Point(18, 103);
            this.txtCaja.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtCaja.Name = "txtCaja";
            this.txtCaja.PlaceholderText = "";
            this.txtCaja.ReadOnly = true;
            this.txtCaja.SelectedText = "";
            this.txtCaja.Size = new System.Drawing.Size(465, 48);
            this.txtCaja.TabIndex = 6;
            //
            // txtUsuario
            //
            this.txtUsuario.BackColor = System.Drawing.Color.Transparent;
            this.txtUsuario.BorderRadius = 6;
            this.txtUsuario.DefaultText = "";
            this.txtUsuario.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtUsuario.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtUsuario.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtUsuario.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtUsuario.FillColor = System.Drawing.Color.Silver;
            this.txtUsuario.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsuario.Location = new System.Drawing.Point(509, 103);
            this.txtUsuario.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.PlaceholderText = "";
            this.txtUsuario.ReadOnly = true;
            this.txtUsuario.SelectedText = "";
            this.txtUsuario.Size = new System.Drawing.Size(445, 48);
            this.txtUsuario.TabIndex = 7;
            //
            // txtFechaHora
            //
            this.txtFechaHora.BackColor = System.Drawing.Color.Transparent;
            this.txtFechaHora.BorderRadius = 6;
            this.txtFechaHora.DefaultText = "";
            this.txtFechaHora.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtFechaHora.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtFechaHora.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFechaHora.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFechaHora.FillColor = System.Drawing.Color.Silver;
            this.txtFechaHora.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFechaHora.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFechaHora.Location = new System.Drawing.Point(18, 193);
            this.txtFechaHora.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtFechaHora.Name = "txtFechaHora";
            this.txtFechaHora.PlaceholderText = "";
            this.txtFechaHora.ReadOnly = true;
            this.txtFechaHora.SelectedText = "";
            this.txtFechaHora.Size = new System.Drawing.Size(465, 48);
            this.txtFechaHora.TabIndex = 8;
            //
            // dgvMontos
            //
            this.dgvMontos.AllowUserToAddRows = false;
            this.dgvMontos.AllowUserToDeleteRows = false;
            this.dgvMontos.AllowUserToResizeRows = false;
            this.dgvMontos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMontos.Location = new System.Drawing.Point(18, 283);
            this.dgvMontos.Name = "dgvMontos";
            this.dgvMontos.RowHeadersVisible = false;
            this.dgvMontos.RowTemplate.Height = 32;
            this.dgvMontos.Size = new System.Drawing.Size(936, 160);
            this.dgvMontos.TabIndex = 9;
            //
            // txtObservaciones
            //
            this.txtObservaciones.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtObservaciones.Location = new System.Drawing.Point(18, 486);
            this.txtObservaciones.Multiline = true;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.Size = new System.Drawing.Size(570, 90);
            this.txtObservaciones.TabIndex = 10;
            //
            // btnCerrarCaja
            //
            this.btnCerrarCaja.BorderRadius = 10;
            this.btnCerrarCaja.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrarCaja.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrarCaja.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCerrarCaja.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCerrarCaja.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnCerrarCaja.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrarCaja.ForeColor = System.Drawing.Color.White;
            this.btnCerrarCaja.Image = global::CapaPresentacion.Properties.Resources.Checked_Checkbox;
            this.btnCerrarCaja.Location = new System.Drawing.Point(647, 486);
            this.btnCerrarCaja.Name = "btnCerrarCaja";
            this.btnCerrarCaja.Size = new System.Drawing.Size(307, 46);
            this.btnCerrarCaja.TabIndex = 11;
            this.btnCerrarCaja.Text = "Cerrar Caja";
            this.btnCerrarCaja.Click += new System.EventHandler(this.btnCerrarCaja_Click);
            //
            // btnCancelarCierre
            //
            this.btnCancelarCierre.BorderRadius = 10;
            this.btnCancelarCierre.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCancelarCierre.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCancelarCierre.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCancelarCierre.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCancelarCierre.FillColor = System.Drawing.Color.Silver;
            this.btnCancelarCierre.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelarCierre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.btnCancelarCierre.Image = global::CapaPresentacion.Properties.Resources.Close_Window;
            this.btnCancelarCierre.Location = new System.Drawing.Point(647, 538);
            this.btnCancelarCierre.Name = "btnCancelarCierre";
            this.btnCancelarCierre.Size = new System.Drawing.Size(307, 38);
            this.btnCancelarCierre.TabIndex = 12;
            this.btnCancelarCierre.Text = "Cancelar";
            this.btnCancelarCierre.Click += new System.EventHandler(this.btnCancelarCierre_Click);
            //
            // guna2Separator1
            //
            this.guna2Separator1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Separator1.FillColor = System.Drawing.Color.Gray;
            this.guna2Separator1.FillStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            this.guna2Separator1.FillThickness = 3;
            this.guna2Separator1.Location = new System.Drawing.Point(647, 583);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(307, 8);
            this.guna2Separator1.TabIndex = 13;
            //
            // Cierre
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(981, 601);
            this.Controls.Add(this.guna2Separator1);
            this.Controls.Add(this.btnCancelarCierre);
            this.Controls.Add(this.btnCerrarCaja);
            this.Controls.Add(this.txtObservaciones);
            this.Controls.Add(this.dgvMontos);
            this.Controls.Add(this.txtFechaHora);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.txtCaja);
            this.Controls.Add(this.lblObservaciones);
            this.Controls.Add(this.lblMontos);
            this.Controls.Add(this.lblFechaHora);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.lblCaja);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Cierre";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cierre de Caja";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMontos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblCaja;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblFechaHora;
        private System.Windows.Forms.Label lblMontos;
        private System.Windows.Forms.Label lblObservaciones;
        private Guna.UI2.WinForms.Guna2TextBox txtCaja;
        private Guna.UI2.WinForms.Guna2TextBox txtUsuario;
        private Guna.UI2.WinForms.Guna2TextBox txtFechaHora;
        private System.Windows.Forms.DataGridView dgvMontos;
        private System.Windows.Forms.TextBox txtObservaciones;
        private Guna.UI2.WinForms.Guna2Button btnCerrarCaja;
        private Guna.UI2.WinForms.Guna2Button btnCancelarCierre;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
    }
}
