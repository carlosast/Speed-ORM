using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Speed.Data;
using Speed.Data.Generation;

namespace Speed.Windows
{

    public partial class FormGenerate : Form
    {

        Database db;
        string nameSpaceEntity;
        string nameSpaceBL;
        string entityDirectory;
        string entityDirectoryExt;
        string blDirectory;
        string blDirectoryExt;
        GenTableCollection alias;

        public FormGenerate()
        {
            InitializeComponent();
        }

        private void FormGenerate_Load(object sender, EventArgs e)
        {

        }

        private void lblSelecteAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTables.Items.Count; i++)
                clbTables.SetItemChecked(i, true);
        }

        private void lblDeSelecteAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTables.Items.Count; i++)
                clbTables.SetItemChecked(i, false);
        }

        public static void Generate(Database db, string nameSpaceEntity, string nameSpaceBL, string entityDirectory, string entityDirectoryExt, string blDirectory, string blDirectoryExt, GenTableCollection alias)
        {
            using (var f = new FormGenerate())
            {
                f.db = db;
                f.nameSpaceEntity = nameSpaceEntity;
                f.nameSpaceBL = nameSpaceBL;
                f.entityDirectory = entityDirectory;
                f.entityDirectoryExt = entityDirectoryExt;
                f.blDirectory = blDirectory;
                f.blDirectoryExt = blDirectoryExt;
                f.alias = alias;

                var tables = db.Provider.GetAllTables();

                int index = 0;
                foreach (var table in tables)
                {
                    if (table.TableName.ToLower() != "sysdiagrams")
                    {
                        f.clbTables.Items.Add(table);
                        if (alias.Count(p => p.TableName.ToLower() == table.TableName.ToLower()) > 0)
                            f.clbTables.SetItemChecked(index, true);
                    }
                    index++;
                }

                f.ShowDialog();

            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            ProgramBase.RunSafe(this, () =>
            {
                //parei aqui
                //ter que ter 3 colunas (
                //TableName;
                //BusinessClassName;
                //DataClassName;

            });
        }

    }

}
