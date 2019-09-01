using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Speed.Data.Generation;
using Speed.Windows;
using System.IO;

namespace Speed.UI.UserControls
{

    public partial class CtlGenParameters : UserControl
    {

        #region Declarations

        private SpeedFile file;
        public event EventHandler NameCaseChanged;

        #endregion Declarations

        #region Constructors

        public CtlGenParameters()
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

        private void DbGenParameters_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
            }
        }

        protected void OnNameCaseChanged()
        {
            if (file != null)
            {
                ViewToData();
                if (NameCaseChanged != null)
                    NameCaseChanged(this, EventArgs.Empty);
            }
        }

        private void btnSuggest_Click(object sender, EventArgs e)
        {
            Program.RunSafe(() =>
            {
                using (OpenFileDialog fd = new OpenFileDialog())
                {
                    fd.CheckFileExists = true;
                    fd.CheckPathExists = true;
                    fd.RestoreDirectory = false;

                    if (fd.ShowDialog() == DialogResult.OK)
                        Suggest(fd.FileName);
                }
            });
        }

        #endregion Events

        #region Methods

        public void DataToView()
        {
            txtEntityNameSpace.Text = file.Parameters.DataClass.NameSpace;
            txtBusinnesNameSpace.Text = file.Parameters.BusinessClass.NameSpace;
            oddEntityDirectoryExt.Directory = file.Parameters.DataClass.DirectoryExt;
            oddBusinnesDirectoryExt.Directory = file.Parameters.BusinessClass.DirectoryExt;
            chkArrangeDirectoriesBySchema.Checked = file.Parameters.ArrangeDirectoriesBySchema;
        }

        public void ViewToData()
        {
            file.Parameters.DataClass.NameSpace = txtEntityNameSpace.Text;
            file.Parameters.BusinessClass.NameSpace = txtBusinnesNameSpace.Text;
            
            file.Parameters.DataClass.DirectoryExt = oddEntityDirectoryExt.Directory;
            if (!string.IsNullOrWhiteSpace(file.Parameters.DataClass.DirectoryExt))
                file.Parameters.DataClass.Directory = Path.Combine(file.Parameters.DataClass.DirectoryExt, "Base");
            
            file.Parameters.BusinessClass.DirectoryExt = oddBusinnesDirectoryExt.Directory;
            if (!string.IsNullOrWhiteSpace(file.Parameters.BusinessClass.DirectoryExt))
                file.Parameters.BusinessClass.Directory = Path.Combine(file.Parameters.BusinessClass.DirectoryExt, "Base");

            file.Parameters.ArrangeDirectoriesBySchema = chkArrangeDirectoriesBySchema.Checked;
        }

        public void Suggest(string slnFileName)
        {
            string dir = Path.GetDirectoryName(slnFileName);
            string name = Path.GetFileNameWithoutExtension(slnFileName);

            txtEntityNameSpace.Text = name + ".Data";

            oddEntityDirectoryExt.Directory = Path.Combine(dir, name + ".Data", "Data");

            txtBusinnesNameSpace.Text = name + ".BL";
            oddBusinnesDirectoryExt.Directory = Path.Combine(dir, name + ".BL", "BL");
        }

        public new bool Validate()
        {
            if (string.IsNullOrEmpty(oddEntityDirectoryExt.Directory))
                throw new Exception("Invalid 'Entity Directory'");
            //else if (!Directory.Exists(oddEntityDirectoryExt.Directory))
            //    throw new Exception("'Entity Directory' not found");

            if (string.IsNullOrEmpty(oddBusinnesDirectoryExt.Directory))
                throw new Exception("Invalid 'Business Directory'");
            //else if (!Directory.Exists(oddBusinnesDirectoryExt.Directory))
            //    throw new Exception("'Business Directory' not found");

            return true;
        }
    
        #endregion Methods

        private void cboNameCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnNameCaseChanged();
        }

    }

}
