namespace Speed.UI.UserControls
{
    partial class CtlClassParameters
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
            this.txtRemove = new System.Windows.Forms.TextBox();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cboNameCase = new System.Windows.Forms.ComboBox();
            this.chkStartWithSchema = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtRemove
            // 
            this.txtRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemove.Location = new System.Drawing.Point(226, 20);
            this.txtRemove.Multiline = true;
            this.txtRemove.Name = "txtRemove";
            this.txtRemove.Size = new System.Drawing.Size(184, 60);
            this.txtRemove.TabIndex = 17;
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(103, 32);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(100, 20);
            this.txtPrefix.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(289, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "Remove";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(14, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = "Prefix";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(14, 2);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 20);
            this.label7.TabIndex = 14;
            this.label7.Text = "Name case";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboNameCase
            // 
            this.cboNameCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNameCase.FormattingEnabled = true;
            this.cboNameCase.Location = new System.Drawing.Point(103, 5);
            this.cboNameCase.Name = "cboNameCase";
            this.cboNameCase.Size = new System.Drawing.Size(99, 21);
            this.cboNameCase.TabIndex = 16;
            this.cboNameCase.SelectedIndexChanged += new System.EventHandler(this.cboNameCase_SelectedIndexChanged);
            // 
            // chkStartWithSchema
            // 
            this.chkStartWithSchema.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStartWithSchema.Location = new System.Drawing.Point(14, 58);
            this.chkStartWithSchema.Name = "chkStartWithSchema";
            this.chkStartWithSchema.Size = new System.Drawing.Size(188, 17);
            this.chkStartWithSchema.TabIndex = 15;
            this.chkStartWithSchema.Text = "Start names whith schema name";
            this.chkStartWithSchema.UseVisualStyleBackColor = true;
            // 
            // CtlClassParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtRemove);
            this.Controls.Add(this.txtPrefix);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboNameCase);
            this.Controls.Add(this.chkStartWithSchema);
            this.Name = "CtlClassParameters";
            this.Size = new System.Drawing.Size(418, 85);
            this.Load += new System.EventHandler(this.CtlClassParameters_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRemove;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboNameCase;
        private System.Windows.Forms.CheckBox chkStartWithSchema;

    }
}
