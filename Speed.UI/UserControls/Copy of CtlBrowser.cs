using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Speed.Data;
using Speed.Data.MetaData;
using Speed.Data.Generation;

namespace Speed.UI.UserControls
{

    public partial class CtlBrowser : UserControl
    {

        #region Declarations

        NodeDatabase nodeDb;
        private SpeedFile file;
        public event EventHandler DataChanged;

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
                    DataToView();
            }
        }

        #endregion Properties

        #region Events

        private void CtlBrowser_Load(object sender, EventArgs e)
        {
            Show();
            splitContainer1.SplitterDistance = (int)(0.2 * splitContainer1.SplitterDistance);
            grdTables.AutoGenerateColumns = false;
        }

        protected void OnDataChanged()
        {
            ViewToData();
            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }

        private void trv_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void trv_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var node = (NodeBase)e.Node;
            if (!node.IsLoaded)
            {
                Program.RunSafeInDb(this, (db) => node.Fill(db));
            }
        }

        #endregion Events

        #region Methods

        public void Fill(Database db)
        {
            trv.Nodes.Clear();

            nodeDb = new NodeDatabase(db.DatabaseName);
            nodeDb.Fill(db);
            trv.Nodes.Add(nodeDb);
            trv.HandleMultiSelection = true;

            trv.ExpandAllForce(db);
            nodeDb.Expand();

            // Grid
            NodeTables nodeTables = GetNode<NodeTables>();
            List<GenTable> tables = new List<GenTable>();
            foreach (NodeTable node in nodeTables.Nodes)
            {
                var gtb = node.Item;
                var tb = db.GetTableInfo(gtb.SchemaName, gtb.TableName);
                gtb.Tag = tb;
                tables.Add(gtb);
            }
            grdTables.DataSource = tables;

            int count = 0;
            foreach (DataGridViewRow row in grdTables.Rows)
            {
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)row.Cells[this.ColEnumColumnName.Index];
                //for (int i = count; i < count+5; i++) 
                //cell.Items.Add("Cell " + i);

                cell.DataSource = new BindingSource( new string[] { "A " + count, "B " + count }, null);

                count++;
            }
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
            if (nodeDb == null)
                return;

            NodeTables nodeTables = GetNode<NodeTables>();
            // indexa pra performance
            var tables = file.Parameters.Tables.ToDictionary(p => p.FullName);
            foreach (NodeTable node in nodeTables.Nodes)
            {
                GenTable table;
                if (tables.TryGetValue(node.Item.FullName, out table))
                    node.Checked = table.IsSelected;
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
                    node.Checked = view.IsSelected;
                else
                {
                    node.Checked = false;
                    file.Parameters.Views.Add(node.Item);
                }
            }

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
        }

        public void ViewToData()
        {
            NodeTables nodeTables = GetNode<NodeTables>();
            var tables = file.Parameters.Tables.ToDictionary(p => p.FullName);
            if (nodeTables != null)
            {
                foreach (NodeTable node in nodeTables.Nodes)
                {
                    GenTable table;
                    if (tables.TryGetValue(node.Item.FullName, out table))
                        table.IsSelected = node.Checked;
                    else
                        table.IsSelected = false;
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
                        view.IsSelected = node.Checked;
                    else
                        view.IsSelected = false;
                }
            }

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
        }

        #endregion Methods

        private void grdTables_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

    }

}
