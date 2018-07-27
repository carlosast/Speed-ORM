//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Speed.Data
//{

//    public class DbSqlInfoMessage : EventArgs
//    {

//        private List<DbSqlError> errors;

//        private string message;
//        private string source;
//        private string toString;

//        public DbSqlInfoMessage(MySql.Data.MySqlClient.MySqlInfoMessageEventArgs ev)
//        {
//            this.message = null;
//            this.source = null;
//            errors = new List<DbSqlError>();
//            for (int i = 0; i < ev.errors.Length; i++)
//            {
//                MySqlError er = ev.errors[i];
//                DbSqlError er2 = new DbSqlError();
//                er2.Number = er.Code;
//                er2.Level = er.Level;
//                er2.Message = er.Message;
//                er2.toString = er.ToString();
//                errors.Add(er2);
//            }
//        }

//        public DbSqlInfoMessage(SqlInfoMessageEventArgs ev)
//        {
//            this.message = ev.Message;
//            this.source = ev.Source;
//            errors = new List<DbSqlError>();
//            for (int i = 0; i < ev.Errors.Count; i++)
//            {
//                var er = ev.Errors[i];
//                DbSqlError er2 = new DbSqlError();
//                er2.Class = er.Class;
//                er2.LineNumber = er.LineNumber;
//                er2.Message = er.Message;
//                er2.Number = er.Number;
//                er2.Procedure = er.Procedure;
//                er2.Server = er.Server;
//                er2.Source = er.Source;
//                er2.State = er.State;
//                er2.toString = er.ToString();
//                errors.Add(er2);
//            }
//        }

//        public DbSqlInfoMessage(System.Data.OleDb.OleDbInfoMessageEventArgs ev)
//        {
//            this.message = null;
//            this.source = null;
//            errors = new List<DbSqlError>();
//            for (int i = 0; i < ev.Errors.Count; i++)
//            {
//                OleDbError er = ev.Errors[i];
//                DbSqlError er2 = new DbSqlError();
//                //er2.Number = er.Code;
//                //er2.Level = er.Level;
//                er2.Message = er.Message;
//                er2.toString = er.ToString();
//                errors.Add(er2);
//            }
//        }

//        public DbSqlInfoMessage(NpgsqlNoticeEventArgs ev)
//        {
//            this.message = null;
//            this.source = null;
//            errors = new List<DbSqlError>();
//            var er = ev.Notice;
//            DbSqlError er2 = new DbSqlError();
//            er2.Number = Conv.ToInt32(er.Code);
//            er2.Level = er.Severity;
//            er2.Message = er.Message;
//            er2.toString = er.ToString();
//            errors.Add(er2);
//        }

//        public List<DbSqlError> Errors
//        {
//            get { return errors; }
//        }

//        public string Message
//        {
//            get { return message; }
//        }

//        public string Source
//        {
//            get { return source; }
//        }

//        public override string ToString()
//        {
//            return toString;
//        }

//    }

//}
