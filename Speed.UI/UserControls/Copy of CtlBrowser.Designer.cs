namespace Speed.UI.UserControls
{
    partial class CtlBrowser
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grdTables = new System.Windows.Forms.DataGridView();
            this.trv = new Speed.UI.UserControls.TreeView2();
            this.ColSchemaName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDataClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColBusinessClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColEnumColumnName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdTables)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.trv);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grdTables);
            this.splitContainer1.Size = new System.Drawing.Size(898, 390);
            this.splitContainer1.SplitterDistance = 307;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 1;
            // 
            // grdTables
            // 
            this.grdTables.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.grdTables.BackgroundColor = System.Drawing.Color.White;
            this.grdTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdTables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColSchemaName,
            this.ColTableName,
            this.ColDataClassName,
            this.ColBusinessClassName,
            this.ColEnumColumnName});
            this.grdTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdTables.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.grdTables.Location = new System.Drawing.Point(0, 0);
            this.grdTables.Name = "grdTables";
            this.grdTables.Size = new System.Drawing.Size(585, 390);
            this.grdTables.TabIndex = 0;
            this.grdTables.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grdTables_DataError);
            // 
            // trv
            // 
            this.trv.CheckBoxes = true;
            this.trv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trv.HandleMultiSelection = false;
            this.trv.Location = new System.Drawing.Point(0, 0);
            this.trv.Name = "trv";
            this.trv.ShowNodeToolTips = true;
            this.trv.Size = new System.Drawing.Size(307, 390);
            this.trv.TabIndex = 0;
            this.trv.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trv_BeforeExpand);
            this.trv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trv_AfterSelect);
            // 
            // ColSchemaName
            // 
            this.ColSchemaName.DataPropertyName = "SchemaName";
            this.ColSchemaName.HeaderText = "Schema Name";
            this.ColSchemaName.MinimumWidth = 100;
            this.ColSchemaName.Name = "ColSchemaName";
            this.ColSchemaName.ReadOnly = true;
            this.ColSchemaName.Width = 102;
            // 
            // ColTableName
            // 
            this.ColTableName.DataPropertyName = "TableName";
            this.ColTableName.FillWeight = 150F;
            this.ColTableName.HeaderText = "Table Name";
            this.ColTableName.MinimumWidth = 150;
            this.ColTableName.Name = "ColTableName";
            this.ColTableName.ReadOnly = true;
            this.ColTableName.Width = 150;
            // 
            // ColDataClassName
            // 
            this.ColDataClassName.DataPropertyName = "DataClassName";
            this.ColDataClassName.FillWeight = 150F;
            this.ColDataClassName.HeaderText = "Data Class Name";
            this.ColDataClassName.MinimumWidth = 150;
            this.ColDataClassName.Name = "ColDataClassName";
            this.ColDataClassName.Width = 150;
            // 
            // ColBusinessClassName
            // 
            this.ColBusinessClassName.DataPropertyName = "BusinessClassName";
            this.ColBusinessClassName.FillWeight = 150F;
            this.ColBusinessClassName.HeaderText = "Business Class Name";
            this.ColBusinessClassName.MinimumWidth = 150;
            this.ColBusinessClassName.Name = "ColBusinessClassName";
            this.ColBusinessClassName.Width = 150;
            // 
            // ColEnumColumnName
            // 
            this.ColEnumColumnName.DataPropertyName = "EnumColumnName";
            this.ColEnumColumnName.FillWeight = 150F;
            this.ColEnumColumnName.HeaderText = "Enum Column Name";
            this.ColEnumColumnName.MinimumWidth = 150;
            this.ColEnumColumnName.Name = "ColEnumColumnName";
            this.ColEnumColumnName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColEnumColumnName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColEnumColumnName.Width = 150;
            // 
            // CtlBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CtlBrowser";
            this.Size = new System.Drawing.Size(898, 390);
            this.Load += new System.EventHandler(this.CtlBrowser_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdTables)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Speed.UI.UserControls.TreeView2 trv;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView grdTables;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSchemaName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDataClassName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColBusinessClassName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColEnumColumnName;
    }
}
