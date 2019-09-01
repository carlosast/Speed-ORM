using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class BussinesClass<T> where T : Record
    {

        public BussinesClass()
        {
        }

        ~BussinesClass()
        {
        }

        #region Select

        public virtual List<T> Select(Database db)
        {
            return db.Select<T>();
        }

        public virtual List<T> Select(Database db, bool updatable)
        {
            return db.Select<T>(updatable);
        }

        public virtual List<T> Select(Database db, string where)
        {
                return db.Select<T>(where);
        }

        public virtual List<T> Select(Database db, string where, bool updatable)
        {
                return db.Select<T>(where, updatable);
        }

        #endregion Select

        #region SelectSingle

        public virtual T SelectSingle(Database db)
        {
            return db.SelectSingle<T>();
        }

        public virtual T SelectSingle(Database db, bool updatable)
        {
            return db.SelectSingle<T>(updatable);
        }

        public virtual T SelectSingle(Database db, string where)
        {
            return db.SelectSingle<T>(where);
        }

        public virtual T SelectSingle(Database db, string where, bool updatable)
        {
            return db.SelectSingle<T>(where, updatable);
        }

        #endregion SelectSingle

        #region Query

        public virtual List<T> Query(Database db, string sql)
        {
            return db.Query<T>(sql);
        }

        public virtual List<T> Query(Database db, string sql, bool updatable)
        {
            return db.Query<T>(sql, updatable);
        }

        #endregion Query

        #region Insert

        public virtual int Insert(Database db, T record)
        {
            return db.Insert<T>(record);
        }

        public virtual int Insert(Database db, T record, EnumSaveMode mode)
        {
            return db.Insert<T>(record, mode);
        }

        #endregion Insert

        #region Update

        public virtual int Update(Database db, T record)
        {
            return db.Update<T>(record);
        }

        public virtual int Update(Database db, T record, EnumSaveMode mode)
        {
            return db.Update<T>(record, mode);
        }

        #endregion Update

    }

}
