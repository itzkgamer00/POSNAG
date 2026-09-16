namespace CapaPresentacion.Formularios
{
    partial class FrmUsuario
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
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.btnCerrar = new Guna.UI2.WinForms.Guna2Button();
            this.lblNombreCompleto = new System.Windows.Forms.Label();
            this.txtNombreCompleto = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtContrasena = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblConfirmar = new System.Windows.Forms.Label();
            this.txtConfirmarContrasena = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblRol = new System.Windows.Forms.Label();
            this.cboRol = new Guna.UI2.WinForms.Guna2ComboBox();
            this.chkActivo = new Guna.UI2.WinForms.Guna2CheckBox();
            this.btnGuardar = new Guna.UI2.WinForms.Guna2Button();
            this.btnCancelar = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(52)))), ((int)(((byte)(60)))));
            this.guna2Panel1.Controls.Add(this.lblTitulo);
            this.guna2Panel1.Controls.Add(this.lblSubtitulo);
            this.guna2Panel1.Controls.Add(this.btnCerrar);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(535, 81);
            this.guna2Panel1.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(11, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(212, 33);
            this.lblTitulo.TabIndex = 2;
            this.lblTitulo.Text = "Nuevo Usuario";
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitulo.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblSubtitulo.Location = new System.Drawing.Point(11, 51);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(393, 19);
            this.lblSubtitulo.TabIndex = 3;
            this.lblSubtitulo.Text = "Complete los datos para crear un nuevo usuario.";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.BorderRadius = 9;
            this.btnCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCerrar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCerrar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCerrar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Image = global::CapaPresentacion.Properties.Resources.Close;
            this.btnCerrar.Location = new System.Drawing.Point(494, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(38, 33);
            this.btnCerrar.TabIndex = 0;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // lblNombreCompleto
            // 
            this.lblNombreCompleto.AutoSize = true;
            this.lblNombreCompleto.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblNombreCompleto.Location = new System.Drawing.Point(37, 110);
            this.lblNombreCompleto.Name = "lblNombreCompleto";
            this.lblNombreCompleto.Size = new System.Drawing.Size(157, 23);
            this.lblNombreCompleto.TabIndex = 4;
            this.lblNombreCompleto.Text = "Nombre completo";
            // 
            // txtNombreCompleto
            // 
            this.txtNombreCompleto.BorderRadius = 6;
            this.txtNombreCompleto.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNombreCompleto.DefaultText = "";
            this.txtNombreCompleto.FillColor = System.Drawing.Color.LightGray;
            this.txtNombreCompleto.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombreCompleto.ForeColor = System.Drawing.Color.Black;
            this.txtNombreCompleto.Location = new System.Drawing.Point(37, 148);
            this.txtNombreCompleto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNombreCompleto.Name = "txtNombreCompleto";
            this.txtNombreCompleto.PlaceholderText = "";
            this.txtNombreCompleto.SelectedText = "";
            this.txtNombreCompleto.Size = new System.Drawing.Size(460, 40);
            this.txtNombreCompleto.TabIndex = 1;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblUsuario.Location = new System.Drawing.Point(33, 203);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(70, 23);
            this.lblUsuario.TabIndex = 5;
            this.lblUsuario.Text = "Usuario";
            // 
            // txtUsuario
            // 
            this.txtUsuario.BorderRadius = 6;
            this.txtUsuario.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsuario.DefaultText = "";
            this.txtUsuario.FillColor = System.Drawing.Color.LightGray;
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsuario.ForeColor = System.Drawing.Color.Black;
            this.txtUsuario.Location = new System.Drawing.Point(37, 243);
            this.txtUsuario.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.PlaceholderText = "";
            this.txtUsuario.SelectedText = "";
            this.txtUsuario.Size = new System.Drawing.Size(460, 40);
            this.txtUsuario.TabIndex = 2;
            // 
            // lblContrasena
            // 
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblContrasena.Location = new System.Drawing.Point(33, 299);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Size = new System.Drawing.Size(99, 23);
            this.lblContrasena.TabIndex = 6;
            this.lblContrasena.Text = "Contraseña";
            // 
            // txtContrasena
            // 
            this.txtContrasena.BorderRadius = 6;
            this.txtContrasena.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtContrasena.DefaultText = "";
            this.txtContrasena.FillColor = System.Drawing.Color.LightGray;
            this.txtContrasena.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContrasena.ForeColor = System.Drawing.Color.Black;
            this.txtContrasena.Location = new System.Drawing.Point(37, 335);
            this.txtContrasena.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.PasswordChar = '•';
            this.txtContrasena.PlaceholderText = "";
            this.txtContrasena.SelectedText = "";
            this.txtContrasena.Size = new System.Drawing.Size(460, 40);
            this.txtContrasena.TabIndex = 3;
            this.txtContrasena.UseSystemPasswordChar = true;
            // 
            // lblConfirmar
            // 
            this.lblConfirmar.AutoSize = true;
            this.lblConfirmar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblConfirmar.Location = new System.Drawing.Point(36, 388);
            this.lblConfirmar.Name = "lblConfirmar";
            this.lblConfirmar.Size = new System.Drawing.Size(183, 23);
            this.lblConfirmar.TabIndex = 7;
            this.lblConfirmar.Text = "Confirmar contraseña";
            // 
            // txtConfirmarContrasena
            // 
            this.txtConfirmarContrasena.BorderRadius = 6;
            this.txtConfirmarContrasena.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConfirmarContrasena.DefaultText = "";
            this.txtConfirmarContrasena.FillColor = System.Drawing.Color.LightGray;
            this.txtConfirmarContrasena.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfirmarContrasena.ForeColor = System.Drawing.Color.Black;
            this.txtConfirmarContrasena.Location = new System.Drawing.Point(37, 413);
            this.txtConfirmarContrasena.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConfirmarContrasena.Name = "txtConfirmarContrasena";
            this.txtConfirmarContrasena.PasswordChar = '•';
            this.txtConfirmarContrasena.PlaceholderText = "";
            this.txtConfirmarContrasena.SelectedText = "";
            this.txtConfirmarContrasena.Size = new System.Drawing.Size(460, 40);
            this.txtConfirmarContrasena.TabIndex = 4;
            this.txtConfirmarContrasena.UseSystemPasswordChar = true;
            // 
            // lblRol
            // 
            this.lblRol.AutoSize = true;
            this.lblRol.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblRol.Location = new System.Drawing.Point(37, 477);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(36, 23);
            this.lblRol.TabIndex = 8;
            this.lblRol.Text = "Rol";
            // 
            // cboRol
            // 
            this.cboRol.BackColor = System.Drawing.Color.Transparent;
            this.cboRol.BorderRadius = 6;
            this.cboRol.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRol.FocusedColor = System.Drawing.Color.Empty;
            this.cboRol.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cboRol.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboRol.ItemHeight = 30;
            this.cboRol.Location = new System.Drawing.Point(37, 503);
            this.cboRol.Name = "cboRol";
            this.cboRol.Size = new System.Drawing.Size(460, 36);
            this.cboRol.TabIndex = 5;
            // 
            // chkActivo
            // 
            this.chkActivo.Checked = true;
            this.chkActivo.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.chkActivo.CheckedState.BorderRadius = 0;
            this.chkActivo.CheckedState.BorderThickness = 0;
            this.chkActivo.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.ForeColor = System.Drawing.Color.Black;
            this.chkActivo.Location = new System.Drawing.Point(37, 545);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(140, 24);
            this.chkActivo.TabIndex = 6;
            this.chkActivo.Text = "Usuario activo";
            this.chkActivo.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.chkActivo.UncheckedState.BorderRadius = 0;
            this.chkActivo.UncheckedState.BorderThickness = 0;
            this.chkActivo.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.chkActivo.UseVisualStyleBackColor = false;
            this.chkActivo.Visible = false;
            // 
            // btnGuardar
            // 
            this.btnGuardar.BorderRadius = 8;
            this.btnGuardar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGuardar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGuardar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(357, 565);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(140, 45);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BorderRadius = 8;
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCancelar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCancelar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCancelar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCancelar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(184, 565);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(133, 45);
            this.btnCancelar.TabIndex = 8;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // FrmUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(535, 634);
            this.ControlBox = false;
            this.Controls.Add(this.chkActivo);
            this.Controls.Add(this.cboRol);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.txtConfirmarContrasena);
            this.Controls.Add(this.lblConfirmar);
            this.Controls.Add(this.txtContrasena);
            this.Controls.Add(this.lblContrasena);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.txtNombreCompleto);
            this.Controls.Add(this.lblNombreCompleto);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.guna2Panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "FrmUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Button btnCerrar;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblNombreCompleto;
        private Guna.UI2.WinForms.Guna2TextBox txtNombreCompleto;
        private System.Windows.Forms.Label lblUsuario;
        private Guna.UI2.WinForms.Guna2TextBox txtUsuario;
        private System.Windows.Forms.Label lblContrasena;
        private Guna.UI2.WinForms.Guna2TextBox txtContrasena;
        private System.Windows.Forms.Label lblConfirmar;
        private Guna.UI2.WinForms.Guna2TextBox txtConfirmarContrasena;
        private System.Windows.Forms.Label lblRol;
        private Guna.UI2.WinForms.Guna2ComboBox cboRol;
        private Guna.UI2.WinForms.Guna2CheckBox chkActivo;
        private Guna.UI2.WinForms.Guna2Button btnGuardar;
        private Guna.UI2.WinForms.Guna2Button btnCancelar;
    }
}
