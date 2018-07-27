using System;
using System.ComponentModel;
using System.Windows.Forms;
using Speed.Data.Generation;
using Speed.Windows;
using Speed.Data;

namespace Speed.UI.UserControls
{

    public partial class CtlClassParameters : UserControl
    {

        private GenClassParameters parameters;

        public CtlClassParameters()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GenClassParameters Parameters
        {
            get
            {
                if (!this.DesignMode)
                    ViewToData();
                return parameters;
            }
            set
            {
                parameters = value;
                if (!this.DesignMode)
                    DataToView();
            }
        }

        private void cboNameCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (parameters != null)
                parameters.NameCase = cboNameCase.SelectedItem<EnumNameCase>();
        }

        private void CtlClassParameters_Load(object sender, EventArgs e)
        {
            cboNameCase.Add("(Original Name)", EnumNameCase.None);
            cboNameCase.Add("Camel", EnumNameCase.Camel);
            cboNameCase.Add("Lower", EnumNameCase.Lower);
            cboNameCase.Add("Pascal", EnumNameCase.Pascal);
            cboNameCase.Add("Upper", EnumNameCase.Upper);
            cboNameCase.SelectByValue(EnumNameCase.Pascal);
        }

        public void DataToView()
        {
            chkStartWithSchema.Checked = parameters.StartWithSchema;
            cboNameCase.SelectByValue(parameters.NameCase);
            txtPrefix.Text = Conv.Trim(parameters.Prefix);
            txtRemove.Text = Conv.Trim(parameters.Remove);
        }

        public void ViewToData()
        {
            parameters.StartWithSchema = chkStartWithSchema.Checked;
            parameters.NameCase = cboNameCase.SelectedItem<EnumNameCase>();
            parameters.Prefix = txtPrefix.Text.Trim();
            parameters.Remove = txtRemove.Text.Trim();
        }

        //public void ApplyNames(GenTable table,  bool isBusiness)
        //{
        //    ViewToData();

        //    if (!isBusiness)
        //    {
        //        table.DataClassName = ApplyNames(table.SchemaName, table.TableName);
                
        //        if (!string.IsNullOrEmpty(table.EnumColumnName))
        //            table.EnumName = ApplyNames("EnumDb_", table.TableName);
        //    }
        //    else
        //        table.BusinessClassName = ApplyNames(table.SchemaName, table.TableName);
        //}

        public string ApplyNames(string schemaName, string name, bool addPrefix)
        {
            ViewToData();

            if (!string.IsNullOrEmpty(parameters.Remove))
            {
                string texts = parameters.Remove.Replace(",", ";").Replace(" ", "");
                var values = texts.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in values)
                    name = name.Replace(value, "", StringComparison.InvariantCultureIgnoreCase);
            }

            if (parameters.StartWithSchema && !string.IsNullOrEmpty(schemaName))
                name = schemaName + "_" + name;

            name = Database.GetName(name, parameters.NameCase);

            if (addPrefix && !string.IsNullOrEmpty(parameters.Prefix))
                name = parameters.Prefix + name;

            return name;
        }

    }

}

