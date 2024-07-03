namespace CapaPresentacion
{
    partial class Inicio
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
            this.menu = new System.Windows.Forms.MenuStrip();
            this.iconMenuItem1 = new FontAwesome.Sharp.IconMenuItem();
            this.menutitulo = new System.Windows.Forms.MenuStrip();
            this.label1 = new System.Windows.Forms.Label();
            this.iconMenuItem2 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem3 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem4 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem5 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem6 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem7 = new FontAwesome.Sharp.IconMenuItem();
            this.menu.SuspendLayout();
            this.menutitulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.BackColor = System.Drawing.Color.DarkGray;
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iconMenuItem1,
            this.iconMenuItem6,
            this.iconMenuItem5,
            this.iconMenuItem4,
            this.iconMenuItem3,
            this.iconMenuItem2});
            this.menu.Location = new System.Drawing.Point(0, 83);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(793, 64);
            this.menu.TabIndex = 0;
            this.menu.Text = "menuStrip1";
            // 
            // iconMenuItem1
            // 
            this.iconMenuItem1.BackColor = System.Drawing.Color.DarkGray;
            this.iconMenuItem1.Font = new System.Drawing.Font("Bahnschrift", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconMenuItem1.ForeColor = System.Drawing.Color.Navy;
            this.iconMenuItem1.IconChar = FontAwesome.Sharp.IconChar.MoneyBillTransfer;
            this.iconMenuItem1.IconColor = System.Drawing.Color.Green;
            this.iconMenuItem1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem1.IconSize = 40;
            this.iconMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.iconMenuItem1.ImageTransparentColor = System.Drawing.Color.White;
            this.iconMenuItem1.Name = "iconMenuItem1";
            this.iconMenuItem1.Size = new System.Drawing.Size(130, 60);
            this.iconMenuItem1.Text = "Control De Efectivo";
            this.iconMenuItem1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // menutitulo
            // 
            this.menutitulo.AutoSize = false;
            this.menutitulo.BackColor = System.Drawing.Color.MidnightBlue;
            this.menutitulo.Font = new System.Drawing.Font("Bahnschrift", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menutitulo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iconMenuItem7});
            this.menutitulo.Location = new System.Drawing.Point(0, 0);
            this.menutitulo.Name = "menutitulo";
            this.menutitulo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.menutitulo.Size = new System.Drawing.Size(793, 83);
            this.menutitulo.TabIndex = 1;
            this.menutitulo.Text = "menuStrip2";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.BackColor = System.Drawing.Color.MidnightBlue;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(256, 9);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(308, 61);
            this.label1.TabIndex = 2;
            this.label1.Text = "SISTEMA DE CAJA ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // iconMenuItem2
            // 
            this.iconMenuItem2.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.iconMenuItem2.ForeColor = System.Drawing.Color.Navy;
            this.iconMenuItem2.IconChar = FontAwesome.Sharp.IconChar.Receipt;
            this.iconMenuItem2.IconColor = System.Drawing.SystemColors.HotTrack;
            this.iconMenuItem2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem2.IconSize = 40;
            this.iconMenuItem2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.iconMenuItem2.ImageTransparentColor = System.Drawing.Color.White;
            this.iconMenuItem2.Name = "iconMenuItem2";
            this.iconMenuItem2.Size = new System.Drawing.Size(72, 60);
            this.iconMenuItem2.Text = "Reportes";
            this.iconMenuItem2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // iconMenuItem3
            // 
            this.iconMenuItem3.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.iconMenuItem3.ForeColor = System.Drawing.Color.Navy;
            this.iconMenuItem3.IconChar = FontAwesome.Sharp.IconChar.Map;
            this.iconMenuItem3.IconColor = System.Drawing.SystemColors.HotTrack;
            this.iconMenuItem3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem3.IconSize = 40;
            this.iconMenuItem3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.iconMenuItem3.ImageTransparentColor = System.Drawing.Color.White;
            this.iconMenuItem3.Name = "iconMenuItem3";
            this.iconMenuItem3.Size = new System.Drawing.Size(152, 60);
            this.iconMenuItem3.Text = "Listado trasnsacciones";
            this.iconMenuItem3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // iconMenuItem4
            // 
            this.iconMenuItem4.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.iconMenuItem4.ForeColor = System.Drawing.Color.Navy;
            this.iconMenuItem4.IconChar = FontAwesome.Sharp.IconChar.Xmark;
            this.iconMenuItem4.IconColor = System.Drawing.Color.Red;
            this.iconMenuItem4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem4.IconSize = 40;
            this.iconMenuItem4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.iconMenuItem4.ImageTransparentColor = System.Drawing.Color.White;
            this.iconMenuItem4.Name = "iconMenuItem4";
            this.iconMenuItem4.Size = new System.Drawing.Size(103, 60);
            this.iconMenuItem4.Text = "Cierre De Caja";
            this.iconMenuItem4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // iconMenuItem5
            // 
            this.iconMenuItem5.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.iconMenuItem5.ForeColor = System.Drawing.Color.Navy;
            this.iconMenuItem5.IconChar = FontAwesome.Sharp.IconChar.Outdent;
            this.iconMenuItem5.IconColor = System.Drawing.Color.Green;
            this.iconMenuItem5.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem5.IconSize = 40;
            this.iconMenuItem5.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.iconMenuItem5.ImageTransparentColor = System.Drawing.Color.White;
            this.iconMenuItem5.Name = "iconMenuItem5";
            this.iconMenuItem5.Size = new System.Drawing.Size(118, 60);
            this.iconMenuItem5.Text = "Apertura De Caja";
            this.iconMenuItem5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // iconMenuItem6
            // 
            this.iconMenuItem6.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.iconMenuItem6.ForeColor = System.Drawing.Color.Navy;
            this.iconMenuItem6.IconChar = FontAwesome.Sharp.IconChar.Rotate;
            this.iconMenuItem6.IconColor = System.Drawing.Color.Green;
            this.iconMenuItem6.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem6.IconSize = 40;
            this.iconMenuItem6.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.iconMenuItem6.ImageTransparentColor = System.Drawing.Color.White;
            this.iconMenuItem6.Name = "iconMenuItem6";
            this.iconMenuItem6.Size = new System.Drawing.Size(125, 60);
            this.iconMenuItem6.Text = "Cambio de Divisas";
            this.iconMenuItem6.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // iconMenuItem7
            // 
            this.iconMenuItem7.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.iconMenuItem7.ForeColor = System.Drawing.Color.White;
            this.iconMenuItem7.IconChar = FontAwesome.Sharp.IconChar.ArrowAltCircleRight;
            this.iconMenuItem7.IconColor = System.Drawing.Color.Red;
            this.iconMenuItem7.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem7.IconSize = 40;
            this.iconMenuItem7.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.iconMenuItem7.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.iconMenuItem7.Name = "iconMenuItem7";
            this.iconMenuItem7.Size = new System.Drawing.Size(52, 79);
            this.iconMenuItem7.Text = "Salir";
            this.iconMenuItem7.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // Inicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(793, 505);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.menutitulo);
            this.MainMenuStrip = this.menu;
            this.Name = "Inicio";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.menutitulo.ResumeLayout(false);
            this.menutitulo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.MenuStrip menutitulo;
        private System.Windows.Forms.Label label1;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem1;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem2;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem6;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem5;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem4;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem3;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem7;
    }
}

