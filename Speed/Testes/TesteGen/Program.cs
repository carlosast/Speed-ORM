using Dapper;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Speed;
using Speed.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TesteGen.Data_Model;
using TesteGen.Hibernate;
using TestGen.SqlServer.BL;

/*
 * 
 * 
 *      Steps to test:
 *      
 *          1 - execute script file 'SCRIPTS.SQL' in your database
 *          2 - Set 2 connections strings in app.config 
 *              
 * 
 * 
 * 
 */

namespace TesteGen
{

    class Program : Speed.Windows.ProgramBase
    {

        #region Declarations

        public static Results Timers = new Results("");
        static string longText = new string('X', 950);
        public static int RecordCount;
        public static int ThreadCount;

        static bool execInsert = true;
        static bool execSelect = false;

        static bool execDapper = false;
        static bool execDataTable = false;
        static bool execEntity = false;
        static bool execNHibernate = true;
        static bool execSpeed = false;
        static bool execSProc = false;
        static bool execReader = false;
        static bool execSql = false;

        #endregion Declarations

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
#if !DEBUG
                RecordCount = 1000;
                ThreadCount = 10;
#else
                RecordCount = 100;
                ThreadCount = 10;
#endif
                ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CSSqlServer"].ConnectionString;

                bool tests = true;

#if !DEBUG
                tests = false;
#endif

                if (tests)
                {
                    Tests();
                    return;
                }

                try
                {
                    using (var db = NewDatabase()) { }
                }
                catch (Exception ex)
                {
                    Program.ShowError("Error accessing the database.\r\nChange 2 ConnectionStrings in file 'app.config'\r\n" + ex.Message);
                }

                // first calls to compile
                using (var db = NewDatabase())
                    BL_Sales.SelectByPk(db, 0);

                using (var cn = NewConnection())
                    cn.Query<DapSale>("select * from Sales where TestPerfID=0");

                using (var ef = NewEFContext())
                    ef.Sales.Where(p => p.TestPerfID == 0);

                InitializeSessionFactory();
                using (var se = SessionFactory.OpenSession())
                    se.Load<TesteGen.Hibernate.Sales>(-1);

