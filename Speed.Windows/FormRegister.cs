using System;
using System.Windows.Forms;
using Speed.Common;
using Speed.Data;

namespace Speed.Windows
{

    public partial class FormRegister : Form
    {

        EnumDbProviderType providerType;

        public FormRegister()
        {
            InitializeComponent();
            providerType = ProgramBase.ProviderType;
        }
        public FormRegister(EnumDbProviderType providerType)
            : this()
        {
            this.providerType = providerType;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = ProgramBase.Title + " - " + "Configuraçao do banco de dados";
            txtAppName.Text = ProgramBase.AppName;

            using (var reg = new RegUtil(ProgramBase.RegistrySource, ProgramBase.AppName))
            {
                reg.Check();
                var date = reg.Date;
                txtServerName.Text = reg.Server;
                txtDatabase.Text = reg.Database;
                txtUserId.Text = reg.UserId;
                txtPassword.Text = reg.Password;
                txtPort.Text = reg.Port.ToString();
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Test(true);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (Test(false))
                Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public bool Test(bool showMessage)
        {
            Cursor = Cursors.WaitCursor;
            bool ok = false;

            try
            {
                string cs = Database.BuildConnectionString(providerType, txtServerName.Text, txtDatabase.Text, txtUserId.Text, txtPassword.Text, false, Conv.ToInt32(txtPort.Text)).ConnectionString;
                if (cs != null)
                {
                    using (var db = new Database(providerType, cs))
                    {
                        db.Open();
                        db.Close();
                    }

                    using (var reg = new RegUtil(ProgramBase.RegistrySource, ProgramBase.AppName))
                    {
                        reg.Check();
                        reg.Server = txtServerName.Text;
                        reg.Database = txtDatabase.Text;
                        reg.UserId = txtUserId.Text;
                        reg.Password = txtPassword.Text;
                        reg.Ok = 1;
                    }

                    ok = true;

                    ProgramBase.ConnectionString = cs;
                }

                if (showMessage)
                    ProgramBase.ShowInformation("Base de dados configurada com sucesso para '" + txtAppName.Text + "'");
            }
            catch (Exception ex)
            {
                ProgramBase.ShowError(ex);
                ok = false;
            }
            Cursor = Cursors.Default;
            return ok;
        }

    }

}
