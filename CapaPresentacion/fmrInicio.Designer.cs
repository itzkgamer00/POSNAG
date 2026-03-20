namespace CapaPresentacion
{
    partial class fmrInicio
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmrInicio));
            this.Pnmain = new System.Windows.Forms.Panel();
            this.PnInformacion = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.Pnsuperior = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Pninferior = new System.Windows.Forms.Panel();
            this.lblHora = new System.Windows.Forms.Label();
            this.hora = new System.Windows.Forms.Label();
            this.lblfecha = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PanelContenedor = new System.Windows.Forms.Panel();
            this.tmTiempo = new System.Windows.Forms.Timer(this.components);
            this.guna2ContextMenuStrip1 = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            this.iconButton4 = new FontAwesome.Sharp.IconButton();
            this.btnHome = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button2 = new Guna.UI2.WinForms.Guna2Button();
            this.btnControlEfectivo = new Guna.UI2.WinForms.Guna2Button();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.Pnmain.SuspendLayout();
            this.PnInformacion.SuspendLayout();
            this.Pnsuperior.SuspendLayout();
            this.Pninferior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Pnmain
            // 
            this.Pnmain.BackColor = System.Drawing.Color.DimGray;
            this.Pnmain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Pnmain.Controls.Add(this.btnHome);
            this.Pnmain.Controls.Add(this.guna2Button2);
            this.Pnmain.Controls.Add(this.btnControlEfectivo);
            this.Pnmain.Controls.Add(this.PnInformacion);
            this.Pnmain.Dock = System.Windows.Forms.DockStyle.Left;
            this.Pnmain.Location = new System.Drawing.Point(0, 0);
            this.Pnmain.Margin = new System.Windows.Forms.Padding(2);
            this.Pnmain.Name = "Pnmain";
            this.Pnmain.Size = new System.Drawing.Size(324, 745);
            this.Pnmain.TabIndex = 16;
            // 
            // PnInformacion
            // 
            this.PnInformacion.BackColor = System.Drawing.Color.DimGray;
            this.PnInformacion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PnInformacion.Controls.Add(this.label7);
            this.PnInformacion.Controls.Add(this.guna2CirclePictureBox1);
            this.PnInformacion.Controls.Add(this.label5);
            this.PnInformacion.Location = new System.Drawing.Point(0, 0);
            this.PnInformacion.Margin = new System.Windows.Forms.Padding(2);
            this.PnInformacion.Name = "PnInformacion";
            this.PnInformacion.Size = new System.Drawing.Size(317, 210);
            this.PnInformacion.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(782, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(237, 27);
            this.label2.TabIndex = 18;
            this.label2.Text = "Nombre de La Empresa";
            // 
            // Pnsuperior
            // 
            this.Pnsuperior.BackColor = System.Drawing.Color.Gainsboro;
            this.Pnsuperior.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Pnsuperior.Controls.Add(this.iconButton4);
            this.Pnsuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Pnsuperior.Location = new System.Drawing.Point(324, 0);
            this.Pnsuperior.Margin = new System.Windows.Forms.Padding(2);
            this.Pnsuperior.Name = "Pnsuperior";
            this.Pnsuperior.Size = new System.Drawing.Size(1042, 95);
            this.Pnsuperior.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(137, 168);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label7.Size = new System.Drawing.Size(48, 23);
            this.label7.TabIndex = 18;
            this.label7.Text = "ROL ";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(119, 134);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label5.Size = new System.Drawing.Size(85, 23);
            this.label5.TabIndex = 16;
            this.label5.Text = "USUARIO";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Pninferior
            // 
            this.Pninferior.BackColor = System.Drawing.Color.Gainsboro;
            this.Pninferior.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Pninferior.Controls.Add(this.lblHora);
            this.Pninferior.Controls.Add(this.label2);
            this.Pninferior.Controls.Add(this.hora);
            this.Pninferior.Controls.Add(this.lblfecha);
            this.Pninferior.Controls.Add(this.label1);
            this.Pninferior.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Pninferior.Location = new System.Drawing.Point(324, 671);
            this.Pninferior.Margin = new System.Windows.Forms.Padding(2);
            this.Pninferior.Name = "Pninferior";
            this.Pninferior.Size = new System.Drawing.Size(1042, 74);
            this.Pninferior.TabIndex = 18;
            // 
            // lblHora
            // 
            this.lblHora.AutoSize = true;
            this.lblHora.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHora.ForeColor = System.Drawing.Color.Blue;
            this.lblHora.Location = new System.Drawing.Point(537, 29);
            this.lblHora.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHora.Name = "lblHora";
            this.lblHora.Size = new System.Drawing.Size(59, 27);
            this.lblHora.TabIndex = 3;
            this.lblHora.Text = "Hora";
            // 
            // hora
            // 
            this.hora.AutoSize = true;
            this.hora.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hora.Location = new System.Drawing.Point(442, 30);
            this.hora.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.hora.Name = "hora";
            this.hora.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.hora.Size = new System.Drawing.Size(70, 23);
            this.hora.TabIndex = 2;
            this.hora.Text = "HORA:";
            this.hora.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblfecha
            // 
            this.lblfecha.AutoSize = true;
            this.lblfecha.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfecha.ForeColor = System.Drawing.Color.Blue;
            this.lblfecha.Location = new System.Drawing.Point(120, 29);
            this.lblfecha.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblfecha.Name = "lblfecha";
            this.lblfecha.Size = new System.Drawing.Size(67, 27);
            this.lblfecha.TabIndex = 1;
            this.lblfecha.Text = "Fecha";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 29);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(77, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "FECHA:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PanelContenedor
            // 
            this.PanelContenedor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PanelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelContenedor.Location = new System.Drawing.Point(324, 95);
            this.PanelContenedor.Margin = new System.Windows.Forms.Padding(2);
            this.PanelContenedor.Name = "PanelContenedor";
            this.PanelContenedor.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PanelContenedor.Size = new System.Drawing.Size(1042, 576);
            this.PanelContenedor.TabIndex = 18;
            // 
            // tmTiempo
            // 
            this.tmTiempo.Enabled = true;
            this.tmTiempo.Tick += new System.EventHandler(this.tmTiempo_Tick);
            // 
            // guna2ContextMenuStrip1
            // 
            this.guna2ContextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.guna2ContextMenuStrip1.Name = "guna2ContextMenuStrip1";
            this.guna2ContextMenuStrip1.RenderStyle.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(143)))), ((int)(((byte)(255)))));
            this.guna2ContextMenuStrip1.RenderStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.guna2ContextMenuStrip1.RenderStyle.ColorTable = null;
            this.guna2ContextMenuStrip1.RenderStyle.RoundedEdges = true;
            this.guna2ContextMenuStrip1.RenderStyle.SelectionArrowColor = System.Drawing.Color.White;
            this.guna2ContextMenuStrip1.RenderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.guna2ContextMenuStrip1.RenderStyle.SelectionForeColor = System.Drawing.Color.White;
            this.guna2ContextMenuStrip1.RenderStyle.SeparatorColor = System.Drawing.Color.Gainsboro;
            this.guna2ContextMenuStrip1.RenderStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.guna2ContextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // iconButton4
            // 
            this.iconButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconButton4.BackColor = System.Drawing.Color.Salmon;
            this.iconButton4.FlatAppearance.BorderColor = System.Drawing.Color.DarkRed;
            this.iconButton4.FlatAppearance.BorderSize = 4;
            this.iconButton4.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.iconButton4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.iconButton4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.iconButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton4.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton4.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.iconButton4.IconColor = System.Drawing.Color.Black;
            this.iconButton4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton4.IconSize = 20;
            this.iconButton4.Location = new System.Drawing.Point(958, 11);
            this.iconButton4.Margin = new System.Windows.Forms.Padding(2);
            this.iconButton4.Name = "iconButton4";
            this.iconButton4.Size = new System.Drawing.Size(68, 64);
            this.iconButton4.TabIndex = 15;
            this.iconButton4.Text = "SALIR";
            this.iconButton4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.iconButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.iconButton4.UseVisualStyleBackColor = false;
            this.iconButton4.Click += new System.EventHandler(this.iconButton4_Click);
            // 
            // btnHome
            // 
            this.btnHome.BorderColor = System.Drawing.Color.White;
            this.btnHome.BorderRadius = 4;
            this.btnHome.BorderThickness = 3;
            this.btnHome.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHome.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHome.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHome.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHome.FillColor = System.Drawing.Color.DimGray;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHome.ForeColor = System.Drawing.Color.Black;
            this.btnHome.Location = new System.Drawing.Point(10, 215);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(296, 48);
            this.btnHome.TabIndex = 24;
            this.btnHome.Text = "home";
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // guna2Button2
            // 
            this.guna2Button2.BorderColor = System.Drawing.Color.White;
            this.guna2Button2.BorderRadius = 4;
            this.guna2Button2.BorderThickness = 3;
            this.guna2Button2.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button2.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button2.FillColor = System.Drawing.Color.DimGray;
            this.guna2Button2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2Button2.ForeColor = System.Drawing.Color.Black;
            this.guna2Button2.Location = new System.Drawing.Point(10, 323);
            this.guna2Button2.Name = "guna2Button2";
            this.guna2Button2.Size = new System.Drawing.Size(296, 48);
            this.guna2Button2.TabIndex = 23;
            this.guna2Button2.Text = "Configuracion";
            // 
            // btnControlEfectivo
            // 
            this.btnControlEfectivo.BorderColor = System.Drawing.Color.White;
            this.btnControlEfectivo.BorderRadius = 4;
            this.btnControlEfectivo.BorderThickness = 3;
            this.btnControlEfectivo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnControlEfectivo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnControlEfectivo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnControlEfectivo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnControlEfectivo.FillColor = System.Drawing.Color.DimGray;
            this.btnControlEfectivo.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnControlEfectivo.ForeColor = System.Drawing.Color.Black;
            this.btnControlEfectivo.Location = new System.Drawing.Point(10, 269);
            this.btnControlEfectivo.Name = "btnControlEfectivo";
            this.btnControlEfectivo.Padding = new System.Windows.Forms.Padding(2);
            this.btnControlEfectivo.Size = new System.Drawing.Size(296, 48);
            this.btnControlEfectivo.TabIndex = 22;
            this.btnControlEfectivo.Text = "Movimientos";
            this.btnControlEfectivo.Click += new System.EventHandler(this.btnControlEfectivo_Click);
            // 
            // guna2CirclePictureBox1
            // 
            this.guna2CirclePictureBox1.ImageRotate = 0F;
            this.guna2CirclePictureBox1.Location = new System.Drawing.Point(93, 3);
            this.guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            this.guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox1.Size = new System.Drawing.Size(135, 128);
            this.guna2CirclePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.guna2CirclePictureBox1.TabIndex = 0;
            this.guna2CirclePictureBox1.TabStop = false;
            // 
            // fmrInicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1366, 745);
            this.Controls.Add(this.PanelContenedor);
            this.Controls.Add(this.Pninferior);
            this.Controls.Add(this.Pnsuperior);
            this.Controls.Add(this.Pnmain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "fmrInicio";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.fmrInicio_Load);
            this.Pnmain.ResumeLayout(false);
            this.PnInformacion.ResumeLayout(false);
            this.PnInformacion.PerformLayout();
            this.Pnsuperior.ResumeLayout(false);
            this.Pninferior.ResumeLayout(false);
            this.Pninferior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private FontAwesome.Sharp.IconButton iconButton4;
        private System.Windows.Forms.Panel Pnmain;
        private System.Windows.Forms.Panel PnInformacion;
        private System.Windows.Forms.Panel Pnsuperior;
        private System.Windows.Forms.Panel Pninferior;
        private System.Windows.Forms.Panel PanelContenedor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblHora;
        private System.Windows.Forms.Label hora;
        private System.Windows.Forms.Label lblfecha;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer tmTiempo;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2Button btnControlEfectivo;
        private Guna.UI2.WinForms.Guna2Button guna2Button2;
        private Guna.UI2.WinForms.Guna2Button btnHome;
        private Guna.UI2.WinForms.Guna2ContextMenuStrip guna2ContextMenuStrip1;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
    }
}

