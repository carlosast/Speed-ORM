using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesteGen
{

    public partial class FormTests : Form
    {

        Dictionary<CheckBox, Action> Tests;
        bool isInsert;
        //bool isRunning = false;


        public FormTests()
        {
            InitializeComponent();
        }

        private void FormTests_Load(object sender, EventArgs e)
        {
            Tests = new Dictionary<CheckBox, Action>();
            this.MaximumSize = new System.Drawing.Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            //var methods = typeof(Program).GetMethods(BindingFlags.Public | BindingFlags.Static);

            Tests.Add(chkInsertDapper, () => Program.InsertDapper());
            Tests.Add(chkInsertEntity, () => Program.InsertEntity());
            Tests.Add(chkInsertNHibernate, () => Program.InsertNHibernate());
            Tests.Add(chkInsertProcedure, () => Program.InsertProcedure());
            Tests.Add(chkInsertSpeed, () => Program.InsertSpeed());
            Tests.Add(chkInsertSpeedXml, () => Program.InsertSpeedXml());
            Tests.Add(chkInsertSql, () => Program.InsertSql());

            Tests.Add(chkSelectDapper, () => Program.SelectDapper());
            Tests.Add(chkSelectDataReader, () => Program.SelectDataReader());
            Tests.Add(chkSelectDataTable, () => Program.SelectDataTable());
            Tests.Add(chkSelectEntity, () => Program.SelectEntity());
            Tests.Add(chkSelectNHibernate, () => Program.SelectNHibernate());
            Tests.Add(chkSelectSpeed, () => Program.SelectSpeed());

            chkTestInsert_CheckedChanged(rbtTestInsert, null);
            chk_Click(chkInsertDapper, null);

            numRecords.Value = Program.RecordCount;
            numThreads.Value = Program.ThreadCount;
            SetControls();

        }

        private void chkTestInsert_CheckedChanged(object sender, EventArgs e)
        {
            isInsert = sender == rbtTestInsert;
            gpbTestInsert.Enabled = isInsert;
            gpbTestSelect.Enabled = !isInsert;
            SetControls();
        }

        private void chk_Click(object sender, EventArgs e)
        {
            SetControls();
        }

        private void btnExecSelected_Click(object sender, EventArgs e)
        {
            var actions = GetChekeds();
            Run(actions);
        }

        void Run(List<Action> actions)
        {
            try
            {
                //isRunning = true;
                this.Cursor = Cursors.WaitCursor;
                this.Invalidate();

                int recs = 0;
                if (isInsert)
                {
                    Program.TruncateSales();
                }
                // SELECT
                else
                {
                    recs = Program.GetRecordCount();

                    if (recs != Program.RecordCount)
                    {
                        Program.TruncateSales();

                        int tc = Program.ThreadCount;
                        Program.ThreadCount = 1;
                        txtResults.AppendText("Inserting " + Program.RecordCount + " records\r\n");
                        Program.Timers.Clear();
                        Program.InsertSpeed();
                        Program.ThreadCount = tc;
                        Program.Timers.Clear();
                    }
                }

                Program.Timers = new Results(isInsert ? gpbTestInsert.Text : gpbTestSelect.Text + " : " + recs + " records");

                for (int i = 0; i < actions.Count; i++)
                {
                    var action = actions[i];
                    action();

                    if (actions.Count > 0 && i < actions.Count - 1)
                    {
                        if (isInsert)
                            Program.TruncateSales();
                        Program.Wait(5);
                    }
                }
                txtResults.AppendText(string.Format("Records: {0} - Threads: {1} - Total Records: {2}\r\n", Program.RecordCount, Program.ThreadCount, Program.RecordCount * Program.ThreadCount));
                txtResults.AppendText(Program.Timers.Report());
                txtResults.AppendText("Total records: " + GetRecordCount() + "\r\n\r\n");
                //isRunning = false;
            }
            catch (Exception ex)
            {
                //isRunning = false;
                Program.ShowError(ex);
            }
            this.Invoke(new Action(() => Cursor = Cursors.Default));
        }

        private int GetRecordCount()
        {
            using (var db = Program.NewDatabase())
            {
                string sql =
@"SELECT ddps.row_count as [RowCount]
 FROM sys.objects so
 JOIN sys.indexes si ON si.OBJECT_ID = so.OBJECT_ID
 JOIN sys.dm_db_partition_stats AS ddps ON si.OBJECT_ID = ddps.OBJECT_ID  AND si.index_id = ddps.index_id
 WHERE si.index_id < 2  AND so.is_ms_shipped = 0 and so.name  ='Sales'";
                return db.ExecuteInt32(sql);
            }
        }

        private void numRecords_ValueChanged(object sender, EventArgs e)
        {
            Program.RecordCount = Convert.ToInt32(numRecords.Value);
            SetControls();
        }

        private void numThreads_ValueChanged(object sender, EventArgs e)
        {
            Program.ThreadCount = Convert.ToInt32(numThreads.Value);
            SetControls();
        }

        List<Action> GetChekeds()
        {
            var grp = isInsert ? gpbTestInsert : gpbTestSelect;
            List<Action> ret = new List<Action>();

            List<Control> list = new List<Control>();
            foreach (CheckBox chk in grp.Controls)
                if (chk.Checked)
                    list.Add(chk);

            foreach (CheckBox chk in list)
                ret.Add(Tests[chk]);
            return ret;
        }

        void SetControls()
        {
            var actions = GetChekeds();
            btnExecSelected.Enabled = actions.Count > 0;
            lblTotlaRecords.Text = string.Format("Total records: {0:N0} ", (numRecords.Value * numThreads.Value));
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () => txtResults.Clear());
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this, () =>
                {
                    int start = txtResults.SelectionStart;
                    int end = txtResults.SelectionLength;
                    txtResults.SelectAll();
                    txtResults.Copy();
                    txtResults.SelectionStart = start;
                    txtResults.SelectionLength = end;
                }
            );
        }

    }

}
