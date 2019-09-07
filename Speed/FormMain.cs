using System;
using System.Drawing;
using System.Windows.Forms;
using Speed.UI.UserControls;
using System.IO;
using System.Diagnostics;
using Speed.Common;

namespace Speed
{

    public partial class FormMain : Form
    {

        static int count = 0;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.Text = Program.Title;
                this.Icon = Properties.Resources.APP;
                imlIcons.Images.Add("Gear", Speed.UI.Properties.Resources.Gear);
            }
#if DEBUG2
            if (count == 0 && Environment.MachineName == "QUASAR")
            {
                string fileName = null;
                // fileName = @"..\..\..\__Testes\MyProject\MyProject.spd";
                // fileName = @"..\..\..\Testes\TestGen.Oracle\TestOracle.spd";
                // fileName = @"..\..\..\Testes\TestGen.MySql\TestMySql.spd";
                fileName = @"E:\_Projects\_Systems\SpeedCluster\SpeedCluster.spd";

                if (fileName != null)
                {
                    FileInfo fi = new FileInfo(fileName);
                    if (fi.Exists)
                    {
                        var gen = new Speed.UI.UserControls.CtlGenerator(fi.FullName);
                        AddChildControl(gen, fi.Name, "Gear", fi.FullName);
                        //AddChildControl<Speed.UI.UserControls.CtlGenerator>("Generator", "Gear");
                    }
                }
            }
#endif
            btnClose.Tag = btnClose.ForeColor;
            btnClose.MouseEnter += (o, ev) => btnClose.ForeColor = Color.Black;
            btnClose.MouseLeave += (o, ev) => btnClose.ForeColor = (Color)btnClose.Tag;

            mnuFile_Click(null, null);

            // abre o último arquivo
#if DEBUG2
            if (mnuFileRecenteFiles.DropDownItems.Count > 0)
                mnuFileRecenteFilesItem_Click(mnuFileRecenteFiles.DropDownItems[0], null);
#endif
            SetControls();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            TabPage page = tabControl.SelectedTab as TabPage;
            if (page != null)
                Program.RunSafe(this, () => tabControl.TabPages.Remove(page));
            SetControls();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Program.RunSafe(this, () =>
            {
                switch (e.ClickedItem.Name)
                {
                    case "tsbNewBrowser":
                        FileNew();
                        break;

                    case "tsbOpen":
                        FileOpen();
                        break;

                    case "tsbSave":
                        FileSave();
                        break;
                }
            });
        }

        private void tbCopyDlls_Click(object sender, EventArgs e)
        {
            CopyDlls();
        }

#region mnuFile

        private void mnuFile_Click(object sender, EventArgs e)
        {
            mnuFileRecenteFiles.DropDownItems.Clear();

            foreach (var file in Program.Config.RecentFiles)
            {
                var item = mnuFileRecenteFiles.DropDownItems.Add(file);
                item.Click += mnuFileRecenteFilesItem_Click;
            }
        }

        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () => FileNew());
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () => FileOpen());
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () => FileSave());
        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () => FileSaveAs());
        }

        private void mnuFileRecenteFilesItem_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () => FileOpen(((ToolStripMenuItem)sender).Text));
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () => Application.Exit());
        }

#endregion mnuFile

#region mnuHelp

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () =>
            {
                using (var f = new FormAboutBox())
                    f.ShowDialog();
            });
        }

        private void mnuHelpHelp_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () => Process.Start("http://speed.codeplex.com/documentation"));
        }

#endregion mnuHelp

