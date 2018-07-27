namespace Speed.Windows
{
    partial class FormGenerate
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
            this.clbTables = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSelecteAll = new System.Windows.Forms.Label();
            this.lblDeSelecteAll = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // clbTables
            // 
            this.clbTables.CheckOnClick = true;
            this.clbTables.FormattingEnabled = true;
            this.clbTables.Location = new System.Drawing.Point(12, 28);
            this.clbTables.Name = "clbTables";
            this.clbTables.Size = new System.Drawing.Size(178, 304);
            this.clbTables.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblDeSelecteAll);
            this.groupBox1.Controls.Add(this.lblSelecteAll);
            this.groupBox1.Controls.Add(this.clbTables);
            this.groupBox1.Location = new System.Drawing.Point(12, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 352);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tables/Views";
            // 
            // lblSelecteAll
            // 
            this.lblSelecteAll.AutoSize = true;
            this.lblSelecteAll.Location = new System.Drawing.Point(162, 12);
            this.lblSelecteAll.Name = "lblSelecteAll";
            this.lblSelecteAll.Size = new System.Drawing.Size(13, 13);
            this.lblSelecteAll.TabIndex = 1;
            this.lblSelecteAll.Text = "+";
            this.lblSelecteAll.Click += new System.EventHandler(this.lblSelecteAll_Click);
            // 
            // lblDeSelecteAll
            // 
            this.lblDeSelecteAll.AutoSize = true;
            this.lblDeSelecteAll.Location = new System.Drawing.Point(181, 12);
            this.lblDeSelecteAll.Name = "lblDeSelecteAll";
            this.lblDeSelecteAll.Size = new System.Drawing.Size(10, 13);
            this.lblDeSelecteAll.TabIndex = 2;
            this.lblDeSelecteAll.Text = "-";
            this.lblDeSelecteAll.Click += new System.EventHandler(this.lblDeSelecteAll_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(228, 17);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Gerar";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // FormGenerate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 374);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormGenerate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormGenerate";
            this.Load += new System.EventHandler(this.FormGenerate_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbTables;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblDeSelecteAll;
        private System.Windows.Forms.Label lblSelecteAll;
        private System.Windows.Forms.Button btnGenerate;
    }
}