namespace Speed.UI.Forms
{
    partial class FormApply
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkEnum = new System.Windows.Forms.CheckBox();
            this.chkBusinessClass = new System.Windows.Forms.CheckBox();
            this.chkDataClass = new System.Windows.Forms.CheckBox();
            this.chkUnnamed = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkOnlySelectedObjects = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkEnum);
            this.groupBox1.Controls.Add(this.chkBusinessClass);
            this.groupBox1.Controls.Add(this.chkDataClass);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(347, 62);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Apply names to";
            // 
            // chkEnum
            // 
            this.chkEnum.AutoSize = true;
            this.chkEnum.Checked = true;
            this.chkEnum.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnum.Location = new System.Drawing.Point(239, 29);
            this.chkEnum.Name = "chkEnum";
            this.chkEnum.Size = new System.Drawing.Size(58, 17);
            this.chkEnum.TabIndex = 0;
            this.chkEnum.Text = "Enums";
            this.chkEnum.UseVisualStyleBackColor = true;
            this.chkEnum.CheckedChanged += new System.EventHandler(this.chkApply_CheckedChanged);
            // 
            // chkBusinessClass
            // 
            this.chkBusinessClass.AutoSize = true;
            this.chkBusinessClass.Checked = true;
            this.chkBusinessClass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBusinessClass.Location = new System.Drawing.Point(116, 29);
            this.chkBusinessClass.Name = "chkBusinessClass";
            this.chkBusinessClass.Size = new System.Drawing.Size(96, 17);
            this.chkBusinessClass.TabIndex = 0;
            this.chkBusinessClass.Text = "Business Class";
            this.chkBusinessClass.UseVisualStyleBackColor = true;
            this.chkBusinessClass.CheckedChanged += new System.EventHandler(this.chkApply_CheckedChanged);
            // 
            // chkDataClass
            // 
            this.chkDataClass.AutoSize = true;
            this.chkDataClass.Checked = true;
            this.chkDataClass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDataClass.Location = new System.Drawing.Point(12, 29);
            this.chkDataClass.Name = "chkDataClass";
            this.chkDataClass.Size = new System.Drawing.Size(77, 17);
            this.chkDataClass.TabIndex = 0;
            this.chkDataClass.Text = "Data Class";
            this.chkDataClass.UseVisualStyleBackColor = true;
            this.chkDataClass.CheckedChanged += new System.EventHandler(this.chkApply_CheckedChanged);
            // 
            // chkUnnamed
            // 
            this.chkUnnamed.AutoSize = true;
            this.chkUnnamed.Checked = true;
            this.chkUnnamed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnnamed.Location = new System.Drawing.Point(12, 90);
            this.chkUnnamed.Name = "chkUnnamed";
            this.chkUnnamed.Size = new System.Drawing.Size(170, 17);
            this.chkUnnamed.TabIndex = 1;
            this.chkUnnamed.Text = "Apply to unnamed objects only";
            this.chkUnnamed.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(376, 12);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(376, 51);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkOnlySelectedObjects
            // 
            this.chkOnlySelectedObjects.AutoSize = true;
            this.chkOnlySelectedObjects.Checked = true;
            this.chkOnlySelectedObjects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnlySelectedObjects.Location = new System.Drawing.Point(262, 90);
            this.chkOnlySelectedObjects.Name = "chkOnlySelectedObjects";
            this.chkOnlySelectedObjects.Size = new System.Drawing.Size(166, 17);
            this.chkOnlySelectedObjects.TabIndex = 1;
            this.chkOnlySelectedObjects.Text = "Apply to selected objects only";
            this.chkOnlySelectedObjects.UseVisualStyleBackColor = true;
            // 
            // FormApply
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(463, 118);
            this.Controls.Add(this.chkOnlySelectedObjects);
            this.Controls.Add(this.chkUnnamed);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormApply";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Apply Names";
            this.Load += new System.EventHandler(this.FormApply_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkDataClass;
        private System.Windows.Forms.CheckBox chkBusinessClass;
        private System.Windows.Forms.CheckBox chkUnnamed;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkEnum;
        private System.Windows.Forms.CheckBox chkOnlySelectedObjects;
    }
}