#region Methods

        public DialogResult AddChildModal<T>() where T : Form, new()
        {
            using (T f = new T())
                return f.ShowDialog();
        }
        public void AddChildControl<T>(string text = null, string imageKey = null) where T : UserControl, new()
        {
            Program.RunSafe(this, () =>
            {
                T ctrl = new T();
                AddChildControl(ctrl, text, imageKey);
            });
            SetControls();
        }

        public void AddChildControl(Control ctrl, string text = null, string imageKey = null, string toolTipText = null)
        {
            count++;
            Program.RunSafe(this, () =>
            {
                //f.Dock = DockStyle.Fill;
                ctrl.Dock = DockStyle.Fill;
                TabPage page = new TabPage();
                ctrl.Tag = page;
                page.Tag = ctrl;
                page.Text = text != null ? text : ctrl.Text;
                page.ToolTipText = toolTipText ?? "";
                page.Controls.Add(ctrl);

                ctrl.TextChanged += (o, ev) =>
                {
                    UserControl _f = (UserControl)o;
                    TabPage _page = (TabPage)_f.Tag;
                    _page.Text = _f.Text;
                };

                if (imageKey != null)
                    page.ImageKey = imageKey;

                //page.SizeChanged += (o, ev) =>
                //{
                //    TabPage _page = (TabPage)o;
                //    Form _f = (Form)_page.Tag;
                //    _f.SetBounds(0, 0, _page.ClientRectangle.Width, _page.ClientRectangle.Height);
                //    _f.Width=  _page.ClientRectangle.Width;
                //    _f.Height = _page.ClientRectangle.Height;
                //    User32.SetParent(_f.Handle, _page.Handle);
                //    f.Invalidate();
                //};

                page.Padding = new Padding(3);
                tabControl.Controls.Add(page);
                //f.Visible = true;
                tabControl.SelectedTab = page;
            });
            SetControls();
        }

        void SetControls()
        {
            btnClose.Visible = tabControl.Visible = tabControl.TabPages.Count > 0;
        }

        public void FileNew()
        {
            AddChildControl<Speed.UI.UserControls.CtlGenerator>("Generator", "Gear");
        }

        public void FileOpen()
        {
            using (var f = new OpenFileDialog())
            {
                f.CheckFileExists = true;
                f.CheckPathExists = true;
                f.AddExtension = true;
                f.DefaultExt = ".spd";
                f.Filter = "Speed files (*.spd)|*.spd|All files (*.*)|*.*";
                f.FilterIndex = 0;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    var gen = new Speed.UI.UserControls.CtlGenerator(f.FileName);
                    AddChildControl(gen, Path.GetFileName(f.FileName), "Gear", f.FileName);
                    Program.Config.SetRecentFile(f.FileName);
                }
            }
        }

        public void FileOpen(string fileName)
        {
            Program.RunSafe(this, () =>
            {
                if (!File.Exists(fileName))
                    throw new Exception("File not found: " + fileName);

                foreach (TabPage page in tabControl.TabPages)
                {
                    if (page.ToolTipText.EqualsICIC(fileName))
                    {
                        tabControl.SelectedTab = page;
                        Program.Config.SetRecentFile(fileName);
                        return;
                    }
                }

                var gen = new Speed.UI.UserControls.CtlGenerator(fileName);
                AddChildControl(gen, Path.GetFileName(fileName), "Gear", fileName);
                Program.Config.SetRecentFile(fileName);
            });
        }

        public void FileSave()
        {
            var ctrl = tabControl.SelectedTab.Tag;
            var browser = ctrl as CtlGenerator;

            if (browser != null)
            {
                if (browser == null || string.IsNullOrEmpty(browser.FileName))
                {
                    using (var f = new SaveFileDialog())
                    {
                        f.CheckFileExists = false;
                        f.CheckPathExists = true;
                        f.AddExtension = true;
                        f.DefaultExt = ".spd";
                        f.Filter = "Speed files (*.spd)|*.spd|All files (*.*)|*.*";
                        f.FilterIndex = 0;
                        if (f.ShowDialog() == DialogResult.OK)
                            browser.Save(f.FileName);
                        Program.Config.SetRecentFile(f.FileName);
                    }
                }
                else
                    browser.Save();
            }
        }

        public void FileSaveAs()
        {
            var ctrl = tabControl.SelectedTab.Tag;
            var browser = ctrl as CtlGenerator;

            if (browser != null)
            {
                using (var f = new SaveFileDialog())
                {
                    f.CheckFileExists = false;
                    f.CheckPathExists = true;
                    f.AddExtension = true;
                    f.DefaultExt = ".spd";
                    f.Filter = "Speed files (*.spd)|*.spd|All files (*.*)|*.*";
                    if (!string.IsNullOrEmpty(browser.FileName))
                    {
                        FileInfo fi = new FileInfo(browser.FileName);
                        f.InitialDirectory = fi.DirectoryName;
                        f.FileName = fi.Name;
                    }
                    f.FilterIndex = 0;
                    if (f.ShowDialog() == DialogResult.OK)
                        browser.Save(f.FileName);
                }
            }
        }

        void CopyDlls()
        {

            string _files =
@"
ICSharpCode.SharpZipLib.dll
Ionic.Zip.dll
Mono.Security.dll
MySql.Data.dll
Npgsql.dll
Npgsql.pdb
Npgsql.xml
Oracle.ManagedDataAccess.dll
policy.2.0.Npgsql.config
policy.2.0.Npgsql.dll
Speed.Common.dll
Speed.Data.dll
Speed.Windows.dll
System.Data.SQLite.dll
System.Data.SQLite.Linq.dll
System.Data.SqlServerCe.dll
Microsoft.SqlServer.Types.dll
";

            Program.RunSafe(this, () =>
                {
                    using (FolderBrowserDialog f = new FolderBrowserDialog())
                    {
                        f.ShowNewFolderButton = true;
                        if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            string[] files = _files.Trim().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                            string dir = f.SelectedPath;
                            foreach (var file in files)
                                CopyFile(file, dir);

                            dir = Path.Combine(Program.AppDirectory, "x86");
                            foreach (var file in Directory.GetFiles(dir, "*.*"))
                                CopyFile(file, Path.Combine(f.SelectedPath, "x86"));

                            dir = Path.Combine(Program.AppDirectory, "x64");
                            foreach (var file in Directory.GetFiles(dir, "*.*"))
                                CopyFile(file, Path.Combine(f.SelectedPath, "x64"));

                            Program.ShowInformation("Files copied to the directory '" + f.SelectedPath);
                        }
                    }
                });
        }

        void CopyFile(string fullName, string dirTarget)
        {
            try
            {
                string target = Path.Combine(dirTarget, Path.GetFileName(fullName));
                if (!Directory.Exists(dirTarget))
                    Directory.CreateDirectory(dirTarget);

                if (File.Exists(target))
                    File.SetAttributes(target, FileAttributes.Normal);
                File.Copy(fullName, target, true);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

#endregion Methods

    }

}
