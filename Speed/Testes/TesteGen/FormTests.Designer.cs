namespace TesteGen
{
    partial class FormTests
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
            this.gpbTestInsert = new System.Windows.Forms.GroupBox();
            this.chkInsertDapper = new System.Windows.Forms.CheckBox();
            this.chkInsertSql = new System.Windows.Forms.CheckBox();
            this.chkInsertProcedure = new System.Windows.Forms.CheckBox();
            this.chkInsertSpeedXml = new System.Windows.Forms.CheckBox();
            this.chkInsertNHibernate = new System.Windows.Forms.CheckBox();
            this.chkInsertEntity = new System.Windows.Forms.CheckBox();
            this.chkInsertSpeed = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.GroupBox();
            this.rbtTestSelect = new System.Windows.Forms.RadioButton();
            this.rbtTestInsert = new System.Windows.Forms.RadioButton();
            this.gpbTestSelect = new System.Windows.Forms.GroupBox();
            this.chkSelectDataReader = new System.Windows.Forms.CheckBox();
            this.chkSelectSpeed = new System.Windows.Forms.CheckBox();
            this.chkSelectNHibernate = new System.Windows.Forms.CheckBox();
            this.chkSelectEntity = new System.Windows.Forms.CheckBox();
            this.chkSelectDataTable = new System.Windows.Forms.CheckBox();
            this.chkSelectDapper = new System.Windows.Forms.CheckBox();
            this.grbTest = new System.Windows.Forms.GroupBox();
            this.numThreads = new System.Windows.Forms.NumericUpDown();
            this.numRecords = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotlaRecords = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExecSelected = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gpbTestInsert.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gpbTestSelect.SuspendLayout();
            this.grbTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRecords)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpbTestInsert
            // 
            this.gpbTestInsert.Controls.Add(this.chkInsertDapper);
            this.gpbTestInsert.Controls.Add(this.chkInsertSql);
            this.gpbTestInsert.Controls.Add(this.chkInsertProcedure);
            this.gpbTestInsert.Controls.Add(this.chkInsertSpeedXml);
            this.gpbTestInsert.Controls.Add(this.chkInsertNHibernate);
            this.gpbTestInsert.Controls.Add(this.chkInsertEntity);
            this.gpbTestInsert.Controls.Add(this.chkInsertSpeed);
            this.gpbTestInsert.Location = new System.Drawing.Point(6, 59);
            this.gpbTestInsert.Name = "gpbTestInsert";
            this.gpbTestInsert.Size = new System.Drawing.Size(779, 45);
            this.gpbTestInsert.TabIndex = 1;
            this.gpbTestInsert.TabStop = false;
            this.gpbTestInsert.Text = "Test INSERT";
            // 
            // chkInsertDapper
            // 
            this.chkInsertDapper.AutoSize = true;
            this.chkInsertDapper.Location = new System.Drawing.Point(230, 19);
            this.chkInsertDapper.Name = "chkInsertDapper";
            this.chkInsertDapper.Size = new System.Drawing.Size(61, 17);
            this.chkInsertDapper.TabIndex = 2;
            this.chkInsertDapper.Text = "Dapper";
            this.chkInsertDapper.UseVisualStyleBackColor = true;
            this.chkInsertDapper.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkInsertSql
            // 
            this.chkInsertSql.AutoSize = true;
            this.chkInsertSql.Location = new System.Drawing.Point(315, 19);
            this.chkInsertSql.Name = "chkInsertSql";
            this.chkInsertSql.Size = new System.Drawing.Size(85, 17);
            this.chkInsertSql.TabIndex = 3;
            this.chkInsertSql.Text = "Dynamic Sql";
            this.chkInsertSql.UseVisualStyleBackColor = true;
            this.chkInsertSql.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkInsertProcedure
            // 
            this.chkInsertProcedure.AutoSize = true;
            this.chkInsertProcedure.Location = new System.Drawing.Point(659, 17);
            this.chkInsertProcedure.Name = "chkInsertProcedure";
            this.chkInsertProcedure.Size = new System.Drawing.Size(109, 17);
            this.chkInsertProcedure.TabIndex = 6;
            this.chkInsertProcedure.Text = "Stored Procedure";
            this.chkInsertProcedure.UseVisualStyleBackColor = true;
            this.chkInsertProcedure.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkInsertSpeedXml
            // 
            this.chkInsertSpeedXml.AutoSize = true;
            this.chkInsertSpeedXml.ForeColor = System.Drawing.Color.Blue;
            this.chkInsertSpeedXml.Location = new System.Drawing.Point(101, 15);
            this.chkInsertSpeedXml.Name = "chkInsertSpeedXml";
            this.chkInsertSpeedXml.Size = new System.Drawing.Size(105, 30);
            this.chkInsertSpeedXml.TabIndex = 1;
            this.chkInsertSpeedXml.Text = "Speed Xml\r\n(Only Sql Server)";
            this.toolTip1.SetToolTip(this.chkInsertSpeedXml, "Faster");
            this.chkInsertSpeedXml.UseVisualStyleBackColor = true;
            this.chkInsertSpeedXml.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkInsertNHibernate
            // 
            this.chkInsertNHibernate.AutoSize = true;
            this.chkInsertNHibernate.Location = new System.Drawing.Point(555, 19);
            this.chkInsertNHibernate.Name = "chkInsertNHibernate";
            this.chkInsertNHibernate.Size = new System.Drawing.Size(80, 17);
            this.chkInsertNHibernate.TabIndex = 5;
            this.chkInsertNHibernate.Text = "NHibernate";
            this.chkInsertNHibernate.UseVisualStyleBackColor = true;
            this.chkInsertNHibernate.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkInsertEntity
            // 
            this.chkInsertEntity.AutoSize = true;
            this.chkInsertEntity.Location = new System.Drawing.Point(424, 19);
            this.chkInsertEntity.Name = "chkInsertEntity";
            this.chkInsertEntity.Size = new System.Drawing.Size(107, 17);
            this.chkInsertEntity.TabIndex = 4;
            this.chkInsertEntity.Text = "Entity Framework";
            this.chkInsertEntity.UseVisualStyleBackColor = true;
            this.chkInsertEntity.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkInsertSpeed
            // 
            this.chkInsertSpeed.AutoSize = true;
            this.chkInsertSpeed.Location = new System.Drawing.Point(20, 19);
            this.chkInsertSpeed.Name = "chkInsertSpeed";
            this.chkInsertSpeed.Size = new System.Drawing.Size(57, 17);
            this.chkInsertSpeed.TabIndex = 0;
            this.chkInsertSpeed.Text = "Speed";
            this.chkInsertSpeed.UseVisualStyleBackColor = true;
            this.chkInsertSpeed.Click += new System.EventHandler(this.chk_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtTestSelect);
            this.panel1.Controls.Add(this.rbtTestInsert);
            this.panel1.Location = new System.Drawing.Point(6, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(779, 49);
            this.panel1.TabIndex = 0;
            this.panel1.TabStop = false;
            this.panel1.Text = "Select Test";
            // 
            // rbtTestSelect
            // 
            this.rbtTestSelect.AutoSize = true;
            this.rbtTestSelect.Location = new System.Drawing.Point(417, 20);
            this.rbtTestSelect.Name = "rbtTestSelect";
            this.rbtTestSelect.Size = new System.Drawing.Size(90, 17);
            this.rbtTestSelect.TabIndex = 0;
            this.rbtTestSelect.Text = "Test SELECT";
            this.rbtTestSelect.UseVisualStyleBackColor = true;
            this.rbtTestSelect.CheckedChanged += new System.EventHandler(this.chkTestInsert_CheckedChanged);
            // 
            // rbtTestInsert
            // 
            this.rbtTestInsert.AutoSize = true;
            this.rbtTestInsert.Checked = true;
            this.rbtTestInsert.Location = new System.Drawing.Point(271, 20);
            this.rbtTestInsert.Name = "rbtTestInsert";
            this.rbtTestInsert.Size = new System.Drawing.Size(89, 17);
            this.rbtTestInsert.TabIndex = 0;
            this.rbtTestInsert.TabStop = true;
            this.rbtTestInsert.Text = "Test INSERT";
            this.rbtTestInsert.UseVisualStyleBackColor = true;
            this.rbtTestInsert.CheckedChanged += new System.EventHandler(this.chkTestInsert_CheckedChanged);
            // 
            // gpbTestSelect
            // 
            this.gpbTestSelect.Controls.Add(this.chkSelectDataReader);
            this.gpbTestSelect.Controls.Add(this.chkSelectSpeed);
            this.gpbTestSelect.Controls.Add(this.chkSelectNHibernate);
            this.gpbTestSelect.Controls.Add(this.chkSelectEntity);
            this.gpbTestSelect.Controls.Add(this.chkSelectDataTable);
            this.gpbTestSelect.Controls.Add(this.chkSelectDapper);
            this.gpbTestSelect.Location = new System.Drawing.Point(6, 114);
            this.gpbTestSelect.Name = "gpbTestSelect";
            this.gpbTestSelect.Size = new System.Drawing.Size(779, 45);
            this.gpbTestSelect.TabIndex = 2;
            this.gpbTestSelect.TabStop = false;
            this.gpbTestSelect.Text = "Test SELECT";
            // 
            // chkSelectDataReader
            // 
            this.chkSelectDataReader.AutoSize = true;
            this.chkSelectDataReader.Location = new System.Drawing.Point(238, 21);
            this.chkSelectDataReader.Name = "chkSelectDataReader";
            this.chkSelectDataReader.Size = new System.Drawing.Size(84, 17);
            this.chkSelectDataReader.TabIndex = 2;
            this.chkSelectDataReader.Text = "DataReader";
            this.chkSelectDataReader.UseVisualStyleBackColor = true;
            this.chkSelectDataReader.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkSelectSpeed
            // 
            this.chkSelectSpeed.AutoSize = true;
            this.chkSelectSpeed.Location = new System.Drawing.Point(20, 19);
            this.chkSelectSpeed.Name = "chkSelectSpeed";
            this.chkSelectSpeed.Size = new System.Drawing.Size(57, 17);
            this.chkSelectSpeed.TabIndex = 0;
            this.chkSelectSpeed.Text = "Speed";
            this.chkSelectSpeed.UseVisualStyleBackColor = true;
            this.chkSelectSpeed.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkSelectNHibernate
            // 
            this.chkSelectNHibernate.AutoSize = true;
            this.chkSelectNHibernate.Location = new System.Drawing.Point(658, 19);
            this.chkSelectNHibernate.Name = "chkSelectNHibernate";
            this.chkSelectNHibernate.Size = new System.Drawing.Size(80, 17);
            this.chkSelectNHibernate.TabIndex = 5;
            this.chkSelectNHibernate.Text = "NHibernate";
            this.chkSelectNHibernate.UseVisualStyleBackColor = true;
            this.chkSelectNHibernate.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkSelectEntity
            // 
            this.chkSelectEntity.AutoSize = true;
            this.chkSelectEntity.Location = new System.Drawing.Point(498, 21);
            this.chkSelectEntity.Name = "chkSelectEntity";
            this.chkSelectEntity.Size = new System.Drawing.Size(110, 17);
            this.chkSelectEntity.TabIndex = 4;
            this.chkSelectEntity.Text = "Entity FrameWork";
            this.chkSelectEntity.UseVisualStyleBackColor = true;
            this.chkSelectEntity.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkSelectDataTable
            // 
            this.chkSelectDataTable.AutoSize = true;
            this.chkSelectDataTable.Location = new System.Drawing.Point(372, 21);
            this.chkSelectDataTable.Name = "chkSelectDataTable";
            this.chkSelectDataTable.Size = new System.Drawing.Size(76, 17);
            this.chkSelectDataTable.TabIndex = 3;
            this.chkSelectDataTable.Text = "DataTable";
            this.chkSelectDataTable.UseVisualStyleBackColor = true;
            this.chkSelectDataTable.Click += new System.EventHandler(this.chk_Click);
            // 
            // chkSelectDapper
            // 
            this.chkSelectDapper.AutoSize = true;
            this.chkSelectDapper.Location = new System.Drawing.Point(127, 21);
            this.chkSelectDapper.Name = "chkSelectDapper";
            this.chkSelectDapper.Size = new System.Drawing.Size(61, 17);
            this.chkSelectDapper.TabIndex = 1;
            this.chkSelectDapper.Text = "Dapper";
            this.chkSelectDapper.UseVisualStyleBackColor = true;
            this.chkSelectDapper.Click += new System.EventHandler(this.chk_Click);
            // 
            // grbTest
            // 
            this.grbTest.Controls.Add(this.numThreads);
            this.grbTest.Controls.Add(this.numRecords);
            this.grbTest.Controls.Add(this.label2);
            this.grbTest.Controls.Add(this.lblTotlaRecords);
            this.grbTest.Controls.Add(this.label1);
            this.grbTest.Controls.Add(this.btnExecSelected);
            this.grbTest.Location = new System.Drawing.Point(6, 168);
            this.grbTest.Name = "grbTest";
            this.grbTest.Size = new System.Drawing.Size(779, 63);
            this.grbTest.TabIndex = 3;
            this.grbTest.TabStop = false;
            this.grbTest.Text = "Test parameters";
            // 
            // numThreads
            // 
            this.numThreads.Location = new System.Drawing.Point(426, 33);
            this.numThreads.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThreads.Name = "numThreads";
            this.numThreads.Size = new System.Drawing.Size(107, 20);
            this.numThreads.TabIndex = 1;
            this.numThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThreads.ValueChanged += new System.EventHandler(this.numThreads_ValueChanged);
            // 
            // numRecords
            // 
            this.numRecords.Location = new System.Drawing.Point(426, 10);
            this.numRecords.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numRecords.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRecords.Name = "numRecords";
            this.numRecords.Size = new System.Drawing.Size(107, 20);
            this.numRecords.TabIndex = 0;
            this.numRecords.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRecords.ValueChanged += new System.EventHandler(this.numRecords_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(370, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Threads";
            // 
            // lblTotlaRecords
            // 
            this.lblTotlaRecords.AutoSize = true;
            this.lblTotlaRecords.Location = new System.Drawing.Point(539, 10);
            this.lblTotlaRecords.Name = "lblTotlaRecords";
            this.lblTotlaRecords.Size = new System.Drawing.Size(47, 13);
            this.lblTotlaRecords.TabIndex = 2;
            this.lblTotlaRecords.Text = "Records";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(370, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Records";
            // 
            // btnExecSelected
            // 
            this.btnExecSelected.Location = new System.Drawing.Point(628, 33);
            this.btnExecSelected.Name = "btnExecSelected";
            this.btnExecSelected.Size = new System.Drawing.Size(145, 23);
            this.btnExecSelected.TabIndex = 2;
            this.btnExecSelected.Text = "Run selected tests";
            this.btnExecSelected.UseVisualStyleBackColor = true;
            this.btnExecSelected.Click += new System.EventHandler(this.btnExecSelected_Click);
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.BackColor = System.Drawing.Color.White;
            this.txtResults.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResults.Location = new System.Drawing.Point(6, 31);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Size = new System.Drawing.Size(767, 460);
            this.txtResults.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.btnCopy);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.txtResults);
            this.groupBox1.Location = new System.Drawing.Point(6, 237);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(779, 496);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Results";
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(733, 9);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(40, 20);
            this.btnCopy.TabIndex = 6;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(691, 9);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(40, 20);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // FormTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 733);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTest);
            this.Controls.Add(this.gpbTestSelect);
            this.Controls.Add(this.gpbTestInsert);
            this.Controls.Add(this.panel1);
            this.Name = "FormTests";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Performance Tests";
            this.Load += new System.EventHandler(this.FormTests_Load);
            this.gpbTestInsert.ResumeLayout(false);
            this.gpbTestInsert.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gpbTestSelect.ResumeLayout(false);
            this.gpbTestSelect.PerformLayout();
            this.grbTest.ResumeLayout(false);
            this.grbTest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRecords)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpbTestInsert;
        private System.Windows.Forms.GroupBox panel1;
        private System.Windows.Forms.RadioButton rbtTestInsert;
        private System.Windows.Forms.CheckBox chkInsertEntity;
        private System.Windows.Forms.CheckBox chkInsertNHibernate;
        private System.Windows.Forms.CheckBox chkInsertSql;
        private System.Windows.Forms.CheckBox chkInsertProcedure;
        private System.Windows.Forms.CheckBox chkInsertSpeed;
        private System.Windows.Forms.GroupBox gpbTestSelect;
        private System.Windows.Forms.GroupBox grbTest;
        private System.Windows.Forms.RadioButton rbtTestSelect;
        private System.Windows.Forms.CheckBox chkInsertDapper;
        private System.Windows.Forms.Button btnExecSelected;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.CheckBox chkSelectDataReader;
        private System.Windows.Forms.CheckBox chkSelectSpeed;
        private System.Windows.Forms.CheckBox chkSelectNHibernate;
        private System.Windows.Forms.CheckBox chkSelectEntity;
        private System.Windows.Forms.CheckBox chkSelectDataTable;
        private System.Windows.Forms.CheckBox chkSelectDapper;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numThreads;
        private System.Windows.Forms.NumericUpDown numRecords;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.CheckBox chkInsertSpeedXml;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblTotlaRecords;
    }
}