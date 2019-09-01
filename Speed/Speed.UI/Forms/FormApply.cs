using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Speed.UI.Forms
{

    public partial class FormApply : Form
    {

        public FormApply()
        {
            InitializeComponent();
        }

        private void FormApply_Load(object sender, EventArgs e)
        {

        }

        private void chkApply_CheckedChanged(object sender, EventArgs e)
        {
            SetControls();
        }

        void SetControls()
        {
            btnApply.Enabled = chkDataClass.Checked | chkBusinessClass.Checked;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {

        }

        public static ApplyOptions GetOptions(IWin32Window owner)
        {
            using (var f = new FormApply())
            {
                if (f.ShowDialog(owner) == DialogResult.OK)
                    return new ApplyOptions { DataClass = f.chkDataClass.Checked, BusinessClass = f.chkBusinessClass.Checked, Enum= f.chkEnum.Checked, Unnamed = f.chkUnnamed.Checked };
                else
                    return null;
            }
        }

        public class ApplyOptions
        {
            public bool DataClass;
            public bool BusinessClass;
            public bool Enum;
            public bool Unnamed;

            public bool ApplyDataClass(string name)
            {
                if (!DataClass)
                    return false;
                if (Unnamed)
                    return string.IsNullOrWhiteSpace(name);
                return true;
            }
            
            public bool ApplyBusinessClass(string name)
            {
                if (!BusinessClass)
                    return false;
                if (Unnamed)
                    return string.IsNullOrWhiteSpace(name);
                return true;
            }
            
            public bool ApplyEnum(string name)
            {
                if (!Enum)
                    return false;
                if (Unnamed)
                    return string.IsNullOrWhiteSpace(name);
                return true;
            }


        }

    }

}
