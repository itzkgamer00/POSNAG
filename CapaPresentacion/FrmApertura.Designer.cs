namespace CapaPresentacion
{
    partial class FrmApertura
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmApertura));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.label1 = new System.Windows.Forms.Label();
            this.lblfecha = new System.Windows.Forms.Label();
            this.lblcajero = new System.Windows.Forms.Label();
            this.lblcaja = new System.Windows.Forms.Label();
            this.lblmontoini = new System.Windows.Forms.Label();
            this.lblobser = new System.Windows.Forms.Label();
            this.lblFechaactual = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnGuardarAprt = new FontAwesome.Sharp.IconButton();
            this.btnCancelarapt = new FontAwesome.Sharp.IconButton();
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.Color.Teal;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(668, 41);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Teal;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Apertura";
            // 
            // lblfecha
            // 
            this.lblfecha.BackColor = System.Drawing.Color.Silver;
            this.lblfecha.Font = new System.Drawing.Font("Bahnschrift", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfecha.Location = new System.Drawing.Point(214, 57);
            this.lblfecha.Name = "lblfecha";
            this.lblfecha.Size = new System.Drawing.Size(198, 34);
            this.lblfecha.TabIndex = 2;
            this.lblfecha.Text = "Fecha De Apertura:";
            this.lblfecha.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblcajero
            // 
            this.lblcajero.BackColor = System.Drawing.Color.Silver;
            this.lblcajero.Font = new System.Drawing.Font("Bahnschrift", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblcajero.Location = new System.Drawing.Point(214, 119);
            this.lblcajero.Name = "lblcajero";
            this.lblcajero.Size = new System.Drawing.Size(199, 34);
            this.lblcajero.TabIndex = 3;
            this.lblcajero.Text = "Cajero";
            this.lblcajero.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblcaja
            // 
            this.lblcaja.BackColor = System.Drawing.Color.Silver;
            this.lblcaja.Font = new System.Drawing.Font("Bahnschrift", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblcaja.Location = new System.Drawing.Point(214, 184);
            this.lblcaja.Name = "lblcaja";
            this.lblcaja.Size = new System.Drawing.Size(200, 34);
            this.lblcaja.TabIndex = 4;
            this.lblcaja.Text = "Caja";
            this.lblcaja.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblmontoini
            // 
            this.lblmontoini.BackColor = System.Drawing.Color.Silver;
            this.lblmontoini.Font = new System.Drawing.Font("Bahnschrift", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblmontoini.Location = new System.Drawing.Point(12, 242);
            this.lblmontoini.Name = "lblmontoini";
            this.lblmontoini.Size = new System.Drawing.Size(197, 34);
            this.lblmontoini.TabIndex = 5;
            this.lblmontoini.Text = "Monto Inicial";
            this.lblmontoini.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblobser
            // 
            this.lblobser.AutoSize = true;
            this.lblobser.Font = new System.Drawing.Font("Bahnschrift", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblobser.Location = new System.Drawing.Point(12, 291);
            this.lblobser.Name = "lblobser";
            this.lblobser.Size = new System.Drawing.Size(150, 25);
            this.lblobser.TabIndex = 6;
            this.lblobser.Text = "Observaciones";
            // 
            // lblFechaactual
            // 
            this.lblFechaactual.AutoSize = true;
            this.lblFechaactual.Location = new System.Drawing.Point(418, 71);
            this.lblFechaactual.Name = "lblFechaactual";
            this.lblFechaactual.Size = new System.Drawing.Size(10, 13);
            this.lblFechaactual.TabIndex = 7;
            this.lblFechaactual.Text = ".";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(420, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = ".";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(420, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = ".";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(215, 242);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(199, 34);
            this.textBox1.TabIndex = 10;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 319);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(370, 75);
            this.textBox2.TabIndex = 11;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Image = global::CapaPresentacion.Properties.Resources.cajaapertura;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 57);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(196, 161);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // btnGuardarAprt
            // 
            this.btnGuardarAprt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGuardarAprt.Font = new System.Drawing.Font("Bahnschrift", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarAprt.IconChar = FontAwesome.Sharp.IconChar.SignInAlt;
            this.btnGuardarAprt.IconColor = System.Drawing.Color.Black;
            this.btnGuardarAprt.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnGuardarAprt.IconSize = 30;
            this.btnGuardarAprt.Location = new System.Drawing.Point(525, 352);
            this.btnGuardarAprt.Name = "btnGuardarAprt";
            this.btnGuardarAprt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnGuardarAprt.Size = new System.Drawing.Size(131, 42);
            this.btnGuardarAprt.TabIndex = 14;
            this.btnGuardarAprt.Text = "Guardar";
            this.btnGuardarAprt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGuardarAprt.UseVisualStyleBackColor = true;
            // 
            // btnCancelarapt
            // 
            this.btnCancelarapt.Font = new System.Drawing.Font("Bahnschrift", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelarapt.IconChar = FontAwesome.Sharp.IconChar.X;
            this.btnCancelarapt.IconColor = System.Drawing.Color.Black;
            this.btnCancelarapt.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCancelarapt.IconSize = 25;
            this.btnCancelarapt.Location = new System.Drawing.Point(388, 352);
            this.btnCancelarapt.Name = "btnCancelarapt";
            this.btnCancelarapt.Size = new System.Drawing.Size(131, 42);
            this.btnCancelarapt.TabIndex = 13;
            this.btnCancelarapt.Text = "Cancelar";
            this.btnCancelarapt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelarapt.UseVisualStyleBackColor = true;
            // 
            // iconButton1
            // 
            this.iconButton1.Font = new System.Drawing.Font("Bahnschrift", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.ClipboardList;
            this.iconButton1.IconColor = System.Drawing.Color.Black;
            this.iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton1.IconSize = 30;
            this.iconButton1.Location = new System.Drawing.Point(420, 242);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(66, 34);
            this.iconButton1.TabIndex = 12;
            this.iconButton1.UseVisualStyleBackColor = true;
            // 
            // FrmApertura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(668, 406);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnGuardarAprt);
            this.Controls.Add(this.btnCancelarapt);
            this.Controls.Add(this.iconButton1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblFechaactual);
            this.Controls.Add(this.lblobser);
            this.Controls.Add(this.lblmontoini);
            this.Controls.Add(this.lblcaja);
            this.Controls.Add(this.lblcajero);
            this.Controls.Add(this.lblfecha);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmApertura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblfecha;
        private System.Windows.Forms.Label lblcajero;
        private System.Windows.Forms.Label lblcaja;
        private System.Windows.Forms.Label lblmontoini;
        private System.Windows.Forms.Label lblobser;
        private System.Windows.Forms.Label lblFechaactual;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private FontAwesome.Sharp.IconButton iconButton1;
        private FontAwesome.Sharp.IconButton btnCancelarapt;
        private FontAwesome.Sharp.IconButton btnGuardarAprt;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}