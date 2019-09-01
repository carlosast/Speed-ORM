using System;
using System.Collections.Generic;
using System.Text;

namespace Speed.Data
{

    /// <summary>
    /// Classe template pra usar na BL
    /// </summary>
    /// <typeparam name="T"></typeparam>
//#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
//#endif
    public class BLClass<T> where T : Record
    {

        public static long Count(Database db)
        {
            return db.Count<T>();
        }

        public static long Count(Database db, string where)
        {
            return db.Count<T>(where);
        }

        public static List<T> Select(Database db)
        {
            return db.Select<T>();
        }

        public static List<T> Select(Database db, string where)
        {
            return db.Select<T>(where);
        }

        public static List<T> Select(Database db, string where, params object[] args)
        {
            return db.Select<T>(where, args);
        }

        public static List<T> Select(Database db, string where, params Parameter[] parameters)
        {
            return db.Select<T>(where, parameters);
        }

        public static T SelectSingle(Database db, string where)
        {
            return db.SelectSingle<T>(where);
        }

        public static T SelectSingle(Database db, string where, params object[] args)
        {
            return db.SelectSingle<T>(where, args);
        }

        public static T SelectSingle(Database db, string where, params Parameter[] parameters)
        {
            return db.SelectSingle<T>(where, parameters);
        }

        public static void Save(Database db, T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            db.Save<T>(rec, saveMode);
        }

        public static void Insert(Database db, T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            db.Insert<T>(rec, saveMode);
        }

        public static void InsertXml(Database db, List<T> recs, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            db.InsertXml<T>(recs, saveMode);
        }

        public static void Update(Database db, T rec, EnumSaveMode saveMode = EnumSaveMode.None)
        {
            db.Update<T>(rec, saveMode);
        }

        public static void SaveList(Database db, List<T> recs, EnumSaveMode mode = EnumSaveMode.None, bool continueOnError = false)
        {
            db.SaveList<T>(recs, mode, continueOnError);
        }

        public static void Truncate(Database db)
        {
            db.Truncate<T>();
        }

        public static void Delete(Database db)
        {
            db.Delete<T>();
        }

        public static void Delete(Database db, T rec)
        {
            db.Delete<T>(rec);
        }

        public static void Delete(Database db, string where)
        {
            db.Delete<T>(where);
        }

        public static void Delete(Database db, string where, params object[] args)
        {
            db.Delete<T>(where, args);
        }

        public static void DeleteList(Database db, List<T> recs)
        {
            foreach (var rec in recs)
                db.Delete<T>(rec);
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
