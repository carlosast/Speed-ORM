﻿namespace Speed.UI.UserControls
{
    partial class CtlGenerator
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabConnection = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblNotConnected = new System.Windows.Forms.Label();
            this.dbConnect = new Speed.Windows.Controls.CtlDbConnect();
            this.tabBrowser = new System.Windows.Forms.TabPage();
            this.btnConnect = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panButtons = new System.Windows.Forms.Panel();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.parameters = new Speed.UI.UserControls.CtlGenParameters();
            this.browser = new Speed.UI.UserControls.CtlBrowser();
            this.tabControl1.SuspendLayout();
            this.tabConnection.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabBrowser.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConnection);
            this.tabControl1.Controls.Add(this.tabBrowser);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(974, 657);
            this.tabControl1.TabIndex = 0;
            // 
            // tabConnection
            // 
            this.tabConnection.Controls.Add(this.groupBox2);
            this.tabConnection.Controls.Add(this.groupBox1);
            this.tabConnection.Location = new System.Drawing.Point(4, 27);
            this.tabConnection.Name = "tabConnection";
            this.tabConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tabConnection.Size = new System.Drawing.Size(966, 626);
            this.tabConnection.TabIndex = 0;
            this.tabConnection.Text = "Configuration";
            this.tabConnection.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.parameters);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(3, 301);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(960, 180);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Global Parameters";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblNotConnected);
            this.groupBox1.Controls.Add(this.dbConnect);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(960, 298);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database";
            // 
            // lblNotConnected
            // 
            this.lblNotConnected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotConnected.ForeColor = System.Drawing.Color.Red;
            this.lblNotConnected.Location = new System.Drawing.Point(584, 360);
            this.lblNotConnected.Name = "lblNotConnected";
            this.lblNotConnected.Size = new System.Drawing.Size(106, 23);
            this.lblNotConnected.TabIndex = 2;
            this.lblNotConnected.Text = "Not connected";
            this.lblNotConnected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dbConnect
            // 
            this.dbConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.dbConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dbConnect.Location = new System.Drawing.Point(3, 16);
            this.dbConnect.Name = "dbConnect";
            this.dbConnect.Size = new System.Drawing.Size(954, 364);
            this.dbConnect.TabIndex = 0;
            this.dbConnect.VisibleChanged += new System.EventHandler(this.dbConnect_VisibleChanged);
            // 
            // tabBrowser
            // 
            this.tabBrowser.Controls.Add(this.browser);
            this.tabBrowser.Location = new System.Drawing.Point(4, 27);
            this.tabBrowser.Name = "tabBrowser";
            this.tabBrowser.Padding = new System.Windows.Forms.Padding(3);
            this.tabBrowser.Size = new System.Drawing.Size(966, 621);
            this.tabBrowser.TabIndex = 1;
            this.tabBrowser.Text = "Object Browser";
            this.tabBrowser.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConnect.Location = new System.Drawing.Point(3, 0);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(104, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect/Refresh";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panButtons);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 657);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(974, 30);
            this.panel1.TabIndex = 1;
            // 
            // panButtons
            // 
            this.panButtons.Controls.Add(this.btnConnect);
            this.panButtons.Controls.Add(this.btnGenerate);
            this.panButtons.Location = new System.Drawing.Point(337, 6);
            this.panButtons.Name = "panButtons";
            this.panButtons.Size = new System.Drawing.Size(240, 25);
            this.panButtons.TabIndex = 3;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerate.Location = new System.Drawing.Point(136, 0);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(100, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // parameters
            // 
            this.parameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.parameters.Location = new System.Drawing.Point(28, 22);
            this.parameters.Name = "parameters";
            this.parameters.Size = new System.Drawing.Size(910, 134);
            this.parameters.TabIndex = 1;
            // 
            // browser
            // 
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browser.Location = new System.Drawing.Point(3, 3);
            this.browser.Margin = new System.Windows.Forms.Padding(4);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(960, 615);
            this.browser.TabIndex = 0;
            // 
            // CtlGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "CtlGenerator";
            this.Size = new System.Drawing.Size(974, 687);
            this.Load += new System.EventHandler(this.CtlGenerator_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabConnection.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabBrowser.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabConnection;
        private System.Windows.Forms.TabPage tabBrowser;
        private System.Windows.Forms.Panel panel1;
        private Windows.Controls.CtlDbConnect dbConnect;
        private CtlBrowser browser;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private CtlGenParameters parameters;
        private System.Windows.Forms.Label lblNotConnected;
        private System.Windows.Forms.Panel panButtons;
    }
}
