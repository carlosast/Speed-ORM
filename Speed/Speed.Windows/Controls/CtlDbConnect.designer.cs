namespace Speed.Windows.Controls
{
    partial class CtlDbConnect
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
            this.components = new System.ComponentModel.Container();
            this.lblHost = new System.Windows.Forms.Label();
            this.grbBuildCs = new System.Windows.Forms.GroupBox();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.chkEmbedded = new System.Windows.Forms.CheckBox();
            this.lblEmbedded = new System.Windows.Forms.Label();
            this.chkIntegSec = new System.Windows.Forms.CheckBox();
            this.lblPortDefault = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserId = new System.Windows.Forms.Label();
            this.lblIntegSec = new System.Windows.Forms.Label();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.lblConenctionString = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.txtDatabase = new Speed.Windows.Controls.CtlHost();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboProviderType = new System.Windows.Forms.ComboBox();
            this.rbtBuildCs = new System.Windows.Forms.RadioButton();
            this.rbtEnterCs = new System.Windows.Forms.RadioButton();
            this.grbEnterCs = new System.Windows.Forms.GroupBox();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lblHints = new System.Windows.Forms.Label();
            this.picHelp = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grbBuildCs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.grbEnterCs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHelp)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHost
            // 
            this.lblHost.Location = new System.Drawing.Point(6, 124);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(88, 20);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "Host";
            // 
            // grbBuildCs
            // 
            this.grbBuildCs.Controls.Add(this.numPort);
            this.grbBuildCs.Controls.Add(this.chkEmbedded);
            this.grbBuildCs.Controls.Add(this.lblEmbedded);
            this.grbBuildCs.Controls.Add(this.chkIntegSec);
            this.grbBuildCs.Controls.Add(this.lblPortDefault);
            this.grbBuildCs.Controls.Add(this.lblPort);
            this.grbBuildCs.Controls.Add(this.lblPassword);
            this.grbBuildCs.Controls.Add(this.lblUserId);
            this.grbBuildCs.Controls.Add(this.lblIntegSec);
            this.grbBuildCs.Controls.Add(this.lblDatabase);
            this.grbBuildCs.Controls.Add(this.lblConenctionString);
            this.grbBuildCs.Controls.Add(this.label8);
            this.grbBuildCs.Controls.Add(this.lblHost);
            this.grbBuildCs.Controls.Add(this.txtPassword);
            this.grbBuildCs.Controls.Add(this.txtUserId);
            this.grbBuildCs.Controls.Add(this.txtDatabase);
            this.grbBuildCs.Controls.Add(this.txtHost);
            this.grbBuildCs.Location = new System.Drawing.Point(0, 77);
            this.grbBuildCs.Name = "grbBuildCs";
            this.grbBuildCs.Size = new System.Drawing.Size(550, 287);
            this.grbBuildCs.TabIndex = 3;
            this.grbBuildCs.TabStop = false;
            this.grbBuildCs.Text = "Build connection string";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(107, 262);
            this.numPort.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(115, 20);
            this.numPort.TabIndex = 8;
            // 
            // chkEmbedded
            // 
            this.chkEmbedded.Location = new System.Drawing.Point(107, 93);
            this.chkEmbedded.Name = "chkEmbedded";
            this.chkEmbedded.Size = new System.Drawing.Size(152, 20);
            this.chkEmbedded.TabIndex = 7;
            this.chkEmbedded.UseVisualStyleBackColor = true;
            this.chkEmbedded.CheckedChanged += new System.EventHandler(this.chkEmbedded_CheckedChanged);
            // 
            // lblEmbedded
            // 
            this.lblEmbedded.Location = new System.Drawing.Point(6, 93);
            this.lblEmbedded.Name = "lblEmbedded";
            this.lblEmbedded.Size = new System.Drawing.Size(99, 20);
            this.lblEmbedded.TabIndex = 6;
            this.lblEmbedded.Text = "Embedded";
            // 
            // chkIntegSec
            // 
            this.chkIntegSec.Location = new System.Drawing.Point(107, 177);
            this.chkIntegSec.Name = "chkIntegSec";
            this.chkIntegSec.Size = new System.Drawing.Size(152, 20);
            this.chkIntegSec.TabIndex = 2;
            this.chkIntegSec.UseVisualStyleBackColor = true;
            this.chkIntegSec.CheckedChanged += new System.EventHandler(this.chkIntegSec_CheckedChanged);
            // 
            // lblPortDefault
            // 
            this.lblPortDefault.Location = new System.Drawing.Point(228, 264);
            this.lblPortDefault.Name = "lblPortDefault";
            this.lblPortDefault.Size = new System.Drawing.Size(88, 20);
            this.lblPortDefault.TabIndex = 0;
            this.lblPortDefault.Text = "( 0 = default)";
            // 
            // lblPort
            // 
            this.lblPort.Location = new System.Drawing.Point(6, 262);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(88, 20);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Port";
            // 
            // lblPassword
            // 
            this.lblPassword.Location = new System.Drawing.Point(6, 232);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(88, 20);
            this.lblPassword.TabIndex = 0;
            this.lblPassword.Text = "Password";
            // 
            // lblUserId
            // 
            this.lblUserId.Location = new System.Drawing.Point(6, 206);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new System.Drawing.Size(88, 20);
            this.lblUserId.TabIndex = 0;
            this.lblUserId.Text = "User ID";
            // 
            // lblIntegSec
            // 
            this.lblIntegSec.Location = new System.Drawing.Point(6, 177);
            this.lblIntegSec.Name = "lblIntegSec";
            this.lblIntegSec.Size = new System.Drawing.Size(99, 20);
            this.lblIntegSec.TabIndex = 0;
            this.lblIntegSec.Text = "Integrated Security";
            // 
            // lblDatabase
            // 
            this.lblDatabase.Location = new System.Drawing.Point(6, 150);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(88, 20);
            this.lblDatabase.TabIndex = 0;
            this.lblDatabase.Text = "Database";
            // 
            // lblConenctionString
            // 
            this.lblConenctionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConenctionString.Location = new System.Drawing.Point(107, 18);
            this.lblConenctionString.Name = "lblConenctionString";
            this.lblConenctionString.Size = new System.Drawing.Size(437, 50);
            this.lblConenctionString.TabIndex = 0;
            this.toolTip1.SetToolTip(this.lblConenctionString, "Click to copy to clipboard");
            this.lblConenctionString.Click += new System.EventHandler(this.lblConenctionString_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 20);
            this.label8.TabIndex = 0;
            this.label8.Text = "Connection string";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(107, 232);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(437, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtUserId
            // 
            this.txtUserId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserId.Location = new System.Drawing.Point(107, 206);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(437, 20);
            this.txtUserId.TabIndex = 3;
            this.txtUserId.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtDatabase
            // 
            this.txtDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabase.Location = new System.Drawing.Point(107, 150);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(437, 20);
            this.txtDatabase.TabIndex = 1;
            this.txtDatabase.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtHost
            // 
            this.txtHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHost.Location = new System.Drawing.Point(107, 124);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(437, 20);
            this.txtHost.TabIndex = 0;
            this.txtHost.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 20);
            this.label6.TabIndex = 9;
            this.label6.Text = "Database Type";
            // 
            // cboProviderType
            // 
            this.cboProviderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProviderType.FormattingEnabled = true;
            this.cboProviderType.Location = new System.Drawing.Point(107, 12);
            this.cboProviderType.Name = "cboProviderType";
            this.cboProviderType.Size = new System.Drawing.Size(420, 21);
            this.cboProviderType.TabIndex = 0;
            this.cboProviderType.SelectedIndexChanged += new System.EventHandler(this.cboProviderType_SelectedIndexChanged);
            // 
            // rbtBuildCs
            // 
            this.rbtBuildCs.AutoSize = true;
            this.rbtBuildCs.Checked = true;
            this.rbtBuildCs.Location = new System.Drawing.Point(9, 47);
            this.rbtBuildCs.Name = "rbtBuildCs";
            this.rbtBuildCs.Size = new System.Drawing.Size(132, 17);
            this.rbtBuildCs.TabIndex = 1;
            this.rbtBuildCs.TabStop = true;
            this.rbtBuildCs.Text = "Build connection string";
            this.rbtBuildCs.UseVisualStyleBackColor = true;
            this.rbtBuildCs.Click += new System.EventHandler(this.rbtBuildCs_Click);
            // 
            // rbtEnterCs
            // 
            this.rbtEnterCs.AutoSize = true;
            this.rbtEnterCs.Location = new System.Drawing.Point(229, 47);
            this.rbtEnterCs.Name = "rbtEnterCs";
            this.rbtEnterCs.Size = new System.Drawing.Size(134, 17);
            this.rbtEnterCs.TabIndex = 2;
            this.rbtEnterCs.TabStop = true;
            this.rbtEnterCs.Text = "Enter connection string";
            this.rbtEnterCs.UseVisualStyleBackColor = true;
            this.rbtEnterCs.Click += new System.EventHandler(this.rbtBuildCs_Click);
            // 
            // grbEnterCs
            // 
            this.grbEnterCs.Controls.Add(this.txtConnectionString);
            this.grbEnterCs.Controls.Add(this.label11);
            this.grbEnterCs.Location = new System.Drawing.Point(165, 111);
            this.grbEnterCs.Name = "grbEnterCs";
            this.grbEnterCs.Size = new System.Drawing.Size(532, 93);
            this.grbEnterCs.TabIndex = 4;
            this.grbEnterCs.TabStop = false;
            this.grbEnterCs.Text = "Enter connection string";
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnectionString.Location = new System.Drawing.Point(107, 25);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(419, 20);
            this.txtConnectionString.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(6, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(98, 20);
            this.label11.TabIndex = 0;
            this.label11.Text = "Connection String";
            // 
            // lblHints
            // 
            this.lblHints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHints.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHints.Location = new System.Drawing.Point(556, 12);
            this.lblHints.Name = "lblHints";
            this.lblHints.Size = new System.Drawing.Size(415, 347);
            this.lblHints.TabIndex = 10;
            this.lblHints.Text = "label8";
            // 
            // picHelp
            // 
            this.picHelp.Image = global::Speed.Windows.Properties.Resources.Help;
            this.picHelp.Location = new System.Drawing.Point(533, 15);
            this.picHelp.Name = "picHelp";
            this.picHelp.Size = new System.Drawing.Size(17, 18);
            this.picHelp.TabIndex = 11;
            this.picHelp.TabStop = false;
            this.picHelp.Click += new System.EventHandler(this.picHelp_Click);
            // 
            // CtlDbConnect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picHelp);
            this.Controls.Add(this.lblHints);
            this.Controls.Add(this.grbEnterCs);
            this.Controls.Add(this.rbtEnterCs);
            this.Controls.Add(this.rbtBuildCs);
            this.Controls.Add(this.cboProviderType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.grbBuildCs);
            this.Name = "CtlDbConnect";
            this.Size = new System.Drawing.Size(974, 368);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grbBuildCs.ResumeLayout(false);
            this.grbBuildCs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.grbEnterCs.ResumeLayout(false);
            this.grbEnterCs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHelp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.GroupBox grbBuildCs;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUserId;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserId;
        private Speed.Windows.Controls.CtlHost txtDatabase;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboProviderType;
        private System.Windows.Forms.RadioButton rbtBuildCs;
        private System.Windows.Forms.RadioButton rbtEnterCs;
        private System.Windows.Forms.GroupBox grbEnterCs;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.CheckBox chkIntegSec;
        private System.Windows.Forms.Label lblIntegSec;
        private System.Windows.Forms.Label lblHints;
        private System.Windows.Forms.PictureBox picHelp;
        private System.Windows.Forms.Label lblConenctionString;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkEmbedded;
        private System.Windows.Forms.Label lblEmbedded;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label lblPortDefault;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

