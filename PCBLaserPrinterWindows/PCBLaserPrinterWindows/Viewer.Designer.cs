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
            this.lblProcess = new System.Windows.Forms.ToolStripStatusLabel();
            this.barProcess = new System.Windows.Forms.ToolStripProgressBar();
            this.lblError = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.PreviewBox = new System.Windows.Forms.PictureBox();
            this.btnPrinter = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ViewerBox)).BeginInit();
            this.statusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).BeginInit();
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
            this.ViewerBox.Size = new System.Drawing.Size(405, 498);
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
            this.statusBar.BackColor = System.Drawing.Color.Gainsboro;
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProcess,
            this.barProcess,
            this.lblError,
            this.lblStatus});
            this.statusBar.Location = new System.Drawing.Point(0, 524);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(582, 26);
            this.statusBar.TabIndex = 3;
            this.statusBar.Text = "statusStrip1";
            this.statusBar.Visible = false;
            // 
            // lblProcess
            // 
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(59, 21);
            this.lblProcess.Text = "Procesing";
            // 
            // barProcess
            // 
            this.barProcess.Name = "barProcess";
            this.barProcess.Size = new System.Drawing.Size(200, 20);
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(39, 21);
            this.lblError.Text = "Error";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(118, 15);
            this.lblStatus.Text = "toolStripStatusLabel1";
            // 
            // PreviewBox
            // 
            this.PreviewBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PreviewBox.BackColor = System.Drawing.Color.Transparent;
            this.PreviewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PreviewBox.Location = new System.Drawing.Point(208, 316);
            this.PreviewBox.Name = "PreviewBox";
            this.PreviewBox.Size = new System.Drawing.Size(209, 222);
            this.PreviewBox.TabIndex = 4;
            this.PreviewBox.TabStop = false;
            // 
            // btnPrinter
            // 
            this.btnPrinter.BackColor = System.Drawing.Color.Transparent;
            this.btnPrinter.BackgroundImage = global::PCBLaserPrinterWindows.Properties.Resources.pcb;
            this.btnPrinter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPrinter.Location = new System.Drawing.Point(511, 10);
            this.btnPrinter.Name = "btnPrinter";
            this.btnPrinter.Size = new System.Drawing.Size(59, 46);
            this.btnPrinter.TabIndex = 5;
            this.btnPrinter.UseVisualStyleBackColor = false;
            this.btnPrinter.Click += new System.EventHandler(this.btnPrinter_Click);
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(582, 550);
            this.Controls.Add(this.btnPrinter);
            this.Controls.Add(this.PreviewBox);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.btnProcesar);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.ViewerBox);
            this.Name = "Viewer";
            this.Text = "Gerber Viewer";
            this.Load += new System.EventHandler(this.Viewer_Load);
            this.Resize += new System.EventHandler(this.Viewer_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.ViewerBox)).EndInit();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).EndInit();
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
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.PictureBox PreviewBox;
        private System.Windows.Forms.Button btnPrinter;
    }
}

