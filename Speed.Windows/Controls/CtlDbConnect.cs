using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Speed.Data;
using System.Diagnostics;
using System.Data.Common;
using Speed.Common;

namespace Speed.Windows.Controls
{

    public partial class CtlDbConnect : UserControl
    {

        #region DbControl

        /// <summary>
        /// Helper controls class
        /// </summary>
        internal class DbControl
        {
            public Label Label;
            public Control Control;

            private bool visible;
            // TabControl faz  Control.Visible=false. Então é necessário manter uma variável para guardar o último valor de Visible
            public bool Visible
            {
                get { return visible; }
                set
                {
                    Control.Visible = visible = value;
                }
            }

            public DbControl(Label label, Control control)
            {
                this.Label = label;
                this.Control = control;
            }

            public string Name
            {
                get
                {
                    return Control.Name;
                }
            }

            public string Text
            {
                get
                {
                    string text = null;
                    if (Control is TextBox)
                        text = visible ? ((TextBox)Control).Text.Trim() : null;
                    else if (Control is CtlHost)
                        text = visible ? ((CtlHost)Control).Text.Trim() : null;
                    return string.IsNullOrEmpty(text) ? null : text;
                }
            }

            public string Password
            {
                get
                {
                    TextBox ctrl = (TextBox)Control;
                    string text = visible ? ctrl.Text.Trim() : null;
                    return visible ? new string('*', text.Length) : null;
                }
            }

            public bool Boolean
            {
                get
                {
                    return visible ? ((CheckBox)Control).Checked : false;
                }
            }

            public int Integer
            {
                get
                {
                    return visible ? Convert.ToInt32(((NumericUpDown)Control).Value) : 0;
                }
            }

            public override string ToString()
            {
                return string.Format("{0} - {1} - {2}", Control.Name, visible, Control.Visible);
            }

        }

        #endregion DbControl

        #region Declarations

        EnumDbProviderType providerType;
        public string ConnectionString;
        private ConnectionInfo ci;
        public event EventHandler ConnectionChanged;
        public bool IsConnected { get; private set; }
        List<DbControl> controls;

        Dictionary<EnumDbProviderType, string> hints;

        #endregion Declarations

        #region Constructors

        public CtlDbConnect()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ConnectionInfo Connection
        {
            get
            {
                if (!this.DesignMode)
                    ViewToData();
                return ci;
            }
            set
            {
                ci = value;
                if (!this.DesignMode)
                    DataToView();
            }
        }

        #endregion Properties

        #region Events

