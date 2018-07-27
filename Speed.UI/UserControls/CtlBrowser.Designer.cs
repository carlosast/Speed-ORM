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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.trv = new Speed.UI.UserControls.TreeView2();
            this.tabObjects = new System.Windows.Forms.TabControl();
            this.tabTables = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.grdTables = new SourceGrid.Grid();
            this.grdTableColumns = new SourceGrid.Grid();
            this.tabViews = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.grdViews = new SourceGrid.Grid();
            this.grdViewColumns = new SourceGrid.Grid();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panNaming = new System.Windows.Forms.Panel();
            this.grpNaming = new System.Windows.Forms.GroupBox();
            this.chkRaisePropertyChanged = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ctlBusPars = new Speed.UI.UserControls.CtlClassParameters();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ctlDataPars = new Speed.UI.UserControls.CtlClassParameters();
            this.btnApplyNames = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabObjects.SuspendLayout();
            this.tabTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabViews.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panNaming.SuspendLayout();
            this.grpNaming.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.tabObjects);
            this.splitContainer1.Size = new System.Drawing.Size(1093, 255);
            this.splitContainer1.SplitterDistance = 372;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 1;
            // 
            // trv
            // 
            this.trv.CheckBoxes = true;
            this.trv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trv.HandleMultiSelection = false;
            this.trv.Location = new System.Drawing.Point(0, 0);
            this.trv.Name = "trv";
            this.trv.ShowNodeToolTips = true;
            this.trv.Size = new System.Drawing.Size(372, 255);
            this.trv.TabIndex = 0;
            this.trv.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trv_AfterCheck);
            this.trv.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trv_BeforeExpand);
            this.trv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trv_AfterSelect);
            // 
            // tabObjects
            // 
            this.tabObjects.Controls.Add(this.tabTables);
            this.tabObjects.Controls.Add(this.tabViews);
            this.tabObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabObjects.Location = new System.Drawing.Point(0, 0);
            this.tabObjects.Name = "tabObjects";
            this.tabObjects.SelectedIndex = 0;
            this.tabObjects.Size = new System.Drawing.Size(715, 255);
            this.tabObjects.TabIndex = 1;
            // 
            // tabTables
            // 
            this.tabTables.AutoScroll = true;
            this.tabTables.Controls.Add(this.splitContainer2);
            this.tabTables.Location = new System.Drawing.Point(4, 22);
            this.tabTables.Name = "tabTables";
            this.tabTables.Padding = new System.Windows.Forms.Padding(3);
            this.tabTables.Size = new System.Drawing.Size(707, 229);
            this.tabTables.TabIndex = 0;
            this.tabTables.Text = "Tables";
            this.tabTables.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.grdTables);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.grdTableColumns);
            this.splitContainer2.Size = new System.Drawing.Size(701, 223);
            this.splitContainer2.SplitterDistance = 155;
            this.splitContainer2.TabIndex = 6;
            // 
            // grdTables
            // 
            this.grdTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdTables.EnableSort = true;
            this.grdTables.Location = new System.Drawing.Point(0, 0);
            this.grdTables.Name = "grdTables";
            this.grdTables.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grdTables.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grdTables.Size = new System.Drawing.Size(701, 155);
            this.grdTables.TabIndex = 1;
            this.grdTables.TabStop = true;
            this.grdTables.ToolTipText = "";
            // 
            // grdTableColumns
            // 
            this.grdTableColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdTableColumns.EnableSort = true;
            this.grdTableColumns.Location = new System.Drawing.Point(0, 0);
            this.grdTableColumns.Name = "grdTableColumns";
            this.grdTableColumns.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grdTableColumns.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grdTableColumns.Size = new System.Drawing.Size(701, 64);
            this.grdTableColumns.TabIndex = 2;
            this.grdTableColumns.TabStop = true;
            this.grdTableColumns.ToolTipText = "";
            // 
            // tabViews
            // 
            this.tabViews.AutoScroll = true;
            this.tabViews.Controls.Add(this.splitContainer3);
            this.tabViews.Location = new System.Drawing.Point(4, 22);
            this.tabViews.Name = "tabViews";
            this.tabViews.Padding = new System.Windows.Forms.Padding(3);
            this.tabViews.Size = new System.Drawing.Size(707, 229);
            this.tabViews.TabIndex = 1;
            this.tabViews.Text = "Views";
            this.tabViews.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.grdViews);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.grdViewColumns);
            this.splitContainer3.Size = new System.Drawing.Size(701, 223);
            this.splitContainer3.SplitterDistance = 155;
            this.splitContainer3.TabIndex = 7;
            // 
            // grdViews
            // 
            this.grdViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdViews.EnableSort = true;
            this.grdViews.Location = new System.Drawing.Point(0, 0);
            this.grdViews.Name = "grdViews";
            this.grdViews.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grdViews.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grdViews.Size = new System.Drawing.Size(701, 155);
            this.grdViews.TabIndex = 1;
            this.grdViews.TabStop = true;
            this.grdViews.ToolTipText = "";
            // 
            // grdViewColumns
            // 
            this.grdViewColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdViewColumns.EnableSort = true;
            this.grdViewColumns.Location = new System.Drawing.Point(0, 0);
            this.grdViewColumns.Name = "grdViewColumns";
            this.grdViewColumns.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grdViewColumns.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grdViewColumns.Size = new System.Drawing.Size(701, 64);
            this.grdViewColumns.TabIndex = 2;
            this.grdViewColumns.TabStop = true;
            this.grdViewColumns.ToolTipText = "";
            // 
            // panNaming
            // 
            this.panNaming.Controls.Add(this.grpNaming);
            this.panNaming.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panNaming.Location = new System.Drawing.Point(0, 255);
            this.panNaming.Name = "panNaming";
            this.panNaming.Size = new System.Drawing.Size(1093, 135);
            this.panNaming.TabIndex = 1;
            // 
            // grpNaming
            // 
            this.grpNaming.Controls.Add(this.chkRaisePropertyChanged);
            this.grpNaming.Controls.Add(this.groupBox2);
            this.grpNaming.Controls.Add(this.groupBox1);
            this.grpNaming.Controls.Add(this.btnApplyNames);
            this.grpNaming.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpNaming.Location = new System.Drawing.Point(0, 0);
            this.grpNaming.Name = "grpNaming";
            this.grpNaming.Size = new System.Drawing.Size(1093, 135);
            this.grpNaming.TabIndex = 0;
            this.grpNaming.TabStop = false;
            this.grpNaming.Text = "Naming";
            // 
            // chkRaisePropertyChanged
            // 
            this.chkRaisePropertyChanged.AutoSize = true;
            this.chkRaisePropertyChanged.Checked = true;
            this.chkRaisePropertyChanged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRaisePropertyChanged.Location = new System.Drawing.Point(992, 19);
            this.chkRaisePropertyChanged.Name = "chkRaisePropertyChanged";
            this.chkRaisePropertyChanged.Size = new System.Drawing.Size(141, 17);
            this.chkRaisePropertyChanged.TabIndex = 13;
            this.chkRaisePropertyChanged.Text = "RaisePropertyChanged?";
            this.chkRaisePropertyChanged.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ctlBusPars);
            this.groupBox2.Location = new System.Drawing.Point(458, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(432, 110);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Business Classes Parameters";
            // 
            // ctlBusPars
            // 
            this.ctlBusPars.Location = new System.Drawing.Point(7, 16);
            this.ctlBusPars.Name = "ctlBusPars";
            this.ctlBusPars.Size = new System.Drawing.Size(415, 90);
            this.ctlBusPars.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ctlDataPars);
            this.groupBox1.Location = new System.Drawing.Point(11, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(432, 110);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Classes Parameters";
            // 
            // ctlDataPars
            // 
            this.ctlDataPars.Location = new System.Drawing.Point(7, 16);
            this.ctlDataPars.Name = "ctlDataPars";
            this.ctlDataPars.Size = new System.Drawing.Size(415, 93);
            this.ctlDataPars.TabIndex = 0;
            // 
            // btnApplyNames
            // 
            this.btnApplyNames.Location = new System.Drawing.Point(897, 106);
            this.btnApplyNames.Name = "btnApplyNames";
            this.btnApplyNames.Size = new System.Drawing.Size(85, 23);
            this.btnApplyNames.TabIndex = 11;
            this.btnApplyNames.Text = "Apply Names";
            this.btnApplyNames.UseVisualStyleBackColor = true;
            this.btnApplyNames.Click += new System.EventHandler(this.btnApplyNames_Click);
            // 
            // CtlBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panNaming);
            this.Name = "CtlBrowser";
            this.Size = new System.Drawing.Size(1093, 390);
            this.Load += new System.EventHandler(this.CtlBrowser_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabObjects.ResumeLayout(false);
            this.tabTables.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabViews.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.panNaming.ResumeLayout(false);
            this.grpNaming.ResumeLayout(false);
            this.grpNaming.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Speed.UI.UserControls.TreeView2 trv;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panNaming;
        private System.Windows.Forms.GroupBox grpNaming;
        private System.Windows.Forms.Button btnApplyNames;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private CtlClassParameters ctlDataPars;
        private CtlClassParameters ctlBusPars;
        private System.Windows.Forms.TabControl tabObjects;
        private System.Windows.Forms.TabPage tabTables;
        private System.Windows.Forms.TabPage tabViews;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private SourceGrid.Grid grdTables;
        private SourceGrid.Grid grdTableColumns;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private SourceGrid.Grid grdViews;
        private SourceGrid.Grid grdViewColumns;
        private System.Windows.Forms.CheckBox chkRaisePropertyChanged;
    }
}
