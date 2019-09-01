namespace Speed.Windows.Controls
{
    partial class CtlHost
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textbox = new System.Windows.Forms.TextBox();
            this.ctlOpenFile = new Speed.Windows.Controls.CtlOpenFileDialog();
            this.SuspendLayout();
            // 
            // textbox
            // 
            this.textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textbox.Location = new System.Drawing.Point(0, 0);
            this.textbox.Name = "textbox";
            this.textbox.Size = new System.Drawing.Size(150, 20);
            this.textbox.TabIndex = 0;
            // 
            // ctlOpenFile
            // 
            this.ctlOpenFile.AddExtension = false;
            this.ctlOpenFile.CheckFileExists = false;
            this.ctlOpenFile.CheckPathExists = true;
            this.ctlOpenFile.DefaultExt = null;
            this.ctlOpenFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlOpenFile.FileName = "";
            this.ctlOpenFile.Filter = null;
            this.ctlOpenFile.Location = new System.Drawing.Point(0, 0);
            this.ctlOpenFile.Name = "ctlOpenFile";
            this.ctlOpenFile.Size = new System.Drawing.Size(150, 20);
            this.ctlOpenFile.TabIndex = 1;
            this.ctlOpenFile.Title = null;
            // 
            // CtlHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctlOpenFile);
            this.Controls.Add(this.textbox);
            this.Name = "CtlHost";
            this.Size = new System.Drawing.Size(150, 20);
            this.Load += new System.EventHandler(this.CtlHost_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textbox;
        private Windows.Controls.CtlOpenFileDialog ctlOpenFile;
    }
}
