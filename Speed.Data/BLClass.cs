using System.Collections.Generic;

namespace Speed.Data
{

    /// <summary>
    /// Classe template pra usar na BL
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //#if !DEBUG
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    //#endif
    public class BLClass<T> where T : Record
    {

        public static long Count(Database db)
        {
            return db.Count<T>();
        }
        public static long Count()
        {
            using (var db = Sys.NewDb())
                return db.Count<T>();
        }

        public static long Count(Database db, string where)
        {
            return db.Count<T>(where);
        }
        public static long Count(string where)
        {
            using (var db = Sys.NewDb())
                return db.Count<T>(where);
        }

        public static long Count(Database db, T filter, EnumDbFilter mode = EnumDbFilter.AndLike, int commandTimeout = 30)
        {
            return db.Count<T>(filter, mode, commandTimeout);
        }
        public static long Count(T filter, EnumDbFilter mode = EnumDbFilter.AndLike, int commandTimeout = 30)
        {
            using (var db = Sys.NewDb())
                return db.Count<T>(filter, mode, commandTimeout);
        }

        public static long Max(Database db, string colunName)
        {
            return db.Count<T>();
        }
        public static long Max(string colunName)
        {
            using (var db = Sys.NewDb())
                return db.Count<T>();
        }

        public static long Max(Database db, string colunName, string where)
        {
            return db.Count<T>(where);
        }
        public static long Max(string colunName, string where)
        {
            using (var db = Sys.NewDb())
                return db.Count<T>(where);
        }

        public static List<T> Select()
        {
            using (var db = Sys.NewDb())
                return db.Select<T>();
        }

        public static List<T> Select(Database db)
        {
            return db.Select<T>();
        }
        public static List<T> Select<T>(T filter, EnumDbFilter mode = EnumDbFilter.AndLike, bool concurrency = false, int commandTimeout = 30)
        {
            using (var db = Sys.NewDb())
                return db.Select<T>(db, filter, mode, concurrency, commandTimeout);
        }
        public static List<T> Select<T>(Database db, T filter, EnumDbFilter mode = EnumDbFilter.AndLike, bool concurrency = false, int commandTimeout = 30)
        {
            return db.Select<T>(db, filter, mode, concurrency, commandTimeout);
        }

        public static List<T> Select(Database db, object filter, EnumDbFilter mode = EnumDbFilter.AndLike, bool concurrency = false, int commandTimeout = 30)
        {
            return db.Select<T>(db, filter, mode, concurrency, commandTimeout) as List<T>;
        }

        public static List<T> Select(Database db, string where)
        {
            return db.Select<T>(where);
        }
        public static List<T> Select(string where)
        {
            using (var db = Sys.NewDb())
                return db.Select<T>(where);
        }

        public static List<T> Select(Database db, string where, params object[] args)
        {
            return db.Select<T>(where, args);
        }
        public static List<T> Select(string where, params object[] args)
        {
            using (var db = Sys.NewDb())
                return db.Select<T>(where, args);
        }

        public static List<T> Select(Database db, string where, params Parameter[] parameters)
        {
            return db.Select<T>(where, parameters);
        }
        public static List<T> Select(string where, params Parameter[] parameters)
        {
            using (var db = Sys.NewDb())
                return db.Select<T>(where, parameters);
        }

        public static List<T> SelectPage(Database db, int start, int pageSize = 20, string sort = null)
        {
            return db.SelectPage<T>(start, pageSize, sort);
        }
        public static List<T> SelectPage(int start, int pageSize = 20, string sort = null)
        {
            using (var db = Sys.NewDb())
                return db.SelectPage<T>(start, pageSize, sort);
        }

        public static List<T> SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null)
        {
            return db.SelectPage<T>(where, start, pageSize, sort);
        }
        public static List<T> SelectPage(string where, int start, int pageSize = 20, string sort = null)
        {
            using (var db = Sys.NewDb())
                return db.SelectPage<T>(where, start, pageSize, sort);
        }

