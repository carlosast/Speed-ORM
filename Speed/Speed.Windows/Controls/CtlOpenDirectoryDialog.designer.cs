namespace Speed.Windows.Controls
{
    partial class CtlOpenDirectoryDialog
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
            this.components = new System.ComponentModel.Container();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // txtDirectory
            // 
            this.txtDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDirectory.Location = new System.Drawing.Point(0, 0);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(110, 20);
            this.txtDirectory.TabIndex = 2;
            this.txtDirectory.TextChanged += new System.EventHandler(this.txtDirectory_TextChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.BackgroundImage = global::Speed.Windows.Properties.Resources.folder_open;
            this.btnOpen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOpen.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Location = new System.Drawing.Point(110, 0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(21, 20);
            this.btnOpen.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnOpen, "Selecionar diretório");
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnView
            // 
            this.btnView.BackgroundImage = global::Speed.Windows.Properties.Resources.find_small;
            this.btnView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnView.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnView.Location = new System.Drawing.Point(131, 0);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(21, 20);
            this.btnView.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnView, "Abrir o diretório no Windows Explorer");
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // CtlOpenDirectoryDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnView);
            this.Name = "CtlOpenDirectoryDialog";
            this.Size = new System.Drawing.Size(152, 20);
            this.Load += new System.EventHandler(this.CtlOpenDirectoryDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDirectory;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
