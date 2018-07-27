using System;
using System.Windows.Forms;
using System.IO;

namespace Speed.Windows.Controls
{

    public partial class CtlOpenFileDialog : UserControl
    {

        public event EventHandler FileSelected;
        public new event EventHandler TextChanged;

        #region Declarations

        public string Title { get; set; }
        public string DefaultExt { get; set; }
        public bool CheckFileExists { get; set; }
        public bool CheckPathExists { get; set; }
        public bool AddExtension { get; set; }
        
        //CtlOpenFileDialogFilter[] filter;
        string filter;


        #endregion Declarations

        #region Constructors

        public CtlOpenFileDialog()
        {
            InitializeComponent();
            CheckPathExists = true;
            //filter = new CtlOpenFileDialogFilter[0];
        }

        #endregion Constructors

        #region Properties

        public string FileName
        {
            get { return txtFileName.Text; }
            set { txtFileName.Text = value; }
        }

        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        #endregion Properties

        #region Events

        private void CtlOpenFileDialog_Load(object sender, EventArgs e)
        {
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog f = new OpenFileDialog())
                {
                    f.FileName = txtFileName.Text;
                    f.AddExtension = AddExtension;
                    f.CheckFileExists = this.CheckFileExists;
                    f.CheckPathExists = this.CheckPathExists;
                    f.Filter = this.Filter;
                    f.RestoreDirectory = false;
                    f.DefaultExt = DefaultExt;
                    f.Title = Title;
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        txtFileName.Text = f.FileName;
                        if (FileSelected != null)
                            FileSelected(this, EventArgs.Empty);
                    }
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
            string path = txtFileName.Text.Trim();
            if (path.Length > 0 && File.Exists(path))
                System.Diagnostics.Process.Start(path);
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, e);
        }

    }

    /*
    [Serializable]
    public class CtlOpenFileDialogFilter
    {

        public string Extension { get; set; }
        public string Description { get; set; }
        //Text files (*.txt)|*.txt|All files (*.*)|*.*

        public CtlOpenFileDialogFilter()
        {
        }


        //public override string ToString()
        //{
        //    string ext = this.Extension;
        //    if (Conv.HasData(ext))
        //    {
        //        if (ext.StartsWith("."))
        //            ext = ext.Right(ext.Length - 1);

        //    }
        //    //Text files (*.txt)|*.txt|All files (*.*)|*.*
        //    return string.Format("{0} (*.{1})|*.{1}", Description, ext);
        //}

    }
    */


}