        public static List<T> SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, params object[] args)
        {
            return db.SelectPage<T>(where, start, pageSize, sort, args);
        }
        public static List<T> SelectPage(string where, int start, int pageSize = 20, string sort = null, params object[] args)
        {
            using (var db = Sys.NewDb())
                return db.SelectPage<T>(where, start, pageSize, sort, args);
        }

        public static List<T> SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, params Parameter[] parameters)
        {
            return db.SelectPage<T>(where, start, pageSize, sort, parameters);
        }
        public static List<T> SelectPage(string where, int start, int pageSize = 20, string sort = null, params Parameter[] parameters)
        {
            using (var db = Sys.NewDb())
                return db.SelectPage<T>(where, start, pageSize, sort, parameters);
        }

        public static List<T> SelectPage(Database db, T filter, int start, int pageSize = 20, string sort = null, EnumDbFilter mode = EnumDbFilter.AndLike, int commandTimeout = 30, bool concurrency = false, params Parameter[] parameters)
        {
            return db.SelectPage<T>(filter, start, pageSize, sort, mode, commandTimeout, concurrency, parameters);
        }

        public static List<T> SelectPage(T filter, int start, int pageSize = 20, string sort = null, EnumDbFilter mode = EnumDbFilter.AndLike, int commandTimeout = 30, bool concurrency = false, params Parameter[] parameters)
        {
            using (var db = Sys.NewDb())
                return db.SelectPage<T>(filter, start, pageSize, sort, mode, commandTimeout, concurrency, parameters);
        }

        public static T SelectSingle(Database db, string where)
        {
            return db.SelectSingle<T>(where);
        }
        public static T SelectSingle(string where)
        {
            using (var db = Sys.NewDb())
                return db.SelectSingle<T>(where);
        }

        public static T SelectSingle(Database db, string where, params object[] args)
        {
            return db.SelectSingle<T>(where, args);
        }
        public static T SelectSingle(string where, params object[] args)
        {
            using (var db = Sys.NewDb())
                return db.SelectSingle<T>(where, args);
        }

        public static T SelectSingle(Database db, string where, params Parameter[] parameters)
        {
            return db.SelectSingle<T>(where, parameters);
        }
        public static T SelectSingle(string where, params Parameter[] parameters)
        {
            using (var db = Sys.NewDb())
                return db.SelectSingle<T>(where, parameters);
        }
        public static T SelectSingle<T>(Database db, T filter, EnumDbFilter mode = EnumDbFilter.AndLike, bool concurrency = false, int commandTimeout = 30)
        {
            return db.SelectSingle<T>(db, filter, mode, concurrency, commandTimeout);
        }
        public static T SelectSingle<T>(T filter, EnumDbFilter mode = EnumDbFilter.AndLike, bool concurrency = false, int commandTimeout = 30)
        {
            using (var db = Sys.NewDb())
                return db.SelectSingle<T>(db, filter, mode, concurrency, commandTimeout);
        }




        public static T SelectByPk(Database db, T rec)
        {
            return db.SelectByPK<T>(rec);
        }
        public static T SelectByPk(T rec)
        {
            using (var db = Sys.NewDb())
                return db.SelectByPK<T>(rec);
        }

        public static void Save(Database db, T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            db.Save<T>(rec, saveMode);
        }
        public static void Save(T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            using (var db = Sys.NewDb())
                db.Save<T>(rec, saveMode);
        }

        public static void Insert(Database db, T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            db.Insert<T>(rec, saveMode);
        }
        public static void Insert(T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            using (var db = Sys.NewDb())
                db.Insert<T>(rec, saveMode);
        }

        public static void InsertXml(Database db, List<T> recs, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            db.InsertXml<T>(recs, saveMode);
        }
        public static void InsertXml(List<T> recs, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            using (var db = Sys.NewDb())
                db.InsertXml<T>(recs, saveMode);
        }

        public static void Update(Database db, T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            db.Update<T>(rec, saveMode);
        }
        public static void Update(T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            using (var db = Sys.NewDb())
                db.Update<T>(rec, saveMode);
        }

        public static void SaveList(Database db, List<T> recs, EnumSaveMode mode = EnumSaveMode.None, bool continueOnError = false)
        {
            db.SaveList<T>(recs, mode, continueOnError);
        }
        public static void SaveList(List<T> recs, EnumSaveMode mode = EnumSaveMode.None, bool continueOnError = false)
        {
            using (var db = Sys.NewDb())
                db.SaveList<T>(recs, mode, continueOnError);
        }

        public static void Truncate(Database db)
        {
            db.Truncate<T>();
        }
        public static void Truncate()
        {
            using (var db = Sys.NewDb())
                db.Truncate<T>();
        }

        public static void Delete(Database db)
        {
            db.Delete<T>();
        }
        public static void Delete()
        {
            using (var db = Sys.NewDb())
                db.Delete<T>();
        }

        public static void Delete(Database db, T rec)
        {
            db.Delete<T>(rec);
        }
        public static void Delete(T rec)
        {
            using (var db = Sys.NewDb())
                db.Delete<T>(rec);
        }

        public static void Delete(Database db, string where)
        {
            db.Delete<T>(where);
        }
        public static void Delete(string where)
        {
            using (var db = Sys.NewDb())
                db.Delete<T>(where);
        }

        public static void Delete(Database db, string where, params object[] args)
        {
            db.Delete<T>(where, args);
        }
        public static void Delete(string where, params object[] args)
        {
            using (var db = Sys.NewDb())
                db.Delete<T>(where, args);
        }

        public static int DeleteByPk(T rec)
        {
            using (var db = Sys.NewDb())
                return db.DeleteByPk<T>(rec);
        }

        public static void DeleteList(Database db, List<T> recs)
        {
            foreach (var rec in recs)
                db.Delete<T>(rec);
        }
        public static void DeleteList(List<T> recs)
        {
            using (var db = Sys.NewDb())
            {
                foreach (var rec in recs)
                    db.Delete<T>(rec);
            }
        }

        /// <summary>
        /// Faz um Conv.ToSqlTextA.
        /// Conv.ToSqlTextA já trata a aspas simples no meio da string e contatena aspas simples no início e final.
        /// Isso evita Sql Injection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Q(object value)
        {
            return Conv.ToSqlTextA(value);
        }

    }

}
