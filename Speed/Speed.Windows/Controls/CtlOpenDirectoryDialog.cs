using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Speed.Windows.Controls
{
    public partial class CtlOpenDirectoryDialog : UserControl
    {

        public CtlOpenDirectoryDialog()
        {
            InitializeComponent();
        }

        private void CtlOpenDirectoryDialog_Load(object sender, EventArgs e)
        {
            //string path = txtDirectory.Text.Trim();
            //if (path.Length > 0 && System.IO.Directory.Exists(path))
            //    System.Diagnostics.Process.Start(path);
            SetControls();
        }

        #region Properties

        public string Directory
        {
            get { return txtDirectory.Text; }
            set
            {
                txtDirectory.Text = value;
                SetControls();
            }
        }

        #endregion Properties

        #region Events

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog f = new FolderBrowserDialog())
                {
                    f.SelectedPath = txtDirectory.Text;
                    if (f.ShowDialog() == DialogResult.OK)
                        txtDirectory.Text = f.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                ProgramBase.ShowError(ex);
            }
        }

        #endregion Events

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(txtDirectory.Text);
            }
            catch (Exception ex)
            {
                ProgramBase.ShowError(ex);
            }
        }

        private void txtDirectory_TextChanged(object sender, EventArgs e)
        {
            SetControls();
        }

        void SetControls()
        {
            btnView.Enabled = !string.IsNullOrEmpty(txtDirectory.Text.Trim());
        }

    }

}
