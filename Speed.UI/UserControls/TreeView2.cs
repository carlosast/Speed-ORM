using System;
using System.Windows.Forms;
using Speed.Data;

namespace Speed.UI.UserControls
{

    public class TreeView2 : TreeView
    {

        public bool HandleMultiSelection { get; set; }
        bool updatingLock = false;

        public TreeView2()
        {
        }

        #region MultiSelection

        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            var node = e.Node as NodeBase;
            if (node != null && !node.Params.AllowCheck)
            {
                e.Cancel = true;
                return;
            }

            base.OnBeforeCheck(e);

            if (HandleMultiSelection)
            {
                if (updatingLock)
                    return;

                TreeNode treeNode = e.Node;
                //   Do not allow Indeterminate state for leaves.
                //if (IsLeafNode(treeNode)) treeNode.NodeStyle = NodeStyle.CheckBox;
            }
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);
            if (HandleMultiSelection)
            {
                if (updatingLock)
                    return;

                TreeNode treeNode = e.Node;

                updatingLock = true; //Begin handling checked state changes of the tree
                HandleTreeCheckedStateChanges(treeNode);

                //checkeds = new List<TreeNode>();
                //GetCheckeds(this.Nodes, checkeds);
                //if (CheckChanged != null)
                //    CheckChanged(this, new CheckChangedArgs(checkeds));

                updatingLock = false; //End handling checked state changes of the tree
            }
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);

            //if (HandleMultiSelection)
            //{
            //    if (updatingLock | this.IsDroppping)
            //        return;

            //    updatingLock = true;
            //    ((NodeBase)e.Node).SetCheck();
            //    updatingLock = false;
            //}
        }

        // <summary>
        // When a tree node has been clicked and its CheckState has
        // changed we need to update the CheckState of the nodes
        // below and above in the tree.
        // </summary>
        void HandleTreeCheckedStateChanges(TreeNode treeNode)
        {
            //   Each time a non-leaf node's CheckState changes
            //   1) Make sure it is Checked or Unchecked - we should not be able to make it indeterminate by clicking!
            //if (treeNode.CheckedState == CheckState.Indeterminate)
            //    treeNode.CheckedState = CheckState.Unchecked; // Since NodeStates: Unchecked => Checked => Indeterminate
            //   2) Set all child nodes to same state.
            SetChildNodesCheckStatesToSameAs(treeNode);

            UpdateParentNodeCheckState(treeNode);
        }

        void SetChildNodesCheckStatesToSameAs(TreeNode treeNode)
        {
            if (IsLeafNode(treeNode))
                return;
            foreach (TreeNode childNode in treeNode.Nodes)
            {
                childNode.Checked = treeNode.Checked;
                SetChildNodesCheckStatesToSameAs(childNode);
            }
        }

        // <summary>
        // Verifies the CheckState of the parent node for the
        // specified treeNode, by setting the parent node's
        // CheckState property based on whether none, some,
        // or all of its child nodes are currently checked.
        // </summary>
        // <param name="treeNode"></param>
        // <remarks>This method only has relevance for non root-level nodes</remarks>
        void UpdateParentNodeCheckState(TreeNode treeNode)
        {
            TreeNode parentNode = treeNode.Parent;
            if (parentNode == null) return; // End recursion

            NodeBase nb = treeNode as NodeBase;
            if (nb != null && !nb.Params.AllowCheck)
                return;

            //    Get the nodes collection to which the specified childNode belongs
            TreeNodeCollection nodesCollection = treeNode.Nodes;

            //    count of the number of checked nodes
            int checkedNodesCount = 0;
            int indeterminateCount = 0;
            foreach (TreeNode node in nodesCollection)
            {
                if (node.Checked) checkedNodesCount += 1;
                //if (node.CheckedState == CheckState.Indeterminate) indeterminateCount += 1;
            }
            //   Set appropriate CheckState of the parentNode
            if (checkedNodesCount == nodesCollection.Count)
                parentNode.Checked = true;
            else if (checkedNodesCount == 0 && indeterminateCount == 0)
                parentNode.Checked = false;
            //else
            //    parentNode.CheckedState = CheckState.Indeterminate;


            //   Traverse up the tree...
            UpdateParentNodeCheckState(parentNode);
        }

        bool IsLeafNode(TreeNode treeNode)
        {
            return treeNode.Nodes.Count == 0;
        }

        #endregion MultiSelection

        public void ExpandAllForce(Database db)
        {
            ExpandAllForce(db, this.Nodes);
        }

        public void ExpandAllForce(Database db, TreeNodeCollection nodes)
        {
            foreach (NodeBase node in nodes)
            {
                if (!node.IsLoaded)
                    node.Fill(db);
                ExpandAllForce(db, node.Nodes);
            }
        }

        /// <summary>
        /// Percorre toda a hierarquia dos nodes e seus filhos
        /// Use este metodo pra aplicar alguma função a todos os nodes
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<TreeNode> action)
        {
            ForEach(action, this.Nodes);
        }

        /// <summary>
        /// Percorre toda a hierarquia dos nodes e seus filhos
        /// Use este metodo pra aplicar alguma função a todos os nodes
        /// </summary>
        /// <param name="action"></param>
        /// <param name="nodes"></param>
        public void ForEach(Action<TreeNode> action, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                action(node);
                ForEach(action, node.Nodes);
            }
        }

    }


}
