using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Speed.Windows.Controls
{

    public partial class CtlHost : UserControl
    {

        public CtlHost()
        {
            InitializeComponent();
        }

        private void CtlHost_Load(object sender, EventArgs e)
        {
        }

        private bool useFile;

        [DefaultValue(false)]
        public bool UseFile
        {
            get { return useFile; }
            set
            {
                useFile = value;
                textbox.Visible = !useFile;
                ctlOpenFile.Visible = useFile;
            }
        }

        public new string Text
        {
            get
            {
                if (!useFile)
                    return textbox.Text;
                else
                    return ctlOpenFile.FileName;
            }
            set
            {
                if (!useFile)
                    textbox.Text = value;
                else
                    ctlOpenFile.FileName = value;
            }
        }

    }

}
