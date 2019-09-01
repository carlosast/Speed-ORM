using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Speed.Data;
using Speed.Data.MetaData;
using Speed.Data.Generation;

namespace Speed.UI.UserControls
{

    public class NodeParams
    {

        public bool ShowExpansionIndicator;
        /// <summary>
        /// Flag se o Item permite sub-itens
        /// </summary>
        public bool AllowSubItems = false;
        public int MaxItems = 0;
        public bool AllowCheck = false;
        public bool AllowDrag = false;
        public bool AllowDelete = true;
        public bool AllowEdit = false;
        public bool IsBook = false;
        public bool CanContainFiles = false;
        public bool AllowCut = true;
        public bool AllowCopy = true;
        public bool AllowPaste = true;
        public bool AllowNew = false;
        public bool HasProperty = true;
        public bool UseFileManager = false;
        public bool AllowFile = false;
        /// <summary>
        /// Se faz a operção do dragdrop diretamente ou se envia pra o método Dooperation fazer.
        /// Isto é usado para NodeItem em que as ações podem demorar muito e são realizadas numa janela de progresso
        /// </summary>
        public bool SendOperations = false;

        public NodeParams()
        {
        }

    }

    public class NodeBase : TreeNode
    {

        protected bool isLoaded = false;
        private bool isWorking;
        public NodeParams Params;

        public NodeBase()
        {
            Params = new NodeParams();
            Params.AllowCheck = true;
        }
        public NodeBase(string text)
            : this()
        {
            this.Text = text;
        }

        public new NodeBase Parent
        {
            get
            {
                return (NodeBase)base.Parent;
            }
        }

        public bool IsLoaded
        {
            get { return isLoaded; }
            internal set { isLoaded = value; }
        }

        public virtual void Fill(Database db)
        {
            isLoaded = true;
            this.Nodes.Clear();
        }
        /// <summary>
        /// Se não foi carregado ainda (isLoaded=true), chama o método Fill, senão não faz nada
        /// </summary>
        public void CheckFill(Database db)
        {
            if (!IsLoaded)
                Fill(db);
        }

        internal void AddFake()
        {
            this.Nodes.Add(new NodeBase());
        }

        public void SetCheck()
        {
            if (isWorking)
                return; 

            this.isWorking = true;
            foreach (var n in this.Nodes)
            {
                NodeBase node = n as NodeBase;
                if (node != null)
                {
                    node.isWorking = true;
                    if (node.Params.AllowCheck)
                    {
                        if (node.Checked != this.Checked)
                            node.Checked = this.Checked;
                    }
                    node.isWorking = false;
                }
            }
            this.isWorking = false;
        }

        public void UncheckParents()
        {
            NodeBase node = this;
            while (node.Parent != null)
            {
                node.Parent.Checked = false;
                node = node.Parent;
            }
        }

        public void UncheckChildren()
        {
            foreach (TreeNode n in this.Nodes)
                uncheckChildren(n);
        }

        private void uncheckChildren(TreeNode node)
        {
            //if (!(node is NodeBase))
            //    return;
            if (node.Checked)
                node.Checked = false;
            foreach (TreeNode n in this.Nodes)
            {
                if (n.Checked)
                    n.Checked = false;
                foreach (TreeNode nc in node.Nodes)
                    uncheckChildren(nc);
            }
        }

        public void CheckChildren()
        {
            foreach (TreeNode n in this.Nodes)
                checkChildren(n);
        }

        private void checkChildren(TreeNode node)
        {
            //if (!(node is NodeBase))
            //    return;
            if (!node.Checked)
                node.Checked = true;
            foreach (TreeNode n in this.Nodes)
            {
                if (!n.Checked)
                    n.Checked = true;
                foreach (TreeNode nc in node.Nodes)
                    checkChildren(nc);
            }
        }

    }

    public class NodeTyped<T> : NodeBase
    {

        public T Item;

        public NodeTyped(T item)
            : base()
        {
            this.Item = item;
        }

        public NodeTyped(T item, string text)
            : base(text)
        {
            this.Item = item;
        }

    }

    public class NodeDatabase : NodeBase
    {

        public string DatabaseName;
        public NodeDatabase(string databaseName)
            : base(databaseName)
        {
            this.DatabaseName = databaseName;
        }

        public override void Fill(Database db)
        {
            base.Fill(db);

            this.Nodes.Add(new NodeTables());
            this.Nodes.Add(new NodeViews());
            //this.Nodes.Add(new NodeProcedures());
            //this.Nodes.Add(new NodeFunctions());
        }

    }

    public class NodeTables : NodeBase
    {
        public NodeTables()
            : base("Tables")
        {
            AddFake();
        }

        public override void Fill(Database db)
        {
            base.Fill(db);
            var tables = db.Provider.GetAllTables().Where(p => p.TableType == EnumTableType.Table).OrderBy(p => p.FullName).ToList();
            foreach (var table in tables)
                this.Nodes.Add(new NodeTable(new GenTable(table)));
        }
    }

    public class NodeViews : NodeBase
    {
        public NodeViews()
            : base("Views")
        {
            AddFake();
        }

        public override void Fill(Database db)
        {
            base.Fill(db);
            var tables = db.Provider.GetAllTables().Where(p => p.TableType == EnumTableType.View).OrderBy(p => p.FullName).ToList();
            foreach (var table in tables)
                this.Nodes.Add(new NodeView(new GenTable(table)));
        }

    }

    public class NodeTable : NodeTyped<GenTable>
    {
        public NodeTable(GenTable item)
            : base(item, item.FullName)
        {
        }
    }

    public class NodeView : NodeTable
    {
        public NodeView(GenTable item)
            : base(item)
        {
        }
    }

    public class NodeProcedures : NodeBase
    {
        public NodeProcedures()
            : base("Procedures")
        {
            AddFake();
        }

        public override void Fill(Database db)
        {
            base.Fill(db);
            var list = (from c in db.Routines where c.Value.RoutineType == EnumRoutineType.Procedure orderby c.Value.FullName select c.Value).ToList();
            foreach (var item in list)
                this.Nodes.Add(new NodeProcedure(item));
        }
    }

    public class NodeFunctions : NodeBase
    {
        public NodeFunctions()
            : base("Functions")
        {
            AddFake();
        }

        public override void Fill(Database db)
        {
            base.Fill(db);
            var list = (from c in db.Routines where c.Value.RoutineType == EnumRoutineType.Function orderby c.Value.FullName select c.Value).ToList();
            foreach (var item in list)
                this.Nodes.Add(new NodeFunction(item));
        }
    }

    public class NodeProcedure : NodeTyped<DbRoutineInfo>
    {
        public NodeProcedure(DbRoutineInfo item)
            : base(item, item.FullName)
        {
        }
    }

    public class NodeFunction : NodeProcedure
    {
        public NodeFunction(DbRoutineInfo item)
            : base(item)
        {
        }
    }

}
