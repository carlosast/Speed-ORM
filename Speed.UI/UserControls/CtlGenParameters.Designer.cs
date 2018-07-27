namespace Speed.UI.UserControls
{
    partial class CtlGenParameters
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtEntityNameSpace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBusinnesNameSpace = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSuggest = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkArrangeDirectoriesBySchema = new System.Windows.Forms.CheckBox();
            this.oddBusinnesDirectoryExt = new Speed.Windows.Controls.CtlOpenDirectoryDialog();
            this.oddEntityDirectoryExt = new Speed.Windows.Controls.CtlOpenDirectoryDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Entity Namespace";
            // 
            // txtEntityNameSpace
            // 
            this.txtEntityNameSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntityNameSpace.Location = new System.Drawing.Point(160, 26);
            this.txtEntityNameSpace.Name = "txtEntityNameSpace";
            this.txtEntityNameSpace.Size = new System.Drawing.Size(402, 20);
            this.txtEntityNameSpace.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Businnes Namespace";
            // 
            // txtBusinnesNameSpace
            // 
            this.txtBusinnesNameSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBusinnesNameSpace.Location = new System.Drawing.Point(160, 52);
            this.txtBusinnesNameSpace.Name = "txtBusinnesNameSpace";
            this.txtBusinnesNameSpace.Size = new System.Drawing.Size(402, 20);
            this.txtBusinnesNameSpace.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Entity Directory";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Business Directory";
            // 
            // btnSuggest
            // 
            this.btnSuggest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSuggest.Location = new System.Drawing.Point(494, 0);
            this.btnSuggest.Name = "btnSuggest";
            this.btnSuggest.Size = new System.Drawing.Size(68, 23);
            this.btnSuggest.TabIndex = 0;
            this.btnSuggest.Text = "Suggestion";
            this.toolTip1.SetToolTip(this.btnSuggest, "Select the solution file (. Sln). Will be suggested namespaces and directories");
            this.btnSuggest.UseVisualStyleBackColor = true;
            this.btnSuggest.Click += new System.EventHandler(this.btnSuggest_Click);
            // 
            // chkArrangeDirectoriesBySchema
            // 
            this.chkArrangeDirectoriesBySchema.Location = new System.Drawing.Point(3, 136);
            this.chkArrangeDirectoriesBySchema.Name = "chkArrangeDirectoriesBySchema";
            this.chkArrangeDirectoriesBySchema.Size = new System.Drawing.Size(188, 17);
            this.chkArrangeDirectoriesBySchema.TabIndex = 16;
            this.chkArrangeDirectoriesBySchema.Text = "Sub-directories by Schema";
            this.chkArrangeDirectoriesBySchema.UseVisualStyleBackColor = true;
            this.chkArrangeDirectoriesBySchema.Visible = false;
            // 
            // oddBusinnesDirectoryExt
            // 
            this.oddBusinnesDirectoryExt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.oddBusinnesDirectoryExt.Directory = "";
            this.oddBusinnesDirectoryExt.Location = new System.Drawing.Point(160, 104);
            this.oddBusinnesDirectoryExt.Name = "oddBusinnesDirectoryExt";
            this.oddBusinnesDirectoryExt.Size = new System.Drawing.Size(402, 20);
            this.oddBusinnesDirectoryExt.TabIndex = 5;
            // 
            // oddEntityDirectoryExt
            // 
            this.oddEntityDirectoryExt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.oddEntityDirectoryExt.Directory = "";
            this.oddEntityDirectoryExt.Location = new System.Drawing.Point(160, 78);
            this.oddEntityDirectoryExt.Name = "oddEntityDirectoryExt";
            this.oddEntityDirectoryExt.Size = new System.Drawing.Size(402, 20);
            this.oddEntityDirectoryExt.TabIndex = 3;
            // 
            // CtlGenParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkArrangeDirectoriesBySchema);
            this.Controls.Add(this.btnSuggest);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.oddBusinnesDirectoryExt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.oddEntityDirectoryExt);
            this.Controls.Add(this.txtBusinnesNameSpace);
            this.Controls.Add(this.txtEntityNameSpace);
            this.Name = "CtlGenParameters";
            this.Size = new System.Drawing.Size(568, 155);
            this.Load += new System.EventHandler(this.DbGenParameters_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEntityNameSpace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBusinnesNameSpace;
        private System.Windows.Forms.Label label4;
        private Windows.Controls.CtlOpenDirectoryDialog oddEntityDirectoryExt;
        private Windows.Controls.CtlOpenDirectoryDialog oddBusinnesDirectoryExt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSuggest;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkArrangeDirectoriesBySchema;
    }
}
