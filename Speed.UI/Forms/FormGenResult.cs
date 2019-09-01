using Speed.Data.Generation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Speed.Windows;
using Speed.Data;

namespace Speed.UI.Forms
{

    public partial class FormGenResult : Form
    {

        public FormGenResult()
        {
            InitializeComponent();
        }

        private void FormGenResult_Load(object sender, EventArgs e)
        {
            btnOk.CenterInParent();
        }

        internal class Report
        {
            public EnumFileChanged FileChanged { get; set; }
            public string FileName { get; set; }
            
            public Report(string fileName, EnumFileChanged fileChanged)
            {
                this.FileName = fileName;
                this.FileChanged = fileChanged;
            }
        }

        public static void ShowDialog(Dictionary<string, GenTableResult> result)
        {
            using (var f = new FormGenResult())
            {
                List<Report> list = new List<Report>();
                foreach (var pair in result)
                {
                    var tb = pair.Value;
                    list.Add(new Report(tb.EntityFileName, tb.EntityFileNameChanged));
                    list.Add(new Report(tb.EntityFileNameExt, tb.EntityFileNameExtChanged));
                    list.Add(new Report(tb.BusinnesFileName, tb.BusinnesFileNameChanged));
                    list.Add(new Report(tb.BusinnesFileNameExt, tb.BusinnesFileNameExtChanged));

                    if (!string.IsNullOrEmpty(tb.EnumFileName))
                        list.Add(new Report(tb.EnumFileName, tb.EnumFileNameChanged));
                }

                list = list.OrderBy(p => -(int)p.FileChanged).ThenBy(p => p.FileName).ToList();

                f.lblResult.Text = string.Format("Unchanged: {0} - New: {1} - Changed: {2}",
                    list.Count(p => p.FileChanged == EnumFileChanged.Unmodified),
                    list.Count(p => p.FileChanged == EnumFileChanged.New),
                    list.Count(p => p.FileChanged == EnumFileChanged.Modified));
                
                f.grid.AutoGenerateColumns = false;
                f.grid.DataSource = list;
                f.ShowDialog();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                var report = row.DataBoundItem as Report;
                if (report != null)
                {
                    if (report.FileChanged == EnumFileChanged.New)
                        row.DefaultCellStyle.ForeColor = Color.Green;
                    else if (report.FileChanged == EnumFileChanged.Modified)
                        row.DefaultCellStyle.ForeColor = Color.Red;
                    else // if (report.FileChanged == EnumFileChanged.Unchanged)
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
        }

    }

}
