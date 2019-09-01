using System;
using System.Windows.Forms;
using Speed.Data;

namespace Speed.Windows
{

    public partial class FormConnect : Form
    {

        private const string DocTotal_REG = "Speed\\";
        EnumDbProviderType providerType;
        public string ConnectionString { get { return ctlDbConnect1.ConnectionString; } }
        public ConnectionInfo Connection { get { return ctlDbConnect1.Connection; } set { ctlDbConnect1.Connection = value; } }

        public FormConnect()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
                this.Text = ProgramBase.Title + " - " + "Base de dados";
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            ctlDbConnect1.Connect(true);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ctlDbConnect1.Connect(false))
                Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }

}
