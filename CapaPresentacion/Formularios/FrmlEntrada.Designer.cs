namespace CapaPresentacion
{
    partial class FrmlEntrada
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmlEntrada));
            this.label11 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btningr = new FontAwesome.Sharp.IconButton();
            this.btnegre = new FontAwesome.Sharp.IconButton();
            this.iconButton9 = new FontAwesome.Sharp.IconButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.DodgerBlue;
            this.label11.Font = new System.Drawing.Font("Bahnschrift", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.Window;
            this.label11.Location = new System.Drawing.Point(3, 0);
            this.label11.Name = "label11";
            this.label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label11.Size = new System.Drawing.Size(347, 50);
            this.label11.TabIndex = 4;
            this.label11.Text = "Control De Efectivo";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DodgerBlue;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label11);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1026, 82);
            this.panel1.TabIndex = 42;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(5, 203);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1023, 349);
            this.dataGridView1.TabIndex = 43;
            // 
            // btningr
            // 
            this.btningr.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btningr.FlatAppearance.BorderSize = 12;
            this.btningr.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.btningr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btningr.Font = new System.Drawing.Font("Microsoft YaHei UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btningr.IconChar = FontAwesome.Sharp.IconChar.ArrowCircleDown;
            this.btningr.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btningr.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btningr.Location = new System.Drawing.Point(8, 93);
            this.btningr.Name = "btningr";
            this.btningr.Size = new System.Drawing.Size(176, 87);
            this.btningr.TabIndex = 46;
            this.btningr.Text = "INGRESO";
            this.btningr.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btningr.UseVisualStyleBackColor = true;
            this.btningr.Click += new System.EventHandler(this.btningr_Click);
            // 
            // btnegre
            // 
            this.btnegre.FlatAppearance.BorderSize = 12;
            this.btnegre.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnegre.Font = new System.Drawing.Font("Microsoft YaHei UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnegre.IconChar = FontAwesome.Sharp.IconChar.ArrowCircleUp;
            this.btnegre.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnegre.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnegre.Location = new System.Drawing.Point(209, 93);
            this.btnegre.Name = "btnegre";
            this.btnegre.Size = new System.Drawing.Size(176, 87);
            this.btnegre.TabIndex = 45;
            this.btnegre.Text = "SALIDA";
            this.btnegre.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnegre.UseVisualStyleBackColor = true;
            this.btnegre.Click += new System.EventHandler(this.btnegre_Click);
            // 
            // iconButton9
            // 
            this.iconButton9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconButton9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.iconButton9.BackColor = System.Drawing.SystemColors.Control;
            this.iconButton9.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton9.IconChar = FontAwesome.Sharp.IconChar.MoneyBillTransfer;
            this.iconButton9.IconColor = System.Drawing.Color.Black;
            this.iconButton9.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.iconButton9.IconSize = 25;
            this.iconButton9.Location = new System.Drawing.Point(915, 93);
            this.iconButton9.Margin = new System.Windows.Forms.Padding(4);
            this.iconButton9.Name = "iconButton9";
            this.iconButton9.Size = new System.Drawing.Size(112, 83);
            this.iconButton9.TabIndex = 42;
            this.iconButton9.Text = "MESA DE CAMBIO";
            this.iconButton9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.iconButton9.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.iconButton9.UseVisualStyleBackColor = false;
            this.iconButton9.Click += new System.EventHandler(this.MesaCambio_Click);
            // 
            // FrmlEntrada
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1036, 560);
            this.ControlBox = false;
            this.Controls.Add(this.btningr);
            this.Controls.Add(this.btnegre);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.iconButton9);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmlEntrada";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.PaddingChanged += new System.EventHandler(this.Entrada_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel1;
        private FontAwesome.Sharp.IconButton iconButton9;
        private System.Windows.Forms.DataGridView dataGridView1;
        private FontAwesome.Sharp.IconButton btnegre;
        private FontAwesome.Sharp.IconButton btningr;
    }
}