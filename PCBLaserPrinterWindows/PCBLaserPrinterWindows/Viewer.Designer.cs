namespace PCBLaserPrinterWindows
{
    partial class Viewer
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
            this.ViewerBox = new System.Windows.Forms.PictureBox();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnProcesar = new System.Windows.Forms.Button();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.barProcess = new System.Windows.Forms.ToolStripProgressBar();
            this.lblProcess = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblError = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ViewerBox)).BeginInit();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ViewerBox
            // 
            this.ViewerBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ViewerBox.BackColor = System.Drawing.Color.Black;
            this.ViewerBox.Location = new System.Drawing.Point(12, 40);
            this.ViewerBox.Name = "ViewerBox";
            this.ViewerBox.Size = new System.Drawing.Size(405, 339);
            this.ViewerBox.TabIndex = 0;
            this.ViewerBox.TabStop = false;
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(12, 13);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(324, 20);
            this.txtFile.TabIndex = 1;
            this.txtFile.Text = "D:\\git\\PCB-Laser-Printer\\gerber_example01.grb";
            // 
            // btnProcesar
            // 
            this.btnProcesar.Location = new System.Drawing.Point(342, 10);
            this.btnProcesar.Name = "btnProcesar";
            this.btnProcesar.Size = new System.Drawing.Size(75, 23);
            this.btnProcesar.TabIndex = 2;
            this.btnProcesar.Text = "Procesar";
            this.btnProcesar.UseVisualStyleBackColor = true;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProcess,
            this.barProcess,
            this.lblError});
            this.statusBar.Location = new System.Drawing.Point(0, 389);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(429, 26);
            this.statusBar.TabIndex = 3;
            this.statusBar.Text = "statusStrip1";
            this.statusBar.Visible = false;
            // 
            // barProcess
            // 
            this.barProcess.Name = "barProcess";
            this.barProcess.Size = new System.Drawing.Size(200, 20);
            // 
            // lblProcess
            // 
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(59, 21);
            this.lblProcess.Text = "Procesing";
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(39, 21);
            this.lblError.Text = "Error";
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(429, 415);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.btnProcesar);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.ViewerBox);
            this.Name = "Viewer";
            this.Text = "Gerber Viewer";
            this.Load += new System.EventHandler(this.Viewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ViewerBox)).EndInit();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ViewerBox;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnProcesar;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel lblProcess;
        private System.Windows.Forms.ToolStripProgressBar barProcess;
        private System.Windows.Forms.ToolStripStatusLabel lblError;
    }
}

