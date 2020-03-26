using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Speed.Data;
using Speed.Data.Generation;
using Speed.Windows;
using Speed.Data.MetaData;

namespace Speed.UI.UserControls
{

    public partial class CtlGenerator : UserControl
    {

        public string FileName;
        SpeedFile file;

        public CtlGenerator()
        {
            InitializeComponent();
        }

        public CtlGenerator(string fileName)
            : this()
        {
            this.FileName = fileName;
        }

        private void CtlGenerator_Load(object sender, EventArgs e)
        {
            //this.FindForm().Icon = Speed.UI.Properties.Resources.Gear;
            this.SuspendLayout();
            // pra carregar os controles dos tabs
            for (int i = tabControl1.TabCount - 1; i >= 0; i--)
                tabControl1.SelectedIndex = i;

            if (string.IsNullOrEmpty(FileName))
                file = new SpeedFile();
            else
            {
                file = SpeedFile.Load(FileName);
            }

            //btnConnect.CenterInParent();
            panButtons.CenterInParent();

            if (!string.IsNullOrEmpty(this.FileName))
            {
                try
                {
                    DataToView();
                    
                    // inicialmente desabilita tudo
                    //file.Parameters.Tables.ForEach(p => p.IsSelected = false);
                    //file.Parameters.Views.ForEach(p => p.IsSelected = false);

                    Connect(true, false, true);
                    
                    // se já estiver preenchida a aba de dados, seleciona a aba de tables
                    if (!string.IsNullOrWhiteSpace(file.Parameters.DataClass.Directory) && !string.IsNullOrWhiteSpace(file.Parameters.BusinessClass.Directory))
                    {
                        tabControl1.SelectedIndex = 1;
                    }

                    // dbConnect.Connect(false);
                }
                catch
                {
                }
            }
            else
                DataToView();


            SetControls();

            foreach (TabPage page in tabControl1.TabPages)
                page.AutoScroll = true;

            this.ResumeLayout();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect(true, false, false);
        }

        private void Connect(bool loadData, bool showMessage, bool selectFirst)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (dbConnect.Connect(showMessage))
                {
                    using (var db = dbConnect.GetDb())
                    {
                        if (loadData)
                        {
                            CheckObjects(db);
                            browser.Fill(db, dbConnect.Connection, selectFirst);
                        }
                    }
                    file.Connection = dbConnect.Connection;
                    DataToView();
                }
            }
            catch (Exception ex)
            {
                if (showMessage)
                    Program.ShowError(ex);
            }
            SetControls();
            this.Cursor = Cursors.Default;
        }

        public Database GetDb()
        {
            return dbConnect.GetDb();
        }


        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Program.RunSafe(this.FindForm(), () =>
            {
                browser.ApplyNames();
                ViewtoData();
                Connect(false, false, false);
                if (dbConnect.IsConnected && Validate())
                {
                    using (var db = dbConnect.GetDb())
                    {

                        //TODO: ao mudar namecase no combo, setar a nulo os nomes de tabelas
                        //TODO: tratar colunas calculadas. Gerar só o GET, e não usar nos insert/update

                        ViewtoData();

                        if (file.Parameters.Tables.Where(p => p.IsSelected).Count() == 0 && file.Parameters.Views.Where(p => p.IsSelected).Count() == 0)
                            throw new Exception("No objects selected");
                        Dictionary<string, GenTableResult> result = db.Generate(file.Parameters);

#if DEBUG2
                        var file = new SpeedFile();
                        file.Parameters = pars;
                        file.Connection = dbConnect.Connection;
                        file.Save("./Pars.xml");
#endif

                        Speed.UI.Forms.FormGenResult.ShowDialog(result);
                        //Program.ShowInformation("Classes geradas com sucesso.");
                    }
                }
            });
            SetControls();
        }

        public new bool Validate()
        {
            //var tables = browser.GetCheckedTables();

            //if (tables.Count == 0)
            //    throw new Exception("Não existe nenhuma tabela selecionada.");

            return parameters.Validate();
        }

        void SetControls()
        {
            btnGenerate.Enabled = tabBrowser.Enabled = dbConnect.IsConnected;
            lblNotConnected.Visible = !dbConnect.IsConnected;
        }

        void ViewtoData()
        {
            dbConnect.ViewToData();
            file.Connection = dbConnect.Connection;
            parameters.ViewToData();
            browser.ViewToData();
        }

        void DataToView()
        {
            dbConnect.Connection = file.Connection;
            parameters.File = file;
            browser.File = file;
        }

        public void Save(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = this.FileName;

            ViewtoData();
            file.Save(fileName);

            this.FileName = fileName;
        }

        public void CheckObjects(Database db)
        {
            //// arquivo
            //Dictionary<string, GenTable> tables = new Dictionary<string, GenTable>();
            //file.Parameters.Tables.ForEach(p => tables.Add(p.FullName, p));
            //file.Parameters.Views.ForEach(p => tables.Add(p.FullName, p));

            //foreach (var dbTb in dbTbs)
            //{
            //    if (!tables.ContainsKey(dbTb.FullName))
            //        tables.Remove(dbTb.FullName);
            //}


            // Tables
            var dbTbs = db.Provider.GetAllTables(null, EnumTableType.Table).ToDictionary(p => p.FullName);
            foreach (var table in file.Parameters.Tables.ToList())
            {
                if (!dbTbs.ContainsKey(table.FullName))
                    file.Parameters.Tables.Remove(table);
            }

            // Views
            var dbVws = db.Provider.GetAllTables(null, EnumTableType.View).ToDictionary(p => p.FullName);
            foreach (var view in file.Parameters.Views.ToList())
            {
                if (!dbVws.ContainsKey(view.FullName))
                    file.Parameters.Tables.Remove(view);
            }
        }

        private void dbConnect_VisibleChanged(object sender, EventArgs e)
        {
            ToString();
        }

    }

}
