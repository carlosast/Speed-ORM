using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Speed.Data;
using Speed.Data.Generation;
using Speed.UI.Forms;
using System.Xml;
using Speed.Common;
using Speed.Windows;

namespace Speed.UI.UserControls
{

    public partial class CtlBrowser : UserControl
    {

        #region Declarations

        NodeDatabase nodeDb;
        private SpeedFile file;
        SourceGrid.Cells.Views.Cell headerView;
        Dictionary<string, string> numericTypes;
        bool isLoading;
        FormProgress progress;

        #endregion Declarations

        #region Constructors

        public CtlBrowser()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpeedFile File
        {
            get
            {
                if (!this.DesignMode)
                    ViewToData();
                return file;
            }
            set
            {
                file = value;
                if (!this.DesignMode)
                {
                    ctlDataPars.Parameters = file.Parameters.DataClass;
                    ctlBusPars.Parameters = file.Parameters.BusinessClass;
                    DataToView();
                }
            }
        }

        #endregion Properties

        #region Events

        private void CtlBrowser_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                Show();
                splitContainer1.SplitterDistance = (int)(0.3 * splitContainer1.SplitterDistance);

                numericTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                numericTypes.Add("Boolean", null);
                numericTypes.Add("Byte", null);
                numericTypes.Add("SByte", null);
                //numericTypes.Add("Char", null);
                numericTypes.Add("Decimal", null);
                numericTypes.Add("Double", null);
                numericTypes.Add("Single", null);
                numericTypes.Add("Int32", null);
                numericTypes.Add("UInt32", null);
                numericTypes.Add("Int64", null);
                numericTypes.Add("UInt64", null);
                //numericTypes.Add("Object", null);
                numericTypes.Add("Int16", null);
                numericTypes.Add("UInt16", null);
                //numericTypes.Add("String", null);
            }
            foreach (TabPage page in tabObjects.TabPages)
                page.AutoScroll = true;
        }

        private void btnApplyNames_Click(object sender, EventArgs e)
        {
            //@"Confirms the application of names to classes and enums?
            //The current names will be overwritten"))

            var options = Speed.UI.Forms.FormApply.GetOptions(this);

            ApplyNames(options);
        }

        public void ApplyNames(Forms.FormApply.ApplyOptions options = null)
        {
            if (options == null)
                return;

            //@"Confirms the application of names to classes and enums?
            //The current names will be overwritten"))

            if (options == null)
                return;

            if (Program.RunSafe(this, () =>
            {
                trv.Focus();
                tabObjects.Focus();

                ViewToData();
                var pars = file.Parameters;

                var crtl = this.ActiveControl;
                btnApplyNames.Focus();



                foreach (var table in pars.Tables)
                {
                    if (!options.OnlySelected || table.IsSelected)
                    {
                        ApplyName(options, table);
                    }
                }

                foreach (var table in pars.Views)
                {
                    if (!options.OnlySelected || table.IsSelected)
                    {
                        ApplyName(options, table);
                    }
                }

                DataToView();
                UpdateGrid();
                ViewToData();
                this.ActiveControl = crtl;
            })) ;
        }

        void ApplyName(Speed.UI.Forms.FormApply.ApplyOptions options, GenTable table)
        {
            if (options.ApplyDataClass(table.DataClassName))
            {
                table.DataClassName = ctlDataPars.ApplyNames(table.SchemaName, table.TableName, true);
            }

            foreach (GenColumn col in table.Columns)
            {
                if (options.ApplyDataClass(col.PropertyName))
                {
                    col.PropertyName = ctlDataPars.ApplyNames(null, col.ColumnName, false);
                }
            }

            if (options.ApplyBusinessClass(table.BusinessClassName))
                table.BusinessClassName = ctlBusPars.ApplyNames(table.SchemaName, table.TableName, true);

            if (!string.IsNullOrEmpty(table.EnumColumnId) && !string.IsNullOrEmpty(table.EnumColumnName) && options.ApplyEnum(table.EnumName))
                table.EnumName = ctlDataPars.ApplyNames(null, "EnumDb" + table.DataClassName, true);

            table.SubDirectory = (file.Parameters.ArrangeDirectoriesBySchema && !string.IsNullOrEmpty(table.SchemaName))
                ? Database.GetName(table.SchemaName, file.Parameters.DataClass.NameCase)
                : null;
        }

        bool isNodeChecking;
        private void trv_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (isLoading | isNodeChecking | trv.HandleMultiSelection)
                return;

            isNodeChecking = true;

            SourceGrid.GridRow row = e.Node.Tag as SourceGrid.GridRow;
            if (row != null)
            {
                row.Visible = e.Node.Checked;
                SelectTab(row.Grid);
            }

            isNodeChecking = false;
        }

        bool isNodeSelecting;
        private void trv_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (isNodeSelecting)
                return;

            isNodeSelecting = true;

            var row = e.Node.Tag as SourceGrid.GridRow;
            if (row != null)
            {
                var grid = row.Grid;
                grid.SuspendLayout();
                for (int i = 0; i < grid.Rows.Count; i++)
                    grid.Selection.SelectRow(i, false);
                grid.Selection.FocusRow(row.Index);
                grid.Selection.SelectRow(row.Index, true);
                grid.ResumeLayout();
                SelectTab(grid);
            }

            if (e.Node is NodeViews || e.Node is NodeView)
                tabObjects.SelectedIndex = 1;
            else if (e.Node is NodeTables || e.Node is NodeTable)
                tabObjects.SelectedIndex = 0;

            isNodeSelecting = false;
        }

        private void trv_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var node = (NodeBase)e.Node;
            if (!node.IsLoaded)
            {
                Program.RunSafeInDb(this, (db) => node.Fill(db));
            }
        }

        private void grdTables_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        void Selection_FocusRowEntered(object sender, SourceGrid.RowEventArgs e)
        {
            Program.RunSafe(() =>
            {
                var sel = (SourceGrid.Selection.FreeSelection)sender;
                var grid = (SourceGrid.Grid)sel.Grid;
                var row = grid.Rows[e.Row];
                //row.Tag = node
                var node = row.Tag as NodeTable;
                if (node != null)
                {
                    LoadColumns(grid == grdTables ? grdTableColumns : grdViewColumns, node);
                }
            });
        }

        void Selection_FocusRowLeaving(object sender, SourceGrid.RowCancelEventArgs e)
        {
        }

        void Selection_FocusRowLeavingColumns(object sender, SourceGrid.RowCancelEventArgs e)
        {
            Program.RunSafe(() =>
            {
                var sel = (SourceGrid.Selection.FreeSelection)sender;
                var grid = (SourceGrid.Grid)sel.Grid;
                var row = grid.Rows[e.Row];
                //row.Tag = node
                var node = row.Tag as NodeTable;
                GenTable table = node.Item;

                if (table != null)
                {
                    var cols = table.Columns.ToDictionary(p => p.ColumnName);

                    //addHeader(grid, 0, "ColumnName", 130);
                    //addHeader(grid, 1, "PropertyName", 130);
                    //addHeader(grid, 2, "Title", 250);
                    //addHeader(grid, 3, "DataType", 100);
                    //addHeader(grid, 4, "Required", 70);
                    //addHeader(grid, 5, "Description", 300);

                    for (int r = 1; r < grid.Rows.Count; r++)
                    {
                        var colName = parseString(grid, r, 0);
                        var col = cols[colName];
                        col.PropertyName = parseString(grid, r, 1);
                        col.Title = parseString(grid, r, 2);
                        col.DataType = parseString(grid, r, 3);
                        col.IsRequired = Conv.ToBoolean(((SourceGrid.Cells.Cell)grid.GetCell(r, 4)).Value);
                        col.Description = parseString(grid, r, 5);
                        col.DataAnnotation = parseString(grid, r, 6);
                    }
                }
            });
        }

        #endregion Events

        #region Methods

        public void Fill(Database db, ConnectionInfo ci)
        {
            isLoading = true;
            trv.Nodes.Clear();
            string time;

            var tc = new TimerCount();

            file.Parameters.Tables.ForEach(p => p.IsSelected = false);
            file.Parameters.Views.ForEach(p => p.IsSelected = false);

            using (progress = new FormProgress())
            {
                //f.StartProgress(1, count);
                progress.SetMessage("Loading objects");
                progress.Show(this);
                progress.Update();

                try
                {
                    tc.Next("NodeDatabase");
                    nodeDb = new NodeDatabase(ci.Text);
                    nodeDb.Fill(db);
                    trv.Nodes.Add(nodeDb);

                    trv.HandleMultiSelection = true;

                    tc.Next("ExpandAllForce");
                    trv.ExpandAllForce(db);

                    tc.Next("DataToView");
                    DataToView();
                    tc.Next("LoadGrid");
                    LoadGrid(db);
                    tc.Next("nodeDb.Expand()");
                    nodeDb.Expand();

                    nodeDb.Checked = false;
                    nodeDb.UncheckChildren();
                    nodeDb.Checked = false;

                    time = tc.ToString();
                }
                catch (Exception ex)
                {
                    time = tc.ToString();
                    Program.ShowError(ex);
                }
            }

            //ControlUtil.ChangeFontOnlyParent(tabObjects, 12);

            isLoading = false;

#if DEBUG2
            if (db.ProviderType == EnumDbProviderType.Oracle)
                Program.ShowInformation(time);
#endif
        }

        void addHeader(SourceGrid.Grid grid, int col, string text, int minWidth)
        {
            SourceGrid.Cells.ColumnHeader header;
            grid[0, col] = header = new SourceGrid.Cells.ColumnHeader(text);
            header.Column.MinimalWidth = minWidth;
            header.Column.Width = minWidth;
            header.View = headerView;
        }

        T GetNode<T>() where T : NodeBase
        {
            foreach (var node in nodeDb.Nodes)
                if (node is T)
                    return (T)node;
            return null;
        }

        public void DataToView()
        {
            isLoading = true;
            if (nodeDb == null)
                return;

            if (file == null)
                return;

            ctlDataPars.Parameters = file.Parameters.DataClass;
            ctlBusPars.Parameters = file.Parameters.BusinessClass;

            var dicTables = file.Parameters.Tables.ToDictionary(p => p.FullName);

            NodeTables nodeTables = GetNode<NodeTables>();
            // indexa pra performance
            var tables = file.Parameters.Tables.ToDictionary(p => p.FullName);
            foreach (NodeTable node in nodeTables.Nodes)
            {
                GenTable table;
                if (tables.TryGetValue(node.Item.FullName, out table))
                {
                    node.Checked = table.IsSelected;
                    node.Item.BusinessClassName = table.BusinessClassName;
                    node.Item.DataClassName = table.DataClassName;
                    node.Item.EnumColumnName = table.EnumColumnName;
                    node.Item.EnumColumnId = table.EnumColumnId;
                    node.Item.EnumName = table.EnumName;
                    node.Item.SequenceColumn = table.SequenceColumn;
                    node.Item.SequenceName = table.SequenceName;
                    node.Item.SchemaName = table.SchemaName;
                    node.Item.SubDirectory = table.SubDirectory;
                    node.Item.Columns = table.Columns;
                    node.Item.DataAnnotation = table.DataAnnotation;

                    dicTables.Remove(table.FullName);
                }
                else
                {
                    node.Checked = false;
                    file.Parameters.Tables.Add(node.Item);
                }
            }

            NodeViews nodeViews = GetNode<NodeViews>();
            // indexa pra performance
            var views = file.Parameters.Views.ToDictionary(p => p.FullName);
            foreach (NodeView node in nodeViews.Nodes)
            {
                GenTable view;
                if (views.TryGetValue(node.Item.FullName, out view))
                {
                    node.Checked = view.IsSelected;
                    node.Item.BusinessClassName = view.BusinessClassName;
                    node.Item.DataClassName = view.DataClassName;
                    node.Item.EnumColumnName = view.EnumColumnName;
                    node.Item.EnumColumnId = view.EnumColumnId;
                    node.Item.EnumName = view.EnumName;
                    node.Item.SequenceColumn = view.SequenceColumn;
                    node.Item.SequenceName = view.SequenceName;
                    node.Item.SchemaName = view.SchemaName;
                    node.Item.SubDirectory = view.SubDirectory;
                    node.Item.Columns = view.Columns;
                    node.Item.DataAnnotation = view.DataAnnotation;
                }
                else
                {
                    node.Checked = false;
                    file.Parameters.Views.Add(node.Item);
                }
            }

            /*
            NodeProcedures nodeProcs = GetNode<NodeProcedures>();
            if (nodeProcs != null)
            {
                // indexa pra performance
                var procs = file.Parameters.Procedures.ToDictionary(p => p.FullName);
                foreach (NodeProcedure node in nodeProcs.Nodes)
                {
                    GenProcedure proc;
                    if (procs.TryGetValue(node.Item.FullName, out proc))
                        node.Checked = proc.IsSelected;
                    else
                    {
                        node.Checked = false;
                        file.Parameters.Procedures.Add(node.Item.ToGenProcedure());
                    }
                }
            }
            */

            chkRaisePropertyChanged.Checked = file.Parameters.RaisePropertyChanged;
            UpdateGrid();
            isLoading = false;
        }

        public void ViewToData()
        {
            viewToData(grdTables);
            viewToData(grdViews);
            file.Parameters.RaisePropertyChanged = chkRaisePropertyChanged.Checked;
        }

        public void viewToData(SourceGrid.Grid grid)
        {
            // TODO: pra terminar o edit. Não achei que método do grid faz isso
            trv.Focus();
            grdTables.Focus();

            file.Parameters.DataClass = ctlDataPars.Parameters;
            file.Parameters.BusinessClass = ctlBusPars.Parameters;

            foreach (SourceGrid.GridRow row in grid.Rows)
            {
                var node = row.Tag as NodeTable;
                if (node != null)
                {
                    GenTable tb = node.Item;
                    if (tb.TableName == "ContactType")
                        ToString();
                    int r = row.Index;
                    tb.SchemaName = parseString(grid, r, 0);
                    tb.TableName = parseString(grid, r, 1);
                    tb.DataClassName = parseString(grid, r, 2);
                    tb.BusinessClassName = parseString(grid, r, 3);
                    tb.EnumColumnName = parseString(grid, r, 4);
                    tb.EnumColumnId = parseString(grid, r, 5);
                    tb.EnumName = parseString(grid, r, 6);
                    tb.SequenceColumn = parseString(grid, r, 7);
                    tb.SequenceName = parseString(grid, r, 8);
                    tb.DataAnnotation = parseString(grid, r, 9);
                    //tb.SubDirectory = parseString(grid, r, 5);
                }
            }

            if (nodeDb == null)
                return;

            NodeTables nodeTables = GetNode<NodeTables>();
            var tables = file.Parameters.Tables.ToDictionary(p => p.FullName);
            if (nodeTables != null)
            {
                foreach (NodeTable node in nodeTables.Nodes)
                {
                    GenTable table;
                    if (tables.TryGetValue(node.Item.FullName, out table))
                    {
                        table.IsSelected = node.Checked;
                        table.BusinessClassName = node.Item.BusinessClassName;
                        table.DataClassName = node.Item.DataClassName;
                        table.EnumColumnName = node.Item.EnumColumnName;
                        table.EnumColumnId = node.Item.EnumColumnId;
                        table.EnumName = node.Item.EnumName;
                        table.SequenceColumn = node.Item.SequenceColumn;
                        table.SequenceName = node.Item.SequenceName;
                        table.SchemaName = node.Item.SchemaName;
                        table.SubDirectory = node.Item.SubDirectory;
                        table.Columns = node.Item.Columns;
                        table.DataAnnotation = node.Item.DataAnnotation;
                    }
                    else
                        node.Checked = false;
                }
            }

            NodeViews nodeViews = GetNode<NodeViews>();
            var views = file.Parameters.Views.ToDictionary(p => p.FullName);
            if (nodeViews != null)
            {
                foreach (NodeView node in nodeViews.Nodes)
                {
                    GenTable view;
                    if (views.TryGetValue(node.Item.FullName, out view))
                    {
                        view.IsSelected = node.Checked;
                        view.BusinessClassName = node.Item.BusinessClassName;
                        view.DataClassName = node.Item.DataClassName;
                        view.EnumColumnName = node.Item.EnumColumnName;
                        view.EnumColumnId = node.Item.EnumColumnId;
                        view.EnumName = node.Item.EnumName;
                        view.SequenceColumn = node.Item.SequenceColumn;
                        view.SequenceName = node.Item.SequenceName;
                        view.SchemaName = node.Item.SchemaName;
                        view.SubDirectory = node.Item.SubDirectory;
                        view.Columns = node.Item.Columns;
                        view.DataAnnotation = node.Item.DataAnnotation;
                    }
                    else
                        view.IsSelected = false;
                }
            }

            /*
            NodeProcedure nodeProcs = GetNode<NodeProcedure>();
            var procs = file.Parameters.Procedures.ToDictionary(p => p.FullName);
            if (nodeProcs != null)
            {
                foreach (NodeProcedure node in nodeProcs.Nodes)
                {
                    GenProcedure proc;
                    if (procs.TryGetValue(node.Item.FullName, out proc))
                        node.Checked = proc.IsSelected;
                    else
                        node.Checked = false;
                }
            }
            */
            ctlDataPars.ViewToData();
            ctlBusPars.ViewToData();
        }

        string parseString(SourceGrid.Grid grid, int r, int c)
        {
            object value = ((SourceGrid.Cells.Cell)grid.GetCell(r, c)).Value;
            string val = Conv.Trim(value);
            return string.IsNullOrEmpty(val) ? null : val;
        }


        void LoadGrid(Database db)
        {
            var node = GetNode<NodeTables>();
            progress.StartProgress(0, node.Nodes.Count);
            progress.SetMessage("Processing Tables ...");
            LoadTables(db, grdTables, "Tables", "Table Name", node);

            progress.StartProgress(0, node.Nodes.Count);
            progress.SetMessage("Processing Views ...");
            LoadTables(db, grdViews, "Views", "View Name", GetNode<NodeViews>());
        }

        void UpdateGrid()
        {
            updateGrid(grdTables);
            updateGrid(grdViews);
        }

        void updateGrid(SourceGrid.Grid grid)
        {
            foreach (SourceGrid.GridRow row in grid.Rows)
            {
                var node = row.Tag as NodeTable;
                if (node != null)
                {
                    GenTable tb = node.Item;

                    if (tb.TableName.ToUpper().StartsWith("VW"))
                        ToString();

                    int r = row.Index;
                    if (tb.TableName == "AddressType")
                        ToString();
                    tb.CheckEnum();
                    grid[r, 0].Value = tb.SchemaName;
                    grid[r, 1].Value = tb.TableName;
                    grid[r, 2].Value = tb.DataClassName;
                    grid[r, 3].Value = tb.BusinessClassName;
                    grid[r, 4].Value = tb.EnumColumnName;
                    grid[r, 5].Value = tb.EnumColumnId;
                    grid[r, 6].Value = tb.EnumName;
                    grid[r, 7].Value = tb.SequenceColumn;
                    grid[r, 8].Value = tb.SequenceName;
                    grid[r, 9].Value = tb.DataAnnotation;
                    //grid[r, 5].Value = tb.SubDirectory;
                }
            }
            grid.AutoSizeCells();
        }

        void LoadTables(Database db, SourceGrid.Grid grid, string title, string nameTitle, NodeBase tableViewNode)
        {
            var tc = new TimerCount("LoadTables");
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            grid.ClipboardMode = SourceGrid.ClipboardMode.All;

            grid.Rows.Clear();
            grid.Columns.Clear();

            grid.ColumnsCount = 10;
            grid.FixedRows = 1;
            grid.FixedColumns = 2;

            //int r = 0;
            SourceGrid.Cells.Views.Cell categoryView = new SourceGrid.Cells.Views.Cell();
            categoryView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Gainsboro, Color.WhiteSmoke, 0);
            categoryView.ForeColor = Color.Black;
            categoryView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            categoryView.Border = DevAge.Drawing.RectangleBorder.NoBorder;
            categoryView.Font = new Font(Font, FontStyle.Bold);

            //grid.Rows.Insert(r);
            //grid[r, 0] = new SourceGrid.Cells.Cell(title);
            //grid[r, 0].View = categoryView;
            //grid[r, 0].ColumnSpan = grid.ColumnsCount;

            //r++;
            int r = 0;
            grid.Rows.Insert(r);
            headerView = new SourceGrid.Cells.Views.Cell();
            //headerView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Gainsboro, Color.WhiteSmoke, 0);
            headerView.ForeColor = Color.FromKnownColor(KnownColor.Black);
            headerView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            headerView.Border = DevAge.Drawing.RectangleBorder.NoBorder;
            headerView.Font = new Font(Font, FontStyle.Bold);

            addHeader(grid, 0, "Schema Name", 100);
            addHeader(grid, 1, "Table Name", 150);
            addHeader(grid, 2, "DataClass Name", 150);
            addHeader(grid, 3, "BusinessClass Name", 150);
            addHeader(grid, 4, "Enum Column Name", 150);
            addHeader(grid, 5, "Enum Column Id", 150);
            addHeader(grid, 6, "Enum Name", 150);
            addHeader(grid, 7, "Sequence Column", 150);
            addHeader(grid, 8, "Sequence Name", 150);
            addHeader(grid, 9, "DataAnnotation", 300);
            //addHeader(grid, 5, "Sub-directory", 150);

            for (int c = 0; c < grid.ColumnsCount; c++)
                grid[r, c].View = categoryView;

            Dictionary<string, GenTable> objects = new Dictionary<string, GenTable>();

            tc.Next("GetSequences");
            var sequences = db.Provider.GetSequences();

            r++;

            foreach (NodeTable node in tableViewNode.Nodes)
            {
                progress.SetProgress();
                //progress.SetProgress("Processing table" + node.Text + " ...");

                var gtb = node.Item;
                if (gtb.TableName == "CAMPOS_MINUTA")
                    ToString();

                objects.Add(gtb.FullName, null);

                tc.Next(gtb.TableName);

                try
                {
                    //tc.Next("GetTableInfo");
                    //var tb = db.GetTableInfo(gtb.SchemaName, gtb.TableName);
                    var tbCols = db.GetColumnsInfo(gtb.SchemaName, gtb.TableName);

                    if (tbCols == null) // objeto inválido
                        continue;

                    Dictionary<string, GenColumn> dict = new Dictionary<string, GenColumn>(StringComparer.InvariantCultureIgnoreCase);
                    foreach (var col in gtb.Columns)
                        if (!dict.ContainsKey(col.ColumnName))
                            dict.Add(col.ColumnName, col);

                    foreach (var col in tbCols)
                    {
                        GenColumn gcol;
                        if (dict.TryGetValue(col.ColumnName, out gcol))
                        {
                            dict.Remove(col.ColumnName); // remove as existentes
                            // se existe, mas o banco é required, força required;
                            if (!col.IsNullable)
                                gcol.IsRequired = true;

                            // o tipo sempre vem do banco
                            gcol.DataType = col.DataTypeDotNet;
                        }
                        else
                        {
                            gcol = new GenColumn();
                            gcol.ColumnName = col.ColumnName;
                            gcol.PropertyName = null;
                            gcol.Description = null;
                            gcol.IsRequired = !col.IsNullable;
                            gcol.DataType = col.DataTypeDotNet;
                            gtb.Columns.Add(gcol);
                        }
                    }

                    // se sobrou no dict, é pq as colunas foram apagadas da table, ou renomedas
                    foreach (var pair in dict)
                        gtb.Columns.RemoveAll(p => p.ColumnName.EqualsICIC(pair.Key));

                    grid.Rows.Insert(r);
                    //grid.Rows.SetHeight(r, 40);
                    //grid.Rows.AutoSizeRow(r);

                    SourceGrid.GridRow row = grid.Rows[r];

                    // tags
                    row.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
                    row.Tag = node;
                    gtb.Tag = tbCols;
                    node.Tag = row;

                    grid[r, 0] = new SourceGrid.Cells.Cell(gtb.SchemaName);
                    grid[r, 1] = new SourceGrid.Cells.Cell(gtb.TableName);

                    SourceGrid.Cells.Editors.EditorBase ed;

                    ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                    ed.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    grid[r, 2] = new SourceGrid.Cells.Cell(gtb.DataClassName, ed);

                    ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                    ed.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    grid[r, 3] = new SourceGrid.Cells.Cell(gtb.BusinessClassName, ed);

                    // EnumColumnName
                    SourceGrid.Cells.Editors.ComboBox cbEditor = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
                    var cols = (from c in tbCols where c.DataTypeDotNet.ToLower().Contains("string") orderby c.ColumnName select c.ColumnName).ToList();
                    if (!string.IsNullOrWhiteSpace(gtb.EnumColumnName))
                    {
                        if (cols.FirstOrDefault(p => p.ToUpper() == gtb.EnumColumnName) == null)
                            gtb.EnumColumnName = gtb.EnumColumnId = gtb.EnumName = null;
                    }
                    else
                        gtb.EnumColumnName = gtb.EnumColumnId = gtb.EnumName = null;

                    if (cols.Count > 0)
                    {
                        cols.Insert(0, "");
                        cbEditor.StandardValues = cols;
                        cbEditor.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    }
                    else
                        cbEditor.EditableMode = SourceGrid.EditableMode.None;
                    grid[r, 4] = new SourceGrid.Cells.Cell(gtb.EnumColumnName, cbEditor);

                    var x = (from c in tbCols orderby c.DataTypeDotNet select c.DataTypeDotNet).Distinct().ToList();

                    // EnumColumnId
                    cbEditor = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
                    cols = (from c in tbCols where numericTypes.ContainsKey(c.DataTypeDotNet.Replace("?", "")) orderby c.ColumnName select c.ColumnName).ToList();
                    if (cols.Count > 0)
                    {
                        cols.Insert(0, "");
                        cbEditor.StandardValues = cols;
                        cbEditor.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    }
                    else
                        cbEditor.EditableMode = SourceGrid.EditableMode.None;
                    grid[r, 5] = new SourceGrid.Cells.Cell(gtb.EnumColumnId, cbEditor);

                    // EnumName
                    ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                    ed.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    grid[r, 6] = new SourceGrid.Cells.Cell(gtb.EnumName, ed);

                    // SequenceColumn
                    cbEditor = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
                    cols = (from c in tbCols where numericTypes.ContainsKey(c.DataTypeDotNet.Replace("?", "")) orderby c.ColumnName select c.ColumnName).ToList();
                    if (cols.Count > 0)
                    {
                        cols.Insert(0, "");
                        cbEditor.StandardValues = cols;
                        cbEditor.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    }
                    else
                        cbEditor.EditableMode = SourceGrid.EditableMode.None;
                    grid[r, 7] = new SourceGrid.Cells.Cell(gtb.SequenceColumn, cbEditor);

                    // SequenceName
                    cbEditor = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
                    if (sequences.Count > 0)
                    {
                        cols.Insert(0, "");
                        cbEditor.StandardValues = (from c in sequences select c.FullName).ToList();
                        cbEditor.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    }
                    else
                        cbEditor.EditableMode = SourceGrid.EditableMode.None;
                    grid[r, 8] = new SourceGrid.Cells.Cell(gtb.SequenceName, cbEditor);

                    var ed2 = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                    var txt = (SourceGrid.Cells.Editors.TextBox)ed2;
                    txt.Control.Multiline = true;
                    txt.Control.AcceptsReturn = true;
                    //txt.Control.ScrollBars = ScrollBars.Both;
                    //txt.Control.ImeMode = ImeMode.Disable;

                    ed2.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    //if (gtb.DataAnnotation != null)
                    //{
                    //    gtb.DataAnnotation =  gtb.DataAnnotation.Replace("\r\n", "\n");
                    //    gtb.DataAnnotation = gtb.DataAnnotation.Replace("\n", "\r\n\r\n");
                    //}

                    grid[r, 9] = new SourceGrid.Cells.Cell(gtb.DataAnnotation, ed2);

                    //ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                    //ed.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                    //grid[r, 5] = new SourceGrid.Cells.Cell(gtb.SubDirectory, ed);
                    r++;
                    //string time = tc.ToString();
                }
                catch (Exception ex)
                {
                    //Program.ShowError(ex);
                }
            }

            // remove objetos não mais existentes na base de dados
            var objs = tableViewNode is NodeTables ? file.Parameters.Tables : file.Parameters.Views;
            foreach (var obj in objs.ToList())
            {
                if (!objects.ContainsKey(obj.FullName))
                    objs.Remove(obj);
            }

            grid.Selection.FocusRowEntered += Selection_FocusRowEntered;
            grid.Selection.FocusRowLeaving += Selection_FocusRowLeaving;
            grid.AutoSizeCells();

            string time = tc.ToString();
        }

        private void Control_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void LoadColumns(SourceGrid.Grid grid, NodeTable node)
        {
            GenTable table = node.Item;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            grid.ClipboardMode = SourceGrid.ClipboardMode.All;

            grid.Rows.Clear();
            grid.Columns.Clear();

            grid.ColumnsCount = 7;
            grid.FixedRows = 0;
            grid.FixedColumns = 2;

            //int r = 0;
            SourceGrid.Cells.Views.Cell categoryView = new SourceGrid.Cells.Views.Cell();
            categoryView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Gainsboro, Color.WhiteSmoke, 0);
            categoryView.ForeColor = Color.Black;
            categoryView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            categoryView.Border = DevAge.Drawing.RectangleBorder.NoBorder;
            categoryView.Font = new Font(Font, FontStyle.Bold);

            //grid.Rows.Insert(r);
            //grid[r, 0] = new SourceGrid.Cells.Cell(title);
            //grid[r, 0].View = categoryView;
            //grid[r, 0].ColumnSpan = grid.ColumnsCount;

            //r++;
            int r = 0;
            grid.Rows.Insert(r);
            grid.FixedRows = 1;
            headerView = new SourceGrid.Cells.Views.Cell();
            //headerView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Gainsboro, Color.WhiteSmoke, 0);
            headerView.ForeColor = Color.FromKnownColor(KnownColor.Black);
            headerView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            headerView.Border = DevAge.Drawing.RectangleBorder.NoBorder;
            headerView.Font = new Font(Font, FontStyle.Bold);

            addHeader(grid, 0, "ColumnName", 130);
            addHeader(grid, 1, "PropertyName", 130);
            addHeader(grid, 2, "Title", 250);
            addHeader(grid, 3, "DataType", 100);
            addHeader(grid, 4, "Required", 70);
            addHeader(grid, 5, "Description", 200);
            addHeader(grid, 6, "DataAnnotation", 300);
            //addHeader(grid, 5, "Sub-directory", 150);

            for (int c = 0; c < grid.ColumnsCount; c++)
                grid[r, c].View = categoryView;

            foreach (GenColumn col in table.Columns)
            {
                r++;

                grid.Rows.Insert(r);
                SourceGrid.GridRow row = grid.Rows[r];

                // tags
                row.Tag = node;

                SourceGrid.Cells.Editors.EditorBase ed;

                ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                grid[r, 0] = new SourceGrid.Cells.Cell(col.ColumnName, ed);

                ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                ed.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                grid[r, 1] = new SourceGrid.Cells.Cell(col.PropertyName, ed);

                ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                ed.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                grid[r, 2] = new SourceGrid.Cells.Cell(col.Title, ed);

                ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                ed.EditableMode = SourceGrid.EditableMode.None;
                grid[r, 3] = new SourceGrid.Cells.Cell(col.DataType, ed);

                ed = SourceGrid.Cells.Editors.Factory.Create(typeof(bool));
                ed.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick;
                grid[r, 4] = new SourceGrid.Cells.Cell(col.IsRequired, ed);

                ed = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                ed.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                grid[r, 5] = new SourceGrid.Cells.Cell(col.Description, ed);

                var ed2 = SourceGrid.Cells.Editors.Factory.Create(typeof(string));
                var txt = (SourceGrid.Cells.Editors.TextBox)ed2;
                txt.Control.Multiline = true;
                txt.Control.AcceptsReturn = true;
                ed2.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;
                grid[r, 6] = new SourceGrid.Cells.Cell(col.DataAnnotation, ed2);
            }
            grid.AutoSizeCells();

            grid.Selection.FocusRowLeaving += Selection_FocusRowLeavingColumns;

        }

        void SelectTab(SourceGrid.GridVirtual grid)
        {
            try
            {
                TabPage page = (TabPage)grid.Parent.Parent.Parent;
                tabObjects.SelectedTab = page;
            }
            catch { }
        }

        #endregion Methods

        private void BtnImportHibernate_Click(object sender, EventArgs e)
        {
            Program.RunSafe(() =>
            {
                using (OpenFileDialog fd = new OpenFileDialog())
                {
                    fd.CheckFileExists = true;
                    fd.CheckPathExists = true;
                    fd.RestoreDirectory = false;

                    if (fd.ShowDialog() == DialogResult.OK)
                        ImportHibernateSchema(fd.FileName);
                }
            });
        }

        void ImportHibernateSchema(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            var node = trv.SelectedNode;

            if (!(node is NodeTable || node is NodeView))
                return;

            var grdColumns = node is NodeView ? grdViewColumns : grdTableColumns;

            // addHeader(grid, 1, "PropertyName", 130);

            Dictionary<string, string> cols = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            XmlNodeList manyToOnes = doc.GetElementsByTagName("many-to-one");
            foreach (XmlNode manyToOne in manyToOnes)
            {
                try
                {
                    var propName = manyToOne.Attributes["name"].Value;
                    var colName = manyToOne.FirstChild.Attributes["name"].Value;
                    cols.Add(colName, "Id" + propName);
                }
                catch { }
            }

            XmlNodeList properties = doc.GetElementsByTagName("property");
            foreach (XmlNode property in properties)
            {
                try
                {
                    var propName = property.Attributes["name"].Value;
                    string colName;
                    if (property.Attributes["column"] != null)
                        colName = property.Attributes["column"].Value;
                    else
                        colName = property.FirstChild.Attributes["name"].Value;
                    cols.Add(colName, propName);
                }
                catch { }
            }

            for (int r = 1; r < grdColumns.RowsCount; r++)
            {
                string colName = grdColumns[r, 0].Value.ToString();
                if (cols.ContainsKey(colName))
                    grdColumns[r, 1].Value = cols[colName];
            }
        }
    }
}