                Application.Run(new FormTests());
                return;
            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
            }
        }

        #region Speed

        public static void InsertSpeed()
        {
            // ------ INSERT Speed
            Timers.Add("INSERT Speed", Exec(() =>
            {
                using (var db = NewDatabase())
                {
                    for (int i = 0; i < RecordCount; i++)
                    {
                        var rec = new TestGen.SqlServer.Data.Sales
                        {
                            CarrierTrackingNumber = "CarrierTrackingNumber" + i,
                            OrderQty = i,
                            ProductID = i,
                            SpecialOfferID = i,
                            UnitPrice = i,
                            UnitPriceDiscount = i,
                            rowguid = Guid.NewGuid(),
                            LongText = longText + i,
                            ModifiedDate = DateTime.Now
                        };
                        db.Insert<TestGen.SqlServer.Data.Sales>(rec);
                    }
                }
            }));
        }

        public static void InsertSpeedXml()
        {
            // ------ INSERT Speed
            Timers.Add("INS Speed Xml", Exec(() =>
            {
                using (var db = NewDatabase())
                {
                    List<TestGen.SqlServer.Data.Sales> recs = new List<TestGen.SqlServer.Data.Sales>();
                    for (int i = 0; i < RecordCount; i++)
                    {
                        var rec = new TestGen.SqlServer.Data.Sales
                        {
                            CarrierTrackingNumber = "CarrierTrackingNumber" + i,
                            OrderQty = i,
                            ProductID = i,
                            SpecialOfferID = i,
                            UnitPrice = i,
                            UnitPriceDiscount = i,
                            rowguid = Guid.NewGuid(),
                            LongText = longText + i,
                            ModifiedDate = DateTime.Now
                        };
                        recs.Add(rec);
                    }

                    //DataClassSales dc = new DataClassSales();
                    //dc.InsertXml(db, recs);
                    db.InsertXml<TestGen.SqlServer.Data.Sales>(recs);
                }
            }));
        }

        public static void SelectSpeed()
        {
            // ------ Select Speed
            Timers.Add("SELECT Speed", Exec(() =>
            {
                using (var db = NewDatabase())
                {
                    var recsS = db.Select<TestGen.SqlServer.Data.Sales>();
                }
            }));
        }

        #endregion Speed

        #region NHibernate

        public static void InsertNHibernate()
        {
            // ------ INSERT Speed
            Timers.Add("INSERT NHibernate", Exec(() =>
            {
                using (var session = SessionFactory.OpenSession())
                {
                    for (int i = 0; i < RecordCount; i++)
                    {
                        var rec = new TesteGen.Hibernate.Sales
                        {
                            CarrierTrackingNumber = "CarrierTrackingNumber" + i,
                            OrderQty = i,
                            ProductID = i,
                            SpecialOfferID = i,
                            UnitPrice = i,
                            UnitPriceDiscount = i,
                            Rowguid = Guid.NewGuid(),
                            LongText = longText + i,
                            ModifiedDate = DateTime.Now
                        };
                        session.Save(rec);
                    }
                }
            }));

        }

        public static void SelectNHibernate()
        {
            // ------ Entity Framework
            Timers.Add("SELECT NHibernate", Exec(() =>
            {
                using (var session = SessionFactory.OpenSession())
                {
                    var recsH = session.CreateCriteria<Sales>().List<Sales>();
                }
            }));
        }

        #endregion NHibernate

        #region Dapper

        public static void InsertDapper()
        {
            // ------ INSERT Dapper
            Timers.Add("INSERT Dapper", Exec(() =>
            {
                using (var cn = NewConnection())
                {
                    for (int i = 0; i < RecordCount; i++)
                    {
                        string sql = "INSERT INTO Sales (CarrierTrackingNumber,OrderQty,ProductID,SpecialOfferID,UnitPrice,UnitPriceDiscount,rowguid,LongText,ModifiedDate) VALUES(@CarrierTrackingNumber,@OrderQty,@ProductID,@SpecialOfferID,@UnitPrice,@UnitPriceDiscount,@rowguid,@LongText,@ModifiedDate)";
                        var rec = new DapSale
                        {
                            CarrierTrackingNumber = "CarrierTrackingNumber" + i,
                            OrderQty = i,
                            ProductID = i,
                            SpecialOfferID = i,
                            UnitPrice = i,
                            UnitPriceDiscount = i,
                            Rowguid = Guid.NewGuid(),
                            LongText = longText + i,
                            ModifiedDate = DateTime.Now
                        };
                        cn.Execute(sql, rec);
                    }
                }
            }));
        }

        public static void SelectDapper()
        {
            // ------ Select Dapper
            Timers.Add("SELECT Dapper", Exec(() =>
            {
                using (var cn = NewConnection())
                {
                    var recsD = cn.Query<DapSale>("select * from Sales");
                }
            }));
        }

        #endregion Dapper

        #region Procedure

        public static void InsertProcedure()
        {
            // ------ INSERT Stored Procedure
            Timers.Add("INSERT Stored Procedure", Exec(() =>
            {
                using (var cn = NewConnection())
                {
                    for (int i = 0; i < RecordCount; i++)
                    {
                        using (var cmd = new SqlCommand("SPD_Sales", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@CarrierTrackingNumber", SqlDbType.VarChar, 50).Value = "CarrierTrackingNumber" + i;
                            cmd.Parameters.Add("@OrderQty", SqlDbType.Int).Value = i;
                            cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = i;
                            cmd.Parameters.Add("@SpecialOfferID", SqlDbType.Int).Value = i;
                            cmd.Parameters.Add("@UnitPrice", SqlDbType.Money).Value = i;
                            cmd.Parameters.Add("@UnitPriceDiscount", SqlDbType.Money).Value = i;
                            cmd.Parameters.Add("@rowguid", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                            cmd.Parameters.Add("@LongText", SqlDbType.VarChar, 1000).Value = longText + i;
                            cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }));
        }

        #endregion Procedure

        #region Sql

        public static void InsertSql()
        {
            // ------ INSERT Sql
            Timers.Add("INSERT Sql", Exec(() =>
            {
                string sql = "INSERT INTO Sales (CarrierTrackingNumber,OrderQty,ProductID,SpecialOfferID,UnitPrice,UnitPriceDiscount,rowguid,LongText,ModifiedDate) VALUES(@CarrierTrackingNumber,@OrderQty,@ProductID,@SpecialOfferID,@UnitPrice,@UnitPriceDiscount,@rowguid,@LongText,@ModifiedDate)";
                using (var cn = NewConnection())
                {
                    for (int i = 0; i < RecordCount; i++)
                    {
                        using (var cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.Add("@CarrierTrackingNumber", SqlDbType.VarChar, 50).Value = "CarrierTrackingNumber" + i;
                            cmd.Parameters.Add("@OrderQty", SqlDbType.Int).Value = i;
                            cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = i;
                            cmd.Parameters.Add("@SpecialOfferID", SqlDbType.Int).Value = i;
                            cmd.Parameters.Add("@UnitPrice", SqlDbType.Money).Value = i;
                            cmd.Parameters.Add("@UnitPriceDiscount", SqlDbType.Money).Value = i;
                            cmd.Parameters.Add("@rowguid", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                            cmd.Parameters.Add("@LongText", SqlDbType.VarChar, 1000).Value = longText + i;
                            cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }));
        }

        #endregion Sql

        #region Entity

        public static void InsertEntity()
        {
            // ------ INSERT Entity Framework
            Timers.Add("INSERT Entity Framework", Exec(() =>
            {
                using (var ef = NewEFContext())
                {
                    for (int i = 0; i < RecordCount; i++)
                    {
                        var rec = new EFSale
                        {
                            CarrierTrackingNumber = "CarrierTrackingNumber" + i,
                            OrderQty = i,
                            ProductID = i,
                            SpecialOfferID = i,
                            UnitPrice = i,
                            UnitPriceDiscount = i,
                            rowguid = Guid.NewGuid(),
                            LongText = longText + i,
                            ModifiedDate = DateTime.Now
                        };
                        ef.Sales.Add(rec);
                        ef.SaveChanges();
                    }
                    //ef.SaveChanges();
                }
            }));
        }

        public static void SelectEntity()
        {
            // ------ Entity Framework
            Timers.Add("SELECT Entity Framework", Exec(() =>
            {
                using (var ef = NewEFContext())
                {
                    var recsE = ef.Sales.ToList();
                }
            }));
        }

        #endregion Entity

        #region DataReader

        public static void SelectDataReader()
        {
            // ------ Select DataReader
            Timers.Add("SELECT DataReader", Exec(() =>
            {
                using (var cn = NewConnection())
                {
                    using (var cmd = new SqlCommand("SELECT TestPerfID,CarrierTrackingNumber,OrderQty,ProductID,SpecialOfferID,UnitPrice,UnitPriceDiscount,rowguid,LineTotal,LongText,ModifiedDate FROM dbo.Sales", cn))
                    {
                        List<TestGen.SqlServer.Data.Sales> recsR = new List<TestGen.SqlServer.Data.Sales>();
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var rec = new TestGen.SqlServer.Data.Sales();
                                rec.TestPerfID = dr.GetInt32(0);
                                rec.CarrierTrackingNumber = dr.GetString(1);
                                rec.OrderQty = dr.GetInt32(2);
                                rec.ProductID = dr.GetInt32(3);
                                rec.SpecialOfferID = dr.GetInt32(4);
                                rec.UnitPrice = dr.GetDecimal(5);
                                rec.UnitPriceDiscount = dr.GetDecimal(6);
                                rec.rowguid = dr.GetGuid(7);
                                rec.LineTotal = dr.GetDecimal(8);
                                rec.LongText = dr.GetString(9);
                                rec.ModifiedDate = dr.GetDateTime(10);
                                recsR.Add(rec);
                            }
                        }
                    }
                }
            }));
        }

        #endregion DataReader

        #region DataTable

        public static void SelectDataTable()
        {
            // ------ Select DataTable
            Timers.Add("SELECT DataTable", Exec(() =>
            {
                using (var db = NewDatabase())
                {
                    var recsT = db.ExecuteDataTable("select * from Sales");
                }
            }));
        }

        #endregion DataTable

        #region Connections

        public static Database NewDatabase()
        {
            var db = new Database(EnumDbProviderType.SqlServer, ConnectionString);
            db.Open();
            return db;
        }

        static SqlConnection NewConnection()
        {
            var cn = new SqlConnection(ConnectionString);
            cn.Open();
            return cn;
        }

        static SpeedTestsEntities NewEFContext()
        {
            //string connectionString = string.Format(
            //    "metadata=res://*/SpeedTestModel.csdl|res://*/SpeedTestModel.ssdl|res://*/SpeedTestModel.msl;provider=SqlClient;provider connection string=&quot;data source={0};initial catalog={1};integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;",
            //    Server, DatabaseName);
            var ef = new SpeedTestsEntities();
            return ef;
        }

        private static ISessionFactory _sessionFactory;
        private static ISessionFactory SessionFactory
        {
            get
            {
                InitializeSessionFactory();
                return _sessionFactory;
            }
        }

        public static void InitializeSessionFactory()
        {
            if (_sessionFactory == null)
            {
                _sessionFactory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString(ConnectionString))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Sales>())
                    .BuildSessionFactory();
            }
        }

        #endregion Connections

        #region OLD

        static void Prepare()
        {
            using (var db = NewDatabase())
            {
                try
                {
                    var tb = db.ExecuteDataTable("select top 0 * from Sales");
                }
                catch
                {
                    Console.WriteLine("creating table Sales");

                    string sql =
@"CREATE TABLE Sales(
	[TestPerfID] [int] IDENTITY(1,1) NOT NULL,
	[CarrierTrackingNumber] [nvarchar](50) NULL,
	[OrderQty] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[SpecialOfferID] [int] NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[UnitPriceDiscount] [money] NOT NULL,
	[LineTotal]  AS (isnull(([UnitPrice]*((1.0)-[UnitPriceDiscount]))*[OrderQty],(0.0))),
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TestPerf_SalesOrderID_TestPerfID] PRIMARY KEY CLUSTERED 
(
	[TestPerfID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
";
                    db.ExecuteNonQuery(sql);

                    Console.WriteLine("Inserting 1.000.000 records in table Sales");

                    sql =
@"set nocount on;
truncate table Sales;

declare @i int, @s varchar(10);
set @i = 0;

while @i < 1000000 begin
	set @i = @i + 1;
	set @s= cast(@i as varchar(10));

	INSERT INTO Sales
           ([CarrierTrackingNumber]
           ,[OrderQty]
           ,[ProductID]
           ,[SpecialOfferID]
           ,[UnitPrice]
           ,[UnitPriceDiscount]
           ,[rowguid]
           ,[ModifiedDate])
     VALUES (
		   'Carrier' + @s,
           1, 
           @i, 
           @i, 
           @i, 
           @i, 
           NewId(),
           GETDATE()
		   )
end;
";

                    db.ExecuteNonQuery(sql, 0);

                }
            }
        }

        #endregion OLD

        #region Exec

        static long Exec(Action action)
        {
            Stopwatch t = new Stopwatch();
            t.Start();

            Task[] tasks = new Task[ThreadCount];

            for (int i = 0; i < ThreadCount; i++)
                tasks[i] = new Task(action);

            for (int i = 0; i < ThreadCount; i++)
                tasks[i].Start();

            Task.WaitAll(tasks);

            t.Stop();
            var ms = t.ElapsedMilliseconds;
            Wait();
            return ms;
        }

        #endregion Exec

        #region Helper

        public static void Wait(double seconds = 1)
        {
            Thread.Sleep((int)(seconds * 1000));
        }

        public static void TruncateSales()
        {
            using (var db = NewDatabase())
                db.ExecuteNonQuery("truncate table Sales");
        }

        public static int GetRecordCount()
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

        #endregion Helper

        #region Working Tests

        enum EnumTests { One, All, Performance, TestXml, TestAccess, TestSqlServer, TestSqllite, TestOracle, TestPostgreSQL, TestMySql, TestMariaDB, TestFirebird };
        enum EnumTestMode { Default, DataClass, Performance, Metadata, Select, Insert, Update, Generate, Concurrency };
        static bool useTran;

        /// <summary>
        /// Testes de desenvolvimento
        /// </summary>
        static void Tests()
        {
            try
            {
                RecordCount = 10;
                ThreadCount = 1;
                useTran = true;

                EnumTests test = EnumTests.One;

                if (test == EnumTests.One)
                {

                    //GenProcedures();
                    //TestAccess();
                    TestSqlServer(EnumTestMode.Concurrency);
                    // TestSqllite();
                    // TestFirebird();
                    //TestMySql();
                    // GenerateSqlServer();
                    //TestOracle();
                }

                else if (test == EnumTests.All)
                {
                    var file = new FileInfo("./_Performance" + (IntPtr.Size * 8) + ".txt");
                    if (file.Exists)
                    {
                        if (file.IsReadOnly)
                            file.IsReadOnly = false;
                        file.Delete();
                    }

                    using (var w = new StreamWriter(file.FullName))
                    {
                        w.WriteLine("useTran={0} - {1} bits ", useTran, IntPtr.Size * 8);

                        ExecTest(EnumTests.TestAccess, w, () => TestAccess(EnumTestMode.Performance, w));

                        ExecTest(EnumTests.TestSqlServer, w, () => TestSqlServer(EnumTestMode.Performance, w));

                        ExecTest(EnumTests.TestSqllite, w, () => TestSqllite(EnumTestMode.Performance, w));

                        ExecTest(EnumTests.TestOracle, w, () => TestOracle(EnumTestMode.Performance, w));

                        ExecTest(EnumTests.TestMySql, w, () => TestMySql(EnumTestMode.Performance, w));

                        ExecTest(EnumTests.TestMariaDB, w, () => TestMariaDB(EnumTestMode.Performance, w));

                        ExecTest(EnumTests.TestPostgreSQL, w, () => TestPostgreSQL(EnumTestMode.Performance, w));

                        ExecTest(EnumTests.TestFirebird, w, () => TestFirebird(EnumTestMode.Performance, w));
                    }
                    Process.Start(file.FullName);
                }

                else if (test == EnumTests.TestAccess)
                {
                    TestAccess();
                }
                else if (test == EnumTests.TestOracle)
                {
                    TestOracle();
                }
                else if (test == EnumTests.TestPostgreSQL)
                {
                    TestPostgreSQL();
                }
                else if (test == EnumTests.TestSqllite)
                {
                    TestSqllite();
                }
                else if (test == EnumTests.TestSqlServer)
                {
                    TestSqlServer();
                }
                else if (test == EnumTests.TestMySql)
                {
                    TestMySql();
                }
                else if (test == EnumTests.TestMariaDB)
                {
                    TestMariaDB();
                }
                else if (test == EnumTests.TestFirebird)
                {
                    TestFirebird();
                }

                #region Performance

                else if (test == EnumTests.Performance)
                {
                    // first calls to compile
                    using (var db = NewDatabase())
                        BL_Sales.SelectByPk(db, 0);

                    // TestXml(); return;

                    using (var cn = NewConnection())
                        cn.Query<DapSale>("select * from Sales where TestPerfID=0");

                    using (var ef = NewEFContext())
                        ef.Sales.Where(p => p.TestPerfID == 0);

                    InitializeSessionFactory();
                    using (var se = SessionFactory.OpenSession())
                        se.Load<TesteGen.Hibernate.Sales>(-1);

                    Application.Run(new FormTests());
                    return;

                    TruncateSales();

                    //Prepare();
                    //TestAdventureWork();

                    try
                    {
                        Timers = new Results("INSERT");

                        // ********************** INSERT **********************
                        Console.WriteLine("INSERT {0} records. ThreadCount: {1}", RecordCount, ThreadCount);
                        TruncateSales(); Wait();

                        if (execInsert)
                        {
                            if (execDapper)
                            {
                                TruncateSales(); Wait();
                                InsertDapper();
                            }

                            if (execSProc)
                            {
                                TruncateSales(); Wait();
                                InsertProcedure();
                            }

                            if (execNHibernate)
                            {
                                TruncateSales(); Wait();
                                InsertNHibernate();
                            }

                            if (execSpeed)
                            {
                                TruncateSales(); Wait();
                                InsertSpeed();
                            }

                            if (execSql)
                            {
                                TruncateSales(); Wait();
                                InsertSql();
                            }

                            if (execEntity)
                            {
                                TruncateSales(); Wait();
                                InsertEntity();
                            }

                            Console.WriteLine(Timers.Report());
                        }


                        //// garante que tem registros pro select
                        //using (var db = NewDbSqlServer())
                        //{
                        //    if (db.ExecuteInt32("select count(*) from Sales") == 0)
                        //    {
                        //        timers.Clear();
                        //        InsertSpeed();
                        //    }
                        //}

                        if (execSelect)
                        {
                            int recs;
                            using (var db = NewDatabase())
                                recs = db.ExecuteInt32("select count(*) from Sales");

                            if (recs < RecordCount)
                            {
                                TruncateSales();
                                InsertSpeed();
                            }

                            // ********************** SELECT **********************
                            Console.WriteLine("SELECT {0} records. ThreadCount: {1}", recs, ThreadCount);
                            Timers = new Results("SELECT");
                            Wait();

                            if (execReader)
                            {
                                SelectDataReader();
                                Wait();
                            }

                            if (execDapper)
                            {
                                SelectDapper();
                                Wait();
                            }

                            if (execEntity)
                            {
                                SelectEntity();
                                Wait();
                            }

                            if (execSpeed)
                            {
                                SelectSpeed();
                                Wait();
                            }

                            if (execDataTable)
                            {
                                SelectDataTable();
                                Wait();
                            }

                            Console.WriteLine(Timers.Report());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}\r\n\r\n{1}", ex.Message, ex.StackTrace);
                    }
                }
                #endregion Performance

                #region TestXml()

                else if (test == EnumTests.TestXml)
                {
                    TestXml(); return;
                }

                #endregion TestXml()

            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
            }
        }

        static void ExecTest(EnumTests test, StreamWriter w, Action action)
        {
            string line = "\r\n" + new string('=', 80) + "\r\n";
            try
            {
                action();
            }
            catch (Exception ex)
            {
                w.WriteLine("Error {0} : {1}", test, Conv.GetErrorMessage(ex, true));
            }
            w.WriteLine(line);
        }

        static void TestXml()
        {
            using (var db = NewDatabase())
            {
                var recs = new List<TestGen.SqlServer.Data.Sales>();
                RecordCount = 10000;
                for (int i = 0; i < RecordCount; i++)
                {
                    var rec = new TestGen.SqlServer.Data.Sales
                    {
                        CarrierTrackingNumber = "CarrierTrackingNumber" + i,
                        OrderQty = i,
                        ProductID = i,
                        SpecialOfferID = i,
                        UnitPrice = i,
                        UnitPriceDiscount = i,
                        rowguid = Guid.NewGuid(),
                        LongText = longText + i,
                        ModifiedDate = DateTime.Now
                    };
                    recs.Add(rec);
                }

                TruncateSales();
                TimerCount tc = new TimerCount("DataTable");

                // create DataTable
                DataTable tb = new DataTable("Table");
                //tb.Columns.Add("TestPerfID", typeof(Int32));
                tb.Columns.Add("CarrierTrackingNumber", typeof(string));
                tb.Columns.Add("OrderQty", typeof(Int32));
                tb.Columns.Add("ProductID", typeof(Int32));
                tb.Columns.Add("SpecialOfferID", typeof(Int32));
                tb.Columns.Add("UnitPrice", typeof(Decimal));
                tb.Columns.Add("UnitPriceDiscount", typeof(Decimal));
                //tb.Columns.Add("LineTotal", typeof(Decimal));
                tb.Columns.Add("rowguid", typeof(Guid));
                tb.Columns.Add("LongText", typeof(String));
                tb.Columns.Add("ModifiedDate", typeof(DateTime));

                foreach (var rec in recs)
                {
                    DataRow row = tb.NewRow();
                    //row["TestPerfID"] = rec.TestPerfID;
                    row["CarrierTrackingNumber"] = rec.CarrierTrackingNumber;
                    row["OrderQty"] = rec.OrderQty;
                    row["ProductID"] = rec.ProductID;
                    row["SpecialOfferID"] = rec.SpecialOfferID;
                    row["UnitPrice"] = rec.UnitPrice;
                    row["UnitPriceDiscount"] = rec.UnitPriceDiscount;
                    //row["LineTotal"] = rec.LineTotal;
                    row["rowguid"] = rec.rowguid;
                    row["LongText"] = rec.LongText;
                    row["ModifiedDate"] = rec.ModifiedDate;
                    tb.Rows.Add(row);
                }

                string sql =
@"
declare @xmlData xml = @Xml
BEGIN
    DECLARE @idInt int;

    EXEC sp_xml_preparedocument @idInt OUTPUT, @xmlData;

    INSERT INTO Sales(CarrierTrackingNumber,OrderQty,ProductID,SpecialOfferID,UnitPrice,UnitPriceDiscount,rowguid,LongText,ModifiedDate)
    SELECT CarrierTrackingNumber,OrderQty,ProductID,SpecialOfferID,UnitPrice,UnitPriceDiscount,rowguid,LongText,ModifiedDate
    FROM OPENXML (@idInt, '/DocumentElement/*')
    WITH 
    (
        --Id int '@id', DATA XML '.'
        CarrierTrackingNumber nvarchar(50) 'CarrierTrackingNumber',
        OrderQty int 'OrderQty',
        ProductID int 'ProductID',
        SpecialOfferID int 'SpecialOfferID',
        UnitPrice money 'UnitPrice',
        UnitPriceDiscount money 'UnitPriceDiscount',
        LongText varchar(100) 'LongText',
        rowguid uniqueidentifier 'rowguid',
        ModifiedDate datetime 'ModifiedDate'
    )

    select CAST( SCOPE_IDENTITY() - @@ROWCOUNT + 1 AS INT) 'FIRST_ID';
    exec sp_xml_removedocument @idInt;
END;";
                tc.Next("InsertXml");

                using (StringWriter swStringWriter = new StringWriter())
                {
                    // Datatable as XML format 
                    tb.WriteXml(swStringWriter);
                    // Datatable as XML string 
                    string xml = swStringWriter.ToString();
                    using (var cmd = db.NewCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        db.AddParameter(cmd, "@xml", DbType.Xml, xml, ParameterDirection.Input);
                        int firstId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                //RecordCount = 1;
                ThreadCount = 1;
                tc.Next("InsertSpeed");
                InsertSpeed();

                string time = tc.ToString();
                ShowInformation(time);
            }
        }

        static void TestAccess(EnumTestMode test = EnumTestMode.Default, StreamWriter stream = null)
        {
            using (var db = new Database(EnumDbProviderType.Access, "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + @"\bdExemplo.accdb"))
            {
                // var db = new Database(EnumDbProviderType.Access,  "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + @"\bdExemplo.mdb");

                db.Open();

                //var oleDbMetaData = db.GetAllOleDbMetadata();

                if (test == EnumTestMode.Default)
                    test = EnumTestMode.Performance;

                #region Metadata

                if (test == EnumTestMode.Metadata)
                {
                    var tb1 = db.GetSchema("Tables");
                    var tb2 = db.GetSchema("ForeignKeys");
                    var tbcs = db.GetSchema("Columns").GetText();

                    //var tb = db.ExecuteDataTable("PRAGMA table_info()");


                    string txt = db.GetAllMetadata();
                }

                #endregion Metadata

                #region Generate

                else if (test == EnumTestMode.Generate)
                {
                    TimerCount tc = new TimerCount("Compilation"); // 0.35s
                    db.GenerateTables("TestGen.Access.Data", "TestGen.Access.BL", "../../../TestGen.Access/Data/Base", "../../../TestGen.Access/Data", "../../../TestGen.Access/BL/Base", "../../../TestGen.Access/BL", false, Speed.Data.Generation.EnumNameCase.Pascal);
                    ShowInformation(tc.ToString());
                }

                #endregion Generate

                #region Performance

                else if (test == EnumTestMode.Performance)
                {

                    try
                    {
                        int count = RecordCount;

                        TestGen.Access.BL.BL_Customer.Delete(db);

                        /*
                        var dc = new DataClassCustomer();
                        var rec1 = new TestGen.Access.Data.Customer { CustomerName = "Name " + 1 };
                        dc.Insert(db, rec1, EnumSaveMode.None);
                        dc.Insert(db, rec1, EnumSaveMode.Requery);

                        dc.Update(db, rec1, EnumSaveMode.None);
                        dc.Update(db, rec1, EnumSaveMode.Requery);
                        */


                        TimerCount tc1 = new TimerCount("Access - Count: " + count);

                        if (useTran)
                            db.BeginTransaction();

                        tc1.Next("Insert EnumSaveMode.None " + count);
                        for (int i = 1; i <= count; i++)
                        {
                            var rec = new TestGen.Access.Data.Customer { CustomerName = "Name " + i };
                            TestGen.Access.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                        }

                        tc1.Next("Insert EnumSaveMode.Requery " + count); // RefCursor is 3x faster
                        for (int i = count + 1; i <= 2 * count + 1; i++)
                        {
                            var rec = new TestGen.Access.Data.Customer { CustomerName = "Name " + i };
                            TestGen.Access.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                        }

                        tc1.Next("Select " + 2 * count);
                        var recs = TestGen.Access.BL.BL_Customer.Select(db);

                        tc1.Next("Update EnumSaveMode.None " + 2 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.Access.BL.BL_Customer.Update(db, rec, EnumSaveMode.None);
                        }


                        tc1.Next("Update EnumSaveMode.Requery " + 2 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.Access.BL.BL_Customer.Update(db, rec, EnumSaveMode.Requery);
                        }

                        tc1.Next("SaveList EnumSaveMode.None " + 2 * count);
                        TestGen.Access.BL.BL_Customer.SaveList(db, recs, EnumSaveMode.None, false);

                        tc1.Next("SaveList EnumSaveMode.Requery " + 2 * count);
                        TestGen.Access.BL.BL_Customer.SaveList(db, recs, EnumSaveMode.Requery, false);

                        tc1.Next("Delete " + 2 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.Access.BL.BL_Customer.Delete(db, rec);
                        }

                        if (useTran)
                            db.Commit();

                        if (stream == null)
                            ShowInformation(tc1.ToString());
                        else
                            stream.WriteLine(tc1.ToString());
                    }
                    catch (Exception ex)
                    {
                        Program.ShowError(ex);
                    }
                }

                #endregion Performance

                #region Select

                else if (test == EnumTestMode.Select)
                {
                    TimerCount tc = new TimerCount("Compilation"); // 0.35s

                    var recs = TestGen.Access.BL.BL_Customer.Select(db, "CustomerId=-1");
                    tc.Next("Select");
                    recs = TestGen.Access.BL.BL_Customer.Select(db);

                    string time = tc.ToString();
                    ShowInformation(time);
                }

                #endregion Select

                #region  Insert

                else if (test == EnumTestMode.Insert)
                {
                    var recs = TestGen.Access.BL.BL_Customer.Select(db, "CustomerId=-1");
                    TestGen.Access.BL.BL_Customer.Delete(db);

                    int count = RecordCount;
                    var tc = new TimerCount("Insert " + count);

                    for (int i = 0; i < count; i++)
                    {
                        TestGen.Access.Data.Customer rec = new TestGen.Access.Data.Customer();
                        rec.CustomerName = "Name " + (i + 1);
                        // Acc.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                        TestGen.Access.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    }

                    tc.Next("SQL");
                    for (int i = 0; i < count; i++)
                    {
                        string sql = string.Format("insert into customer(CustomerName) values({0})", Conv.ToSqlTextA("Name " + (i + 1)));
                        db.ExecuteNonQuery(sql);
                    }

                    string time = tc.ToString();
                    ShowInformation(time);
                    recs = TestGen.Access.BL.BL_Customer.Select(db);
                    recs.ToString();
                }

                #endregion  Insert

                #region  Update

                else if (test == EnumTestMode.Update)
                {
                    var recs = TestGen.Access.BL.BL_Customer.Select(db);

                    for (int i = 0; i < Math.Min(recs.Count, 10); i++)
                    {
                        recs[i].CustomerName = "Update " + (i + 1);
                        TestGen.Access.BL.BL_Customer.Update(db, recs[i], EnumSaveMode.Requery);
                        recs[i].ToString();
                    }
                }

                #endregion  Update

                db.ToString();
            }
        }

        static void TestSqlServer(EnumTestMode test = EnumTestMode.Default, StreamWriter stream = null)
        {
            using (var db = NewDatabase())
            {

                //var oleDbMetaData = db.GetAllOleDbMetadata();

                if (test == EnumTestMode.Default)
                    test = EnumTestMode.Concurrency;

                #region Metadata

                if (test == EnumTestMode.Metadata)
                {
                    var tb1 = db.GetSchema("Tables");
                    var tb2 = db.GetSchema("ForeignKeys");
                    var tbcs = db.GetSchema("Columns").GetText();

                    //var tb = db.ExecuteDataTable("PRAGMA table_info()");


                    string txt = db.GetAllMetadata();
                }

                #endregion Metadata

                #region Generate

                else if (test == EnumTestMode.Generate)
                {
                    TimerCount tc = new TimerCount("Compilation"); // 0.35s
                    db.GenerateTables("TestGen.SqlServer.Data", "TestGen.SqlServer.BL", "../../../TestGen.SqlServer/Data/Base", "../../../TestGen.SqlServer/Data", "../../../TestGen.SqlServer/BL/Base", "../../../TestGen.SqlServer/BL", false, Speed.Data.Generation.EnumNameCase.Pascal);
                    ShowInformation(tc.ToString());
                }

                #endregion Generate

                #region Concurrency
                else if (test == EnumTestMode.Concurrency)
                {

                    Sys.ProviderType = EnumDbProviderType.SqlServer;
                    Sys.ConnectionString = "Data Source=localhost;Initial Catalog=SpeedTests;Integrated Security=True;MultipleActiveResultSets=True;";

                    // select 1 record - Second parameter (concurrency must be true)
                    var rec1 = db.SelectSingle<TestGen.SqlServer.Data.Customer>("CustomerId=1", true);
                    // change value to 'Teste 1'
                    rec1.CustomerName = "Test 1";
                    // Save and requery value
                    db.Save<TestGen.SqlServer.Data.Customer>(rec1, EnumSaveMode.Requery);
                    // Now in database, CustomerName ='Teste 1'

                    // make a copy
                    var rec2 = rec1.CloneT();

                    // change and save rec2
                    rec2.CustomerName = "Test 2";
                    db.Save<TestGen.SqlServer.Data.Customer>(rec2);
                    // Now in database, CustomerName ='Teste 2'

                    try
                    {
                        // save rec1 - Concurency Exception
                        db.Save<TestGen.SqlServer.Data.Customer>(rec1);
                    }
                    catch (DBConcurrencyException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                #endregion Concurrency

                #region Performance

                else if (test == EnumTestMode.Performance)
                {

                    try
                    {
                        int count = RecordCount;

                        TestGen.SqlServer.BL.BL_Customer.Delete(db);

                        TimerCount tc1 = new TimerCount("Sql Server - Count: " + count);

                        if (useTran)
                            db.BeginTransaction();

                        tc1.Next("Insert EnumSaveMode.None " + count);
                        for (int i = 1; i <= count; i++)
                        {
                            var rec = new TestGen.SqlServer.Data.Customer { CustomerName = "Name " + i };
                            TestGen.SqlServer.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                        }

                        tc1.Next("Insert EnumSaveMode.Requery " + count);
                        for (int i = count + 1; i <= 2 * count + 1; i++)
                        {
                            var rec = new TestGen.SqlServer.Data.Customer { CustomerName = "Name " + i };
                            TestGen.SqlServer.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                        }

                        tc1.Next("Insert Xml " + count);
                        var recs = new List<TestGen.SqlServer.Data.Customer>();
                        for (int i = 2 * count + 1; i <= 3 * count + 1; i++)
                        {
                            recs.Add(new TestGen.SqlServer.Data.Customer { CustomerName = "Name " + i });
                        }
                        TestGen.SqlServer.BL.BL_Customer.InsertXml(db, recs);

                        tc1.Next("Select " + 3 * count);
                        recs = TestGen.SqlServer.BL.BL_Customer.Select(db);

                        tc1.Next("Update EnumSaveMode.None " + 3 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.SqlServer.BL.BL_Customer.Update(db, rec, EnumSaveMode.None);
                        }


                        tc1.Next("Update EnumSaveMode.Requery " + 3 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.SqlServer.BL.BL_Customer.Update(db, rec, EnumSaveMode.Requery);
                        }

                        tc1.Next("SaveList EnumSaveMode.None " + 3 * count);
                        TestGen.SqlServer.BL.BL_Customer.SaveList(db, recs, EnumSaveMode.None, false);

                        tc1.Next("SaveList EnumSaveMode.Requery " + 3 * count);
                        TestGen.SqlServer.BL.BL_Customer.SaveList(db, recs, EnumSaveMode.Requery, false);

                        tc1.Next("Delete " + 3 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.SqlServer.BL.BL_Customer.Delete(db, rec);
                        }

                        if (useTran)
                            db.Commit();

                        if (stream == null)
                            ShowInformation(tc1.ToString());
                        else
                            stream.WriteLine(tc1.ToString());
                    }
                    catch (Exception ex)
                    {
                        Program.ShowError(ex);
                    }
                }

                #endregion Performance

                #region Select

                else if (test == EnumTestMode.Select)
                {
                    TimerCount tc = new TimerCount("Compilation"); // 0.35s

                    var recs = TestGen.SqlServer.BL.BL_Customer.Select(db, "CustomerId=-1");
                    tc.Next("Select");
                    recs = TestGen.SqlServer.BL.BL_Customer.Select(db);

                    string time = tc.ToString();
                    ShowInformation(time);
                }

                #endregion Select

                #region  Insert

                else if (test == EnumTestMode.Insert)
                {
                    var recs = TestGen.SqlServer.BL.BL_Customer.Select(db, "CustomerId=-1");
                    TestGen.SqlServer.BL.BL_Customer.Delete(db);

                    int count = RecordCount;
                    var tc = new TimerCount("Insert " + count);

                    for (int i = 0; i < count; i++)
                    {
                        TestGen.SqlServer.Data.Customer rec = new TestGen.SqlServer.Data.Customer();
                        rec.CustomerName = "Name " + (i + 1);
                        // Acc.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                        TestGen.SqlServer.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    }

                    tc.Next("SQL");
                    for (int i = 0; i < count; i++)
                    {
                        string sql = string.Format("insert into customer(CustomerName) values({0})", Conv.ToSqlTextA("Name " + (i + 1)));
                        db.ExecuteNonQuery(sql);
                    }

                    string time = tc.ToString();
                    ShowInformation(time);
                    recs = TestGen.SqlServer.BL.BL_Customer.Select(db);
                    recs.ToString();
                }

                #endregion  Insert

                #region  Update

                else if (test == EnumTestMode.Update)
                {
                    var recs = TestGen.SqlServer.BL.BL_Customer.Select(db);

                    for (int i = 0; i < Math.Min(recs.Count, 10); i++)
                    {
                        recs[i].CustomerName = "Update " + (i + 1);
                        TestGen.SqlServer.BL.BL_Customer.Update(db, recs[i], EnumSaveMode.Requery);
                        recs[i].ToString();
                    }
                }

                #endregion  Update

                db.ToString();
            }
        }

        static void TestPostgreSQL(EnumTestMode test = EnumTestMode.Default, StreamWriter stream = null)
        {
            string cs = System.Configuration.ConfigurationManager.ConnectionStrings["CSPostgreSQL"].ConnectionString;

            using (var db = new Database(EnumDbProviderType.PostgreSQL, cs))
            {
                db.Open();

                if (test == EnumTestMode.Default)
                    test = EnumTestMode.Performance;

                #region Metadata

                if (test == EnumTestMode.Metadata)
                {
                    var tb1 = db.GetSchema("Tables");
                    var tb2 = db.GetSchema("ForeignKeys");
                    var tbcs = db.GetSchema("Columns").GetText();

                    //var tb = db.ExecuteDataTable("PRAGMA table_info()");


                    string txt = db.GetAllMetadata();
                }

                #endregion Metadata

                #region Generate

                else if (test == EnumTestMode.Generate)
                {
                    TimerCount tc = new TimerCount("Compilation"); // 0.63s
                    db.GenerateTables("TestGen.PostgreSQL.Data", "TestGen.PostgreSQL.BL", "../../../TestGen.PostgreSQL/Data/Base", "../../../TestGen.PostgreSQL/Data", "../../../TestGen.PostgreSQL/BL/Base", "../../../TestGen.PostgreSQL/BL", false, Speed.Data.Generation.EnumNameCase.Pascal);
                    ShowInformation(tc.ToString());
                }

                #endregion Generate

                #region DataClass

                else if (test == EnumTestMode.DataClass)
                {
                    try
                    {
                        /*
                        TestGen.PostgreSQL.BL.BL_Customer.Delete(db);

                        var rec1 = new TestGen.PostgreSQL.Data.Customer { CustomerName = "A" };

                        DataClassCustomer dc = new DataClassCustomer();
                        dc.Insert(db, rec1, EnumSaveMode.None);

                        rec1.CustomerName = "B";
                        dc.Insert(db, rec1, EnumSaveMode.Requery);

                        rec1.CustomerName = "C";
                        dc.Insert(db, rec1, EnumSaveMode.Requery);

                        rec1.CustomerName = "D";
                        TestGen.PostgreSQL.BL.BL_Customer.Insert(db, rec1, EnumSaveMode.Requery);

                        var rec1s = TestGen.PostgreSQL.BL.BL_Customer.Select(db);

                        dc.Update(db, rec1s[0], EnumSaveMode.Requery);

                        rec1s.ToString();
                        */
                    }
                    catch (Exception ex)
                    {
                        Program.ShowError(ex);
                    }
                }

                #endregion DataClass

                #region Performance

                else if (test == EnumTestMode.Performance)
                {
                    try
                    {
                        int count = RecordCount;

                        TestGen.PostgreSQL.BL.BL_Customer.Delete(db);

                        TimerCount tc1 = new TimerCount("PostgreSQL - Count: " + count);

                        if (useTran)
                            db.BeginTransaction();

                        tc1.Next("Insert EnumSaveMode.None " + count);
                        for (int i = 1; i <= count; i++)
                        {
                            var rec = new TestGen.PostgreSQL.Data.Customer { CustomerName = "Name " + i };
                            TestGen.PostgreSQL.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                        }

                        tc1.Next("Insert EnumSaveMode.Requery " + count);
                        for (int i = count + 1; i <= 2 * count + 1; i++)
                        {
                            var rec = new TestGen.PostgreSQL.Data.Customer { CustomerName = "Name " + i };
                            TestGen.PostgreSQL.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                        }

                        tc1.Next("Select " + 2 * count);
                        var recs = TestGen.PostgreSQL.BL.BL_Customer.Select(db);

                        tc1.Next("Update EnumSaveMode.None " + 2 * count);
                        foreach (var rec in recs)
                        {
                            //var dc = new DataClassCustomer();
                            //dc.Update(db, rec, EnumSaveMode.Requery);


                            TestGen.PostgreSQL.BL.BL_Customer.Update(db, rec, EnumSaveMode.None);
                        }


                        tc1.Next("Update EnumSaveMode.Requery " + 2 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.PostgreSQL.BL.BL_Customer.Update(db, rec, EnumSaveMode.Requery);
                        }

                        tc1.Next("SaveList EnumSaveMode.None " + 2 * count);
                        TestGen.PostgreSQL.BL.BL_Customer.SaveList(db, recs, EnumSaveMode.None, false);

                        tc1.Next("SaveList EnumSaveMode.Requery " + 2 * count);
                        TestGen.PostgreSQL.BL.BL_Customer.SaveList(db, recs, EnumSaveMode.Requery, false);

                        tc1.Next("Delete " + 2 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.PostgreSQL.BL.BL_Customer.Delete(db, rec);
                        }

                        if (useTran)
                            db.Commit();

                        if (stream == null)
                            ShowInformation(tc1.ToString());
                        else
                            stream.WriteLine(tc1.ToString());
                    }
                    catch (Exception ex)
                    {
                        Program.ShowError(ex);
                    }
                }

                #endregion Performance

                #region Select

                else if (test == EnumTestMode.Select)
                {
                    TimerCount tc = new TimerCount("Compilation"); // 0.35s

                    var recs = TestGen.PostgreSQL.BL.BL_Customer.Select(db, "CustomerId=-1");
                    tc.Next("Select");
                    recs = TestGen.PostgreSQL.BL.BL_Customer.Select(db);

                    string time = tc.ToString();
                    ShowInformation(time);
                }

                #endregion Select

                #region  Insert

                else if (test == EnumTestMode.Insert)
                {
                    var recs = TestGen.PostgreSQL.BL.BL_Customer.Select(db, "CustomerId=-1");
                    TestGen.PostgreSQL.BL.BL_Customer.Delete(db);

                    int count = RecordCount;
                    var tc = new TimerCount("Insert " + count);

                    for (int i = 0; i < count; i++)
                    {
                        TestGen.PostgreSQL.Data.Customer rec = new TestGen.PostgreSQL.Data.Customer();
                        rec.CustomerName = "Name " + (i + 1);
                        // Acc.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                        TestGen.PostgreSQL.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    }

                    tc.Next("SQL");
                    for (int i = 0; i < count; i++)
                    {
                        string sql = string.Format("insert into customer(CustomerName) values({0})", Conv.ToSqlTextA("Name " + (i + 1)));
                        db.ExecuteNonQuery(sql);
                    }

                    string time = tc.ToString();
                    ShowInformation(time);
                    recs = TestGen.PostgreSQL.BL.BL_Customer.Select(db);
                    recs.ToString();
                }

                #endregion  Insert

                #region  Update

                else if (test == EnumTestMode.Update)
                {
                    var recs = TestGen.PostgreSQL.BL.BL_Customer.Select(db);

                    for (int i = 0; i < Math.Min(recs.Count, 10); i++)
                    {
                        recs[i].CustomerName = "Update " + (i + 1);
                        TestGen.PostgreSQL.BL.BL_Customer.Update(db, recs[i], EnumSaveMode.Requery);
                        recs[i].ToString();
                    }
                }

                #endregion  Update

                db.ToString();
            }
        }

        static void TestSqllite(EnumTestMode test = EnumTestMode.Default, StreamWriter stream = null)
        {
            string cs = "Data Source=./Test.sqlite;Pooling=true;FailIfMissing=false";
            using (var db = new Database(EnumDbProviderType.SQLite, cs))
            {
                db.Open();

                if (test == EnumTestMode.Default)
                    test = EnumTestMode.Performance;

                #region Metadata

                if (test == EnumTestMode.Metadata)
                {
                    var tb1 = db.GetSchema("Tables");
                    var tb2 = db.GetSchema("ForeignKeys");
                    var tbcs = db.GetSchema("Columns").GetText();

                    //var tb = db.ExecuteDataTable("PRAGMA table_info()");


                    string txt = db.GetAllMetadata();
                }

                #endregion Metadata

                #region Performance

                else if (test == EnumTestMode.Performance)
                {

                    try
                    {
                        int count = RecordCount;

                        TestGen.SqlLite.BL.BL_Customer.Delete(db);

                        TimerCount tc1 = new TimerCount("SQLite - Count: " + count);

                        if (useTran)
                            db.BeginTransaction();

                        tc1.Next("Insert EnumSaveMode.None " + count);
                        for (int i = 1; i <= count; i++)
                        {
                            var rec = new TestGen.SqlLite.Data.Customer { CustomerName = "Name " + i };
                            TestGen.SqlLite.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                        }

                        tc1.Next("Insert EnumSaveMode.Requery " + count);
                        for (int i = count + 1; i <= 2 * count + 1; i++)
                        {
                            var rec = new TestGen.SqlLite.Data.Customer { CustomerName = "Name " + i };
                            TestGen.SqlLite.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                        }


                        tc1.Next("Select " + 2 * count);
                        var recs = TestGen.SqlLite.BL.BL_Customer.Select(db);

                        tc1.Next("Update EnumSaveMode.None " + 2 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.SqlLite.BL.BL_Customer.Update(db, rec, EnumSaveMode.None);
                        }


                        tc1.Next("Update EnumSaveMode.Requery " + 2 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.SqlLite.BL.BL_Customer.Update(db, rec, EnumSaveMode.Requery);
                        }

                        tc1.Next("SaveList EnumSaveMode.None " + 2 * count);
                        TestGen.SqlLite.BL.BL_Customer.SaveList(db, recs, EnumSaveMode.None, false);

                        tc1.Next("SaveList EnumSaveMode.Requery " + 2 * count);
                        TestGen.SqlLite.BL.BL_Customer.SaveList(db, recs, EnumSaveMode.Requery, false);

                        tc1.Next("Delete " + 2 * count);
                        foreach (var rec in recs)
                        {
                            TestGen.SqlLite.BL.BL_Customer.Delete(db, rec);
                        }

                        if (useTran)
                            db.Commit();

                        if (stream == null)
                            ShowInformation(tc1.ToString());
                        else
                            stream.WriteLine(tc1.ToString());
                    }
                    catch (Exception ex)
                    {
                        Program.ShowError(ex);
                    }
                }

                #endregion Performance

                #region Generate

                else if (test == EnumTestMode.Generate)
                {
                    db.GenerateTables("TestGen.SqlLite.Data", "TestGen.SqlLite.BL", "../../../TestGen.Sqlite/Data/Base", "../../../TestGen.Sqlite/Data", "../../../TestGen.Sqlite/BL/Base", "../../../TestGen.Sqlite/BL", false, Speed.Data.Generation.EnumNameCase.Pascal);
                }

                #endregion Generate

                #region Select

                else if (test == EnumTestMode.Select)
                {
                    TimerCount tc = new TimerCount("Compilation"); // 0.35s

                    var recNum = TestGen.SqlLite.BL.BL_Customer.Count(db);

                    tc.Next("Select " + recNum); // 100.000 = 0.24s
                    var recs = TestGen.SqlLite.BL.BL_Customer.Select(db);

                    tc.Next("Select " + recNum); // 100.000 = 0.21s
                    recs = TestGen.SqlLite.BL.BL_Customer.Select(db);

                    recs.ToString();

                    string time = tc.ToString();
                    ShowInformation(time);
                }

                #endregion Select

                #region  Insert

                else if (test == EnumTestMode.Insert)
                {
                    var recs = TestGen.SqlLite.BL.BL_Customer.Select(db, "CustomerId=-1");
                    db.ExecuteNonQuery("delete from Customer");
                    db.ExecuteNonQuery("vacuum");

                    //int count = 1000; // 1000 = 0.16s (with transaction)
                    //int count = 10000; // 1000 = 0.58s (with transaction)
                    //int count = 100000; // 1000 = 4.43s (with transaction)

                    int count = RecordCount;

                    var tc = new TimerCount("Insert " + count);

                    // Very slow without transaction

                    //for (int i = 0; i < count; i++)
                    //{
                    //    TestGen.SqlLite.Data.Customer rec = new TestGen.SqlLite.Data.Customer();
                    //    rec.CustomerName = "Name " + (i + 1);
                    //    TestGen.SqlLite.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    //}


                    // SQLite - using Transaction is much faster
                    tc.Next("Transaction");
                    db.BeginTransaction();
                    for (int i = count; i < 2 * count; i++)
                    {
                        TestGen.SqlLite.Data.Customer rec = new TestGen.SqlLite.Data.Customer();
                        rec.CustomerName = "Name " + (i + 1);
                        TestGen.SqlLite.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    }
                    db.Commit();

                    //tc.Next("SQL");

                    //for (int i = 0; i < count; i++)
                    //{
                    //    string sql = string.Format("insert into customer(CustomerName) values({0})", Conv.ToSqlTextA("Name " + (i + 1)));
                    //    db.ExecuteNonQuery(sql);
                    //}


                    string time = tc.ToString();
                    ShowInformation(time);
                    recs = TestGen.SqlLite.BL.BL_Customer.Select(db);
                    recs.ToString();
                }

                #endregion  Insert

                #region  Update

                else if (test == EnumTestMode.Update)
                {
                    var recs = TestGen.SqlLite.BL.BL_Customer.Select(db);

                    for (int i = 0; i < Math.Min(recs.Count, 10); i++)
                    {
                        recs[i].CustomerName = "Update " + (i + 1);
                        TestGen.SqlLite.BL.BL_Customer.Update(db, recs[i], EnumSaveMode.Requery);
                        recs[i].ToString();
                    }
                }

                #endregion  Update

                db.ToString();

            }
        }

        static void TestOracle(EnumTestMode test = EnumTestMode.Default, StreamWriter stream = null)
        {
            string cs = "Data Source=orcl;User ID=Speed;Password=manager10";
            var db = new Database(EnumDbProviderType.Oracle, cs);

            db.Open();

            if (test == EnumTestMode.Default)
                test = EnumTestMode.Performance;

            #region DataClass
            /*
                if (test == EnumTestMode.DataClasss)
                {
                    try
                    {
                    TestGen.Oracle.BL.BL_Customer.Delete(db);

                    var rec1 = new TestGen.Oracle.Data.Customer { CustomerName = "A" };

                    DataClassCustomer dc = new DataClassCustomer();
                    dc.Insert(db, rec1, EnumSaveMode.None);

                    rec1.CustomerName = "B";
                    dc.Insert(db, rec1, EnumSaveMode.Requery);

                    rec1.CustomerName = "C";
                    dc.Insert(db, rec1, EnumSaveMode.Requery);

                    rec1.CustomerName = "D";
                    TestGen.Oracle.BL.BL_Customer.Insert(db, rec1, EnumSaveMode.Requery);
                    
                    var rec1s = TestGen.Oracle.BL.BL_Customer.Select(db);

                    dc.Update(db, rec1s[0], EnumSaveMode.Requery);
                    
                    rec1s.ToString();
                        ShowInformation(tc1.ToString());
                    }
                    catch (Exception ex)
                    {
                        Program.ShowError(ex);
                    }
                }
                */

            #endregion DataClass

            #region Performance

            if (test == EnumTestMode.Performance)
            {
                int count = RecordCount;
                TestGen.Oracle.BL.BL_Customer.Delete(db);

                try
                {
                    if (useTran)
                        db.BeginTransaction();

                    TimerCount tc1 = new TimerCount("Oracle - Count: " + count);

                    tc1.Next("Insert EnumSaveMode.Requery " + count); // RefCursor is 3x faster
                    for (int i = count + 1; i <= 2 * count + 1; i++)
                    {
                        var rec = new TestGen.Oracle.Data.Customer { CustomerName = "Name " + i };
                        TestGen.Oracle.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                    }

                    tc1.Next("Insert EnumSaveMode.None " + count);
                    for (int i = 1; i <= count; i++)
                    {
                        var rec = new TestGen.Oracle.Data.Customer { CustomerName = "Name " + i };
                        TestGen.Oracle.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    }

                    tc1.Next("Select " + 2 * count);
                    var recs = TestGen.Oracle.BL.BL_Customer.Select(db);

                    tc1.Next("Update EnumSaveMode.None " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.Oracle.BL.BL_Customer.Update(db, rec, EnumSaveMode.None);
                    }

                    tc1.Next("Update EnumSaveMode.Requery " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.Oracle.BL.BL_Customer.Update(db, rec, EnumSaveMode.Requery);
                    }

                    tc1.Next("Delete " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.Oracle.BL.BL_Customer.Delete(db, rec);
                    }

                    if (useTran)
                        db.Commit();


                    if (stream == null)
                        ShowInformation(tc1.ToString());
                    else
                        stream.WriteLine(tc1.ToString());
                }
                catch (Exception ex)
                {
                    Program.ShowError(ex);
                }
            }

            #endregion Performance

            #region Metadata

            else if (test == EnumTestMode.Metadata)
            {
                var tb1 = db.GetSchema("Tables");
                var tb2 = db.GetSchema("ForeignKeys");
                var tbcs = db.GetSchema("Columns").GetText();

                //var tb = db.ExecuteDataTable("PRAGMA table_info()");

                string txt = db.GetAllMetadata();
                File.WriteAllText("../../Metadata/Oracle.txt", txt);
                txt.ToString();
            }

            #endregion Metadata

            #region Generate

            else if (test == EnumTestMode.Generate)
            {
                TimerCount tc = new TimerCount("Compilation"); // 0.35s
                db.GenerateTables("TestGen.Oracle.Data", "TestGen.Oracle.BL", "../../../TestGen.Oracle/Data/Base", "../../../TestGen.Oracle/Data", "../../../TestGen.Oracle/BL/Base", "../../../TestGen.Oracle/BL", false, Speed.Data.Generation.EnumNameCase.Pascal);
                ShowInformation(tc.ToString());
            }

            #endregion Generate

            #region Select

            else if (test == EnumTestMode.Select)
            {
                TimerCount tc = new TimerCount("Compilation"); // 0.94s

                var recs = TestGen.Oracle.BL.BL_Customer.Select(db, "Customer_Id=-1"); // first call compile
                tc.Next("Count");
                long count = TestGen.Oracle.BL.BL_Customer.Count(db);
                tc.Next("Select " + count);
                recs = TestGen.Oracle.BL.BL_Customer.Select(db); // 100.000 = 0.17s

                string time = tc.ToString();
                ShowInformation(time);
            }

            #endregion Select

            #region  Insert

            else if (test == EnumTestMode.Insert)
            {
                var recs = TestGen.Oracle.BL.BL_Customer.Select(db, "Customer_Id=-1"); // first call compile

                TestGen.Oracle.BL.BL_Customer.Delete(db);

                int count = RecordCount;
                var tc = new TimerCount("Insert " + count);

                db.BeginTransaction(); // 2 x faster  -- 100.000 = 28.48s
                for (int i = 0; i < count; i++)
                {
                    TestGen.Oracle.Data.Customer rec = new TestGen.Oracle.Data.Customer();
                    rec.CustomerId = i;
                    rec.CustomerName = "Name " + (i + 1);
                    // Acc.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                    TestGen.Oracle.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                }
                db.Commit();

                string time = tc.ToString();
                ShowInformation(time);

                //tc.Next("SQL");
                //for (int i = 0; i < count; i++)
                //{
                //    string sql = string.Format("insert into customer(CustomerName) values({0})", Conv.ToSqlTextA("Name " + (i + 1)));
                //    db.ExecuteNonQuery(sql);
                //}

                //string time = tc.ToString();
                //ShowInformation(time);
                //recs = TestGen.Oracle.BL.BL_Customer.Select(db);
                //recs.ToString();
            }

            #endregion  Insert

            #region  Update

            else if (test == EnumTestMode.Update)
            {
                var recs = TestGen.Oracle.BL.BL_Customer.Select(db);

                for (int i = 0; i < Math.Min(recs.Count, 10); i++)
                {
                    recs[i].CustomerName = "Update " + (i + 1);
                    TestGen.Oracle.BL.BL_Customer.Update(db, recs[i], EnumSaveMode.Requery);
                    recs[i].ToString();
                }
            }

            #endregion  Update

            db.ToString();
        }

        static void TestMySql(EnumTestMode test = EnumTestMode.Default, StreamWriter stream = null)
        {
            string cs = Database.BuildConnectionString(EnumDbProviderType.MySql, "localhost", "Speed", "root", "manager").ConnectionString;

            var db = new Database(EnumDbProviderType.MySql, cs);

            db.Open();

            if (test == EnumTestMode.Default)
                test = EnumTestMode.Performance;

            #region DataClass

            if (test == EnumTestMode.DataClass)
            {
                try
                {
                    /*
                    TestGen.MySql.BL.BL_Customer.Delete(db);

                    var rec1 = new TestGen.MySql.Data.Customer { CustomerName = "A" };

                    DataClassCustomer dc = new DataClassCustomer();
                    dc.Insert(db, rec1, EnumSaveMode.None);

                    rec1.CustomerName = "B";
                    dc.Insert(db, rec1, EnumSaveMode.Requery);

                    rec1.CustomerName = "C";
                    dc.Insert(db, rec1, EnumSaveMode.Requery);

                    rec1.CustomerName = "D";
                    TestGen.MySql.BL.BL_Customer.Insert(db, rec1, EnumSaveMode.Requery);

                    var rec1s = TestGen.MySql.BL.BL_Customer.Select(db);

                    dc.Update(db, rec1s[0], EnumSaveMode.Requery);

                    rec1s.ToString();
                    */
                }
                catch (Exception ex)
                {
                    Program.ShowError(ex);
                }
            }

            #endregion DataClass

            #region Performance

            if (test == EnumTestMode.Performance)
            {
                int count = RecordCount;
                TestGen.MySql.BL.BL_Customer.Delete(db);

                try
                {
                    TimerCount tc1 = new TimerCount("MySql - Count: " + count);

                    if (useTran)
                        db.BeginTransaction();

                    tc1.Next("Insert EnumSaveMode.Requery " + count); // RefCursor is 3x faster
                    for (int i = count + 1; i <= 2 * count + 1; i++)
                    {
                        var rec = new TestGen.MySql.Data.Customer { CustomerName = "Name " + i };
                        TestGen.MySql.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                    }

                    tc1.Next("Insert EnumSaveMode.None " + count);
                    for (int i = 1; i <= count; i++)
                    {
                        var rec = new TestGen.MySql.Data.Customer { CustomerName = "Name " + i };
                        TestGen.MySql.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    }

                    tc1.Next("Select " + 2 * count);
                    var recs = TestGen.MySql.BL.BL_Customer.Select(db);

                    tc1.Next("Update EnumSaveMode.None " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.MySql.BL.BL_Customer.Update(db, rec, EnumSaveMode.None);
                    }

                    tc1.Next("Update EnumSaveMode.Requery " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.MySql.BL.BL_Customer.Update(db, rec, EnumSaveMode.Requery);
                    }

                    tc1.Next("Delete " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.MySql.BL.BL_Customer.Delete(db, rec);
                    }

                    if (useTran)
                        db.Commit();

                    if (stream == null)
                        ShowInformation(tc1.ToString());
                    else
                        stream.WriteLine(tc1.ToString());
                }
                catch (Exception ex)
                {
                    Program.ShowError(ex);
                }
            }

            #endregion Performance

            #region Metadata

            else if (test == EnumTestMode.Metadata)
            {
                var tb1 = db.GetSchema("Tables");
                var tb2 = db.GetSchema("ForeignKeys");
                var tbcs = db.GetSchema("Columns").GetText();

                //var tb = db.ExecuteDataTable("PRAGMA table_info()");

                string txt = db.GetAllMetadata();
                File.WriteAllText("../../Metadata/MySql.txt", txt);
                txt.ToString();
            }

            #endregion Metadata

            #region Generate

            else if (test == EnumTestMode.Generate)
            {
                TimerCount tc = new TimerCount("Compilation"); // 0.35s
                db.GenerateTables("TestGen.MySql.Data", "TestGen.MySql.BL", "../../../TestGen.MySql/Data/Base", "../../../TestGen.MySql/Data", "../../../TestGen.MySql/BL/Base", "../../../TestGen.MySql/BL", false, Speed.Data.Generation.EnumNameCase.Pascal);
                ShowInformation(tc.ToString());
            }

            #endregion Generate

            #region Select

            else if (test == EnumTestMode.Select)
            {
                TimerCount tc = new TimerCount("Compilation"); // 0.94s

                var recs = TestGen.MySql.BL.BL_Customer.Select(db, "Customer_Id=-1"); // first call compile
                tc.Next("Count");
                long count = TestGen.MySql.BL.BL_Customer.Count(db);
                tc.Next("Select " + count);
                recs = TestGen.MySql.BL.BL_Customer.Select(db); // 100.000 = 0.17s

                string time = tc.ToString();
                ShowInformation(time);
            }

            #endregion Select

            #region  Insert

            else if (test == EnumTestMode.Insert)
            {
                var recs = TestGen.MySql.BL.BL_Customer.Select(db, "Customer_Id=-1"); // first call compile

                TestGen.MySql.BL.BL_Customer.Delete(db);

                int count = RecordCount;
                var tc = new TimerCount("Insert " + count);

                db.BeginTransaction(); // 2 x faster  -- 100.000 = 28.48s
                for (int i = 0; i < count; i++)
                {
                    TestGen.MySql.Data.Customer rec = new TestGen.MySql.Data.Customer();
                    rec.CustomerId = i;
                    rec.CustomerName = "Name " + (i + 1);
                    // Acc.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                    TestGen.MySql.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                }
                db.Commit();

                string time = tc.ToString();
                ShowInformation(time);

                //tc.Next("SQL");
                //for (int i = 0; i < count; i++)
                //{
                //    string sql = string.Format("insert into customer(CustomerName) values({0})", Conv.ToSqlTextA("Name " + (i + 1)));
                //    db.ExecuteNonQuery(sql);
                //}

                //string time = tc.ToString();
                //ShowInformation(time);
                //recs = TestGen.MySql.BL.BL_Customer.Select(db);
                //recs.ToString();
            }

            #endregion  Insert

            #region  Update

            else if (test == EnumTestMode.Update)
            {
                var recs = TestGen.MySql.BL.BL_Customer.Select(db);

                for (int i = 0; i < Math.Min(recs.Count, 10); i++)
                {
                    recs[i].CustomerName = "Update " + (i + 1);
                    TestGen.MySql.BL.BL_Customer.Update(db, recs[i], EnumSaveMode.Requery);
                    recs[i].ToString();
                }
            }

            #endregion  Update

            db.ToString();
        }

        static void TestMariaDB(EnumTestMode test = EnumTestMode.Default, StreamWriter stream = null)
        {
            string cs = Database.BuildConnectionString(EnumDbProviderType.MariaDB, "localhost", "Speed", "root", "manager", false, 3307).ConnectionString;

            var db = new Database(EnumDbProviderType.MariaDB, cs);

            db.Open();

            if (test == EnumTestMode.Default)
                test = EnumTestMode.Performance;

            #region DataClass

            if (test == EnumTestMode.DataClass)
            {
                try
                {
                    /*
                    TestGen.MariaDB.BL.BL_Customer.Delete(db);

                    var rec1 = new TestGen.MariaDB.Data.Customer { CustomerName = "A" };

                    DataClassCustomer dc = new DataClassCustomer();
                    dc.Insert(db, rec1, EnumSaveMode.None);

                    rec1.CustomerName = "B";
                    dc.Insert(db, rec1, EnumSaveMode.Requery);

                    rec1.CustomerName = "C";
                    dc.Insert(db, rec1, EnumSaveMode.Requery);

                    rec1.CustomerName = "D";
                    TestGen.MariaDB.BL.BL_Customer.Insert(db, rec1, EnumSaveMode.Requery);

                    var rec1s = TestGen.MariaDB.BL.BL_Customer.Select(db);

                    dc.Update(db, rec1s[0], EnumSaveMode.Requery);

                    rec1s.ToString();
                    */
                }
                catch (Exception ex)
                {
                    Program.ShowError(ex);
                }
            }

            #endregion DataClass

            #region Performance

            if (test == EnumTestMode.Performance)
            {
                int count = RecordCount;
                TestGen.MariaDB.BL.BL_Customer.Delete(db);

                try
                {
                    TimerCount tc1 = new TimerCount("MariaDB - Count: " + count);

                    if (useTran)
                        db.BeginTransaction();

                    tc1.Next("Insert EnumSaveMode.Requery " + count); // RefCursor is 3x faster
                    for (int i = count + 1; i <= 2 * count + 1; i++)
                    {
                        var rec = new TestGen.MariaDB.Data.Customer { CustomerName = "Name " + i };
                        TestGen.MariaDB.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                    }

                    tc1.Next("Insert EnumSaveMode.None " + count);
                    for (int i = 1; i <= count; i++)
                    {
                        var rec = new TestGen.MariaDB.Data.Customer { CustomerName = "Name " + i };
                        TestGen.MariaDB.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    }

                    tc1.Next("Select " + 2 * count);
                    var recs = TestGen.MariaDB.BL.BL_Customer.Select(db);

                    tc1.Next("Update EnumSaveMode.None " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.MariaDB.BL.BL_Customer.Update(db, rec, EnumSaveMode.None);
                    }

                    tc1.Next("Update EnumSaveMode.Requery " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.MariaDB.BL.BL_Customer.Update(db, rec, EnumSaveMode.Requery);
                    }

                    tc1.Next("Delete " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.MariaDB.BL.BL_Customer.Delete(db, rec);
                    }

                    if (useTran)
                        db.Commit();

                    if (stream == null)
                        ShowInformation(tc1.ToString());
                    else
                        stream.WriteLine(tc1.ToString());
                }
                catch (Exception ex)
                {
                    Program.ShowError(ex);
                }
            }

            #endregion Performance

            #region Metadata

            else if (test == EnumTestMode.Metadata)
            {
                var tb1 = db.GetSchema("Tables");
                var tb2 = db.GetSchema("ForeignKeys");
                var tbcs = db.GetSchema("Columns").GetText();

                //var tb = db.ExecuteDataTable("PRAGMA table_info()");

                string txt = db.GetAllMetadata();
                File.WriteAllText("../../Metadata/MariaDB.txt", txt);
                txt.ToString();
            }

            #endregion Metadata

            #region Generate

            else if (test == EnumTestMode.Generate)
            {
                TimerCount tc = new TimerCount("Compilation"); // 0.35s
                db.GenerateTables("TestGen.MariaDB.Data", "TestGen.MariaDB.BL", "../../../TestGen.MariaDB/Data/Base", "../../../TestGen.MariaDB/Data", "../../../TestGen.MariaDB/BL/Base", "../../../TestGen.MariaDB/BL", false, Speed.Data.Generation.EnumNameCase.Pascal);
                ShowInformation(tc.ToString());
            }

            #endregion Generate

            #region Select

            else if (test == EnumTestMode.Select)
            {
                TimerCount tc = new TimerCount("Compilation"); // 0.94s

                var recs = TestGen.MariaDB.BL.BL_Customer.Select(db, "Customer_Id=-1"); // first call compile
                tc.Next("Count");
                long count = TestGen.MariaDB.BL.BL_Customer.Count(db);
                tc.Next("Select " + count);
                recs = TestGen.MariaDB.BL.BL_Customer.Select(db); // 100.000 = 0.17s

                string time = tc.ToString();
                ShowInformation(time);
            }

            #endregion Select

            #region  Insert

            else if (test == EnumTestMode.Insert)
            {
                var recs = TestGen.MariaDB.BL.BL_Customer.Select(db, "Customer_Id=-1"); // first call compile

                TestGen.MariaDB.BL.BL_Customer.Delete(db);

                int count = RecordCount;
                var tc = new TimerCount("Insert " + count);

                db.BeginTransaction(); // 2 x faster  -- 100.000 = 28.48s
                for (int i = 0; i < count; i++)
                {
                    TestGen.MariaDB.Data.Customer rec = new TestGen.MariaDB.Data.Customer();
                    rec.CustomerId = i;
                    rec.CustomerName = "Name " + (i + 1);
                    // Acc.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                    TestGen.MariaDB.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                }
                db.Commit();

                string time = tc.ToString();
                ShowInformation(time);

                //tc.Next("SQL");
                //for (int i = 0; i < count; i++)
                //{
                //    string sql = string.Format("insert into customer(CustomerName) values({0})", Conv.ToSqlTextA("Name " + (i + 1)));
                //    db.ExecuteNonQuery(sql);
                //}

                //string time = tc.ToString();
                //ShowInformation(time);
                //recs = TestGen.MariaDB.BL.BL_Customer.Select(db);
                //recs.ToString();
            }

            #endregion  Insert

            #region  Update

            else if (test == EnumTestMode.Update)
            {
                var recs = TestGen.MariaDB.BL.BL_Customer.Select(db);

                for (int i = 0; i < Math.Min(recs.Count, 10); i++)
                {
                    recs[i].CustomerName = "Update " + (i + 1);
                    TestGen.MariaDB.BL.BL_Customer.Update(db, recs[i], EnumSaveMode.Requery);
                    recs[i].ToString();
                }
            }

            #endregion  Update

            db.ToString();
        }

        static void TestFirebird(EnumTestMode test = EnumTestMode.Default, StreamWriter stream = null)
        {
            Database db;
            //string cs = Database.BuildConnectionString(EnumDbProviderType.Firebird, "localhost", "Speed", "SYSDBA", "masterkey").ConnectionString;

            // string dbFile = @"E:\_Codeplex\_Systems\Speed\Testes\TestGen.Firebird\TestFirebird.fdb";
            string dbFile = GetLocalFile("Test.fb");
            //db = new Database(EnumDbProviderType.Firebird, null, dbFile, "SYSDBA", "masterkey", 30, false, 0, false);
            //db = new Database(EnumDbProviderType.Firebird, null, dbFile, "SYSDBA", "masterkey", 30, false, 0, true);
            //db = new Database(EnumDbProviderType.Firebird, null, dbFile, "SYSDBA", "masterkey", 30, false, 0, true);

            db = new Database(EnumDbProviderType.Firebird, @"initial catalog=" + dbFile + ";user id=SYSDBA;password=masterkey;pooling=True;server type=Embedded");

            db.Open();

            if (test == EnumTestMode.Default)
                test = EnumTestMode.Performance;

            #region DataClass

            if (test == EnumTestMode.DataClass)
            {
                try
                {
                    TestGen.Firebird.BL.BL_Customer.Delete(db);

                    var rec1 = new TestGen.Firebird.Data.Customer { CustomerName = "A" };

                    //DataClassCustomer dc = new DataClassCustomer();
                    //dc.Insert(db, rec1, EnumSaveMode.None);

                    //rec1.CustomerName = "B";
                    //dc.Insert(db, rec1, EnumSaveMode.Requery);

                    //rec1.CustomerName = "C";
                    //dc.Insert(db, rec1, EnumSaveMode.Requery);

                    //rec1.CustomerName = "D";
                    //TestGen.Firebird.BL.BL_Customer.Insert(db, rec1, EnumSaveMode.Requery);

                    //var rec1s = TestGen.Firebird.BL.BL_Customer.Select(db);

                    //rec1s[0].CustomerName = "X";
                    //dc.Update(db, rec1s[0], EnumSaveMode.Requery);

                    //rec1s.ToString();
                }
                catch (Exception ex)
                {
                    Program.ShowError(ex);
                }
            }

            #endregion DataClass

            #region Performance

            if (test == EnumTestMode.Performance)
            {
                int count = RecordCount;
                TestGen.Firebird.BL.BL_Customer.Delete(db);

                try
                {
                    TimerCount tc1 = new TimerCount("Firebird - Count: " + count);

                    if (useTran)
                        db.BeginTransaction();

                    tc1.Next("Insert EnumSaveMode.Requery " + count); // RefCursor is 3x faster
                    for (int i = count + 1; i <= 2 * count + 1; i++)
                    {
                        var rec = new TestGen.Firebird.Data.Customer { CustomerName = "Name " + i };
                        TestGen.Firebird.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                    }

                    tc1.Next("Insert EnumSaveMode.None " + count);
                    for (int i = 1; i <= count; i++)
                    {
                        var rec = new TestGen.Firebird.Data.Customer { CustomerName = "Name " + i };
                        TestGen.Firebird.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    }

                    tc1.Next("Select " + 2 * count);
                    var recs = TestGen.Firebird.BL.BL_Customer.Select(db);

                    tc1.Next("Update EnumSaveMode.None " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.Firebird.BL.BL_Customer.Update(db, rec, EnumSaveMode.None);
                    }

                    tc1.Next("Update EnumSaveMode.Requery " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.Firebird.BL.BL_Customer.Update(db, rec, EnumSaveMode.Requery);
                    }

                    tc1.Next("Delete " + 2 * count);
                    foreach (var rec in recs)
                    {
                        TestGen.Firebird.BL.BL_Customer.Delete(db, rec);
                    }

                    if (useTran)
                        db.Commit();

                    if (stream == null)
                        ShowInformation(tc1.ToString());
                    else
                        stream.WriteLine(tc1.ToString());
                }
                catch (Exception ex)
                {
                    Program.ShowError(ex);
                }
            }

            #endregion Performance

            #region Metadata

            else if (test == EnumTestMode.Metadata)
            {
                var tb1 = db.GetSchema("Tables");
                var tb2 = db.GetSchema("ForeignKeys");
                var tbcs = db.GetSchema("Columns").GetText();

                //var tb = db.ExecuteDataTable("PRAGMA table_info()");

                string txt = db.GetAllMetadata();
                File.WriteAllText("../../Metadata/Firebird.txt", txt);
                txt.ToString();
            }

            #endregion Metadata

            #region Generate

            else if (test == EnumTestMode.Generate)
            {
                TimerCount tc = new TimerCount("Compilation"); // 0.35s
                db.GenerateTables("TestGen.Firebird.Data", "TestGen.Firebird.BL", "../../../TestGen.Firebird/Data/Base", "../../../TestGen.Firebird/Data", "../../../TestGen.Firebird/BL/Base", "../../../TestGen.Firebird/BL", false, Speed.Data.Generation.EnumNameCase.Pascal);
                ShowInformation(tc.ToString());
            }

            #endregion Generate

            #region Select

            else if (test == EnumTestMode.Select)
            {
                TimerCount tc = new TimerCount("Compilation"); // 0.94s

                var recs = TestGen.Firebird.BL.BL_Customer.Select(db, "Customer_Id=-1"); // first call compile
                tc.Next("Count");
                long count = TestGen.Firebird.BL.BL_Customer.Count(db);
                tc.Next("Select " + count);
                recs = TestGen.Firebird.BL.BL_Customer.Select(db); // 100.000 = 0.17s

                string time = tc.ToString();
                ShowInformation(time);
            }

            #endregion Select

            #region  Insert

            else if (test == EnumTestMode.Insert)
            {
                var recs = TestGen.Firebird.BL.BL_Customer.Select(db, "Customer_Id=-1"); // first call compile

                TestGen.Firebird.BL.BL_Customer.Delete(db);

                int count = RecordCount;
                var tc = new TimerCount("Insert " + count);

                db.BeginTransaction(); // 2 x faster  -- 100.000 = 28.48s
                for (int i = 0; i < count; i++)
                {
                    TestGen.Firebird.Data.Customer rec = new TestGen.Firebird.Data.Customer();
                    rec.CustomerId = i;
                    rec.CustomerName = "Name " + (i + 1);
                    // Acc.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                    TestGen.Firebird.BL.BL_Customer.Insert(db, rec, EnumSaveMode.None);
                    TestGen.Firebird.BL.BL_Customer.Insert(db, rec, EnumSaveMode.Requery);
                }
                db.Commit();

                string time = tc.ToString();
                ShowInformation(time);

                //tc.Next("SQL");
                //for (int i = 0; i < count; i++)
                //{
                //    string sql = string.Format("insert into customer(CustomerName) values({0})", Conv.ToSqlTextA("Name " + (i + 1)));
                //    db.ExecuteNonQuery(sql);
                //}

                //string time = tc.ToString();
                //ShowInformation(time);
                //recs = TestGen.Firebird.BL.BL_Customer.Select(db);
                //recs.ToString();
            }

            #endregion  Insert

            #region  Update

            else if (test == EnumTestMode.Update)
            {
                var recs = TestGen.Firebird.BL.BL_Customer.Select(db);

                for (int i = 0; i < Math.Min(recs.Count, 10); i++)
                {
                    recs[i].CustomerName = "Update " + (i + 1);
                    TestGen.Firebird.BL.BL_Customer.Update(db, recs[i], EnumSaveMode.Requery);
                    recs[i].ToString();
                }
            }

            #endregion  Update

            db.ToString();
        }

        static void GenerateSqlServer()
        {
            using (var db = NewDatabase())
            {
                db.GenerateTables("TestGen.SqlServer.Data", "TestGen.SqlServer.BL", "../../../TestGen.SqlServer/Data/Base", "../../../TestGen.SqlServer/Data", "../../../TestGen.SqlServer/BL/Base", "../../../TestGen.SqlServer/BL", false, Speed.Data.Generation.EnumNameCase.None);
            }
        }

        #endregion Working Tests

        #region Procedures

        static void GenProcedures()
        {
            using (var db = new Database(EnumDbProviderType.Oracle, "Oracle", null, "raizen", "manager"))
            {
                db.Open();
                var routines = db.Routines;
                db.ToString();
                string sql =
@"
SET NO_BROWSETABLE ON; 
SET FMTONLY ON;
exec [dbo].[GetPolicy] @ItemName = null, @AuthType = null
SET FMTONLY ON;
";

                var tb = db.ExecuteDataTable(sql);

                db.ToString();


            }
        }

        #endregion Procedures

    }

}