        protected void OnConnectionChanged()
        {
            if (ConnectionChanged != null)
                ConnectionChanged(this, EventArgs.Empty);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {

                controls = new List<DbControl>();
                controls.Add(new DbControl(lblEmbedded, chkEmbedded));
                controls.Add(new DbControl(lblHost, txtHost));
                controls.Add(new DbControl(lblDatabase, txtDatabase));
                controls.Add(new DbControl(lblIntegSec, chkIntegSec));
                controls.Add(new DbControl(lblUserId, txtUserId));
                controls.Add(new DbControl(lblPassword, txtPassword));
                controls.Add(new DbControl(lblPort, numPort));

                hints = new Dictionary<EnumDbProviderType, string>();

                hints.Add(EnumDbProviderType.Access,
"\"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=FILENAME.accdb\r\n\r\n" +
"\"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=FILENAME.mdb");

                hints.Add(EnumDbProviderType.Firebird,
@"Initial Catalog=E:\MyDb.fdb;user id=SYSDBA;password=masterkey;pooling=True" +
@"Initial Catalog=E:\TEST.FB;user id=SYSDBA;password=masterkey;pooling=True;server type=Embedded");

                hints.Add(EnumDbProviderType.SqlServer,
"Data Source=localhost;Initial Catalog=MyDatabasename;Integrated Security=True;MultipleActiveResultSets=True;\r\n\r\n" +
"Data Source=localhost;Initial Catalog=MyDatabasename;User ID=MyUserName;Password=MyPassword;MultipleActiveResultSets=True;");

                hints.Add(EnumDbProviderType.PostgreSQL,
"HOST=localhost;DATABASE=Speed;USER ID=MyUserName;PASSWORD=MyPassword;\r\n\r\n" +
"Data Source=localhost;Initial Catalog=MyDatabasename;User ID=MyUserName;Password=MyPassword;MultipleActiveResultSets=True;");

                hints.Add(EnumDbProviderType.SQLite,
"Data Source=FILENAME;Pooling=true;FailIfMissing=false");

                hints.Add(EnumDbProviderType.Oracle,
"Data Source=orcl;User ID=MyUserName;Password=MyPassword");

                hints.Add(EnumDbProviderType.MySql,
"server=localhost;database=MyDatabasename;User Id=root;password=MyPassword;\r\n\r\n" +
"server=localhost;database=MyDatabasename;Integrated Security=True");

                hints.Add(EnumDbProviderType.MariaDB,
"server=localhost;database=MyDatabasename;User Id=root;password=MyPassword;\r\n\r\n" +
"server=localhost;database=MyDatabasename;Integrated Security=True");


                cboProviderType.Items.Add(new ItemText<EnumDbProviderType>("Access", EnumDbProviderType.Access));
                cboProviderType.Items.Add(new ItemText<EnumDbProviderType>("Firebird", EnumDbProviderType.Firebird));
                cboProviderType.Items.Add(new ItemText<EnumDbProviderType>("MariaDB", EnumDbProviderType.MariaDB));
                cboProviderType.Items.Add(new ItemText<EnumDbProviderType>("MySql", EnumDbProviderType.MySql));
                cboProviderType.Items.Add(new ItemText<EnumDbProviderType>("Oracle", EnumDbProviderType.Oracle));
                cboProviderType.Items.Add(new ItemText<EnumDbProviderType>("PostgreSQL", EnumDbProviderType.PostgreSQL));
                cboProviderType.Items.Add(new ItemText<EnumDbProviderType>("Sql Server", EnumDbProviderType.SqlServer));
                cboProviderType.Items.Add(new ItemText<EnumDbProviderType>("SQLite", EnumDbProviderType.SQLite));

                providerType = EnumDbProviderType.SqlServer;
                cboProviderType.SelectByValue<EnumDbProviderType>(providerType);
                grbEnterCs.Location = grbBuildCs.Location;
                grbEnterCs.Width = grbBuildCs.Width;

#if DEBUG2
                Connection = new ConnectionInfo(EnumDbProviderType.SqlServer, ".", "Testes", "sa", "manager", 0, 30);
#endif
                SetControls();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cboProviderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cboProviderType.SelectedItem as ItemText<EnumDbProviderType>;
            if (item != null)
                providerType = (EnumDbProviderType)item.Value;

            if (hints.ContainsKey(providerType))
            {

                lblHints.Text = "Samples:\r\n\r\n" + hints[providerType];
            }
            else
                lblHints.Text = "";

            switch (providerType)
            {
                case EnumDbProviderType.Access:
                    SetVisible(chkEmbedded, false, txtHost, txtDatabase, chkIntegSec, false, txtUserId, txtPassword, numPort, false);
                    txtDatabase.UseFile = true;
                    break;

                case EnumDbProviderType.Firebird:
                    SetVisible(chkEmbedded, txtHost, false, txtDatabase, chkIntegSec, false, txtUserId, txtPassword, numPort, false);

                    SetVisible(txtDatabase, true);
                    txtDatabase.UseFile = true;

                    //if (chkEmbedded.Checked)
                    //{
                    //    SetVisible(txtDatabase, false);
                    //    txtDatabase.UseFile = true;
                    //}
                    //else
                    //    txtDatabase.UseFile = false;

                    break;

                case EnumDbProviderType.MariaDB:
                    SetVisible(chkEmbedded, false, txtHost, txtDatabase, chkIntegSec, txtUserId, txtPassword, numPort);
                    txtDatabase.UseFile = false;
                    break;

                case EnumDbProviderType.MySql:
                    SetVisible(chkEmbedded, false, txtHost, txtDatabase, chkIntegSec, txtUserId, txtPassword, numPort);
                    txtDatabase.UseFile = false;
                    break;

                case EnumDbProviderType.Oracle:
                    SetVisible(chkEmbedded, false, txtHost, txtDatabase, false, chkIntegSec, txtUserId, txtPassword, numPort);
                    txtDatabase.UseFile = false;
                    break;

                case EnumDbProviderType.PostgreSQL:
                    SetVisible(chkEmbedded, false, txtHost, txtDatabase, chkIntegSec, txtUserId, txtPassword, numPort);
                    txtDatabase.UseFile = false;
                    break;

                case EnumDbProviderType.SQLite:
                    SetVisible(chkEmbedded, false, txtHost, false, txtDatabase, chkIntegSec, false, txtUserId, false, txtPassword, numPort, false);
                    txtDatabase.UseFile = true;
                    break;

                case EnumDbProviderType.SqlServer:
                    SetVisible(chkEmbedded, false, txtHost, txtDatabase, chkIntegSec, txtUserId, txtPassword, numPort);
                    txtDatabase.UseFile = false;
                    break;

                case EnumDbProviderType.SqlServerCe:
                    SetVisible(chkEmbedded, false, txtHost, false, txtDatabase, chkIntegSec, false, txtUserId, false, txtPassword, numPort, false);
                    txtDatabase.UseFile = true;
                    break;
            }

            lblPortDefault.Visible = numPort.Visible;

            Draw();
            SetControls();
        }

        private void rbtBuildCs_Click(object sender, EventArgs e)
        {
            SetControls();
        }

        private void chkIntegSec_CheckedChanged(object sender, EventArgs e)
        {
            Draw();
            SetControls();
        }

        private void chkEmbedded_CheckedChanged(object sender, EventArgs e)
        {
            Draw();
            SetControls();
        }

        private void lblConenctionString_Click(object sender, EventArgs e)
        {
            ProgramBase.RunSafe(this, () => Clipboard.SetText(lblConenctionString.Text));
        }

        private void picHelp_Click(object sender, EventArgs e)
        {
            ProgramBase.RunSafe(this, () => Process.Start("http://www.connectionstrings.com/"));
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            SetControls();
        }

        #endregion Events

        #region Methods

        public bool Connect(bool showMessage)
        {
            Cursor = Cursors.WaitCursor;
            IsConnected = false;

            try
            {
                string cs;
                if (IsBuild())
                {
                    var ctrls = controls.ToDictionary(p => p.Control);
                    DbConnectionStringBuilder csb = Database.BuildConnectionString(providerType, ctrls[txtHost].Text, ctrls[txtDatabase].Text, ctrls[txtUserId].Text, ctrls[txtPassword].Text, ctrls[chkIntegSec].Boolean, ctrls[numPort].Integer, ctrls[chkEmbedded].Boolean);
                    cs = csb.ConnectionString;
                }
                else
                    cs = txtConnectionString.Text.Trim();

                if (!string.IsNullOrEmpty(cs))
                {
                    using (var db = new Database(providerType, cs))
                    {
                        db.Open();
                        db.Close();
                    }
                    ViewToData();
                    IsConnected = true;
                }
                else
                    throw new Exception("Invalid connection string");

                if (showMessage)
                    ProgramBase.ShowInformation("Database set up successfully: '" + txtDatabase.Text + "'");
            }
            catch (Exception ex)
            {
                //if (showMessage)
                    ProgramBase.ShowError(ex);
                IsConnected = false;
            }
            Cursor = Cursors.Default;
            Draw();
            SetControls();
            return IsConnected;
        }

        void Draw()
        {
            grbBuildCs.SuspendLayout();
            int top = lblConenctionString.Bottom + 5;
            foreach (var ctrl in controls)
            {
                if (ctrl.Visible)
                {
                    ctrl.Label.Top = ctrl.Control.Top = top;
                    top += ctrl.Control.Height + 5;
                }
            }
            lblPortDefault.Top = numPort.Top;
            grbBuildCs.ResumeLayout();
        }

        void setVisible(DbControl control, bool visible)
        {
            control.Visible = visible;
            grbBuildCs.Controls["lbl" + control.Name.Right(control.Name.Length - 3)].Visible = visible;
        }
        void SetVisible(params object[] controls)
        {
            var ctrls = this.controls.ToDictionary(p => p.Control);
            for (int i = 0; i < controls.Length; i++)
            {
                if (i < controls.Length - 1 && controls[i + 1] is bool)
                {
                    setVisible(ctrls[(Control)controls[i]], (bool)controls[i + 1]);
                    i++; // skip 1
                }
                else
                    setVisible(ctrls[(Control)controls[i]], true); // default = true
            }
        }

        public void DataToView()
        {
            if (ci != null)
            {
                cboProviderType.SelectByValue<EnumDbProviderType>(ci.Provider);

                if (ci.BuildConnectionString)
                    rbtBuildCs.Checked = true;
                else
                    rbtEnterCs.Checked = true;

                txtHost.Text = ci.Server;

                // compatibility
                if (providerType == EnumDbProviderType.SQLite)
                {
                    if (string.IsNullOrWhiteSpace(ci.Database) && !string.IsNullOrWhiteSpace(ci.Server))
                    {
                        txtDatabase.UseFile = true;
                        ci.Database = ci.Server;
                        ci.Server = null;
                    }
                }
                txtDatabase.Text = ci.Database;
                txtUserId.Text = ci.UserId;
                chkIntegSec.Checked = ci.IntegratedSecurity;
                txtPassword.Text = ci.Password;
                numPort.Value = ci.Port;
                chkEmbedded.Checked = ci.Embedded;
                //txtProviderName.Text = ci.ProviderName;
                txtConnectionString.Text = ci.ConnectionString;
            }
        }

        public void ViewToData()
        {
            ci = new ConnectionInfo();
            ci.Provider = this.providerType;
            ci.Server = txtHost.Text.Trim();
            ci.Database = txtDatabase.Text.Trim();
            ci.UserId = txtUserId.Text.Trim();
            ci.Password = txtPassword.Text.Trim();
            ci.IntegratedSecurity = chkIntegSec.Checked;
            ci.Port = Conv.ToInt32(numPort.Value);
            // ci.ProviderName = this.txtProviderName.Text;
            ci.Embedded = chkEmbedded.Checked;
            ci.ConnectionString = txtConnectionString.Text.Trim();
            ci.BuildConnectionString = rbtBuildCs.Checked;
        }

        public Database GetDb()
        {
            Database db;
            if (string.IsNullOrWhiteSpace(Connection.ConnectionString))
                db = new Database(Connection);
            else
                db = new Database(Connection.Provider, Connection.ConnectionString);

            db.Open();
            return db;
        }

        bool IsBuild()
        {
            return rbtBuildCs.Checked;
        }

        void SetControls()
        {
            grbBuildCs.Visible = IsBuild();
            grbEnterCs.Visible = !IsBuild();
            txtUserId.Enabled = txtPassword.Enabled = !chkIntegSec.Checked;

            if (IsBuild())
            {
                try
                {
                    var ctrls = controls.ToDictionary(p => p.Control);
                    DbConnectionStringBuilder csb = Database.BuildConnectionString(providerType, ctrls[txtHost].Text, ctrls[txtDatabase].Text, ctrls[txtUserId].Text, ctrls[txtPassword].Password, ctrls[chkIntegSec].Boolean, ctrls[numPort].Integer, ctrls[chkEmbedded].Boolean);
                    lblConenctionString.Text = Database.BuildConnectionString(providerType, txtHost.Text, txtDatabase.Text, txtUserId.Text, new string('*', txtPassword.Text.Length), chkIntegSec.Checked, Conv.ToInt32(numPort.Value)).ConnectionString;
                    lblConenctionString.ForeColor = Color.Black;
                }
                catch (Exception ex)
                {
                    lblConenctionString.Text = "Error: " + ex.Message;
                    lblConenctionString.ForeColor = Color.Red;
                }
            }
            grbEnterCs.Location = grbBuildCs.Location;
            grbEnterCs.Width = grbBuildCs.Width;

            var name1 = grbEnterCs.Parent.Name;
            var name2 = grbBuildCs.Parent.Name;
        }

        #endregion Methods

    }

}
