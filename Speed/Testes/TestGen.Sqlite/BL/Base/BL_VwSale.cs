// ****** SPEED ******
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Speed.Data;
using TestGen.SqlLite.Data;

namespace TestGen.SqlLite.BL
{

    //[System.Diagnostics.DebuggerStepThrough]
    public partial class BL_VwSale : BLClass<TestGen.SqlLite.Data.VwSale>
    {

        public static TestGen.SqlLite.Data.VwSale SelectByPk(Database db, Int64 _SaleId)
        {
            return db.SelectSingle<TestGen.SqlLite.Data.VwSale>(string.Format("SaleId={0}", _SaleId));
        }

        public static int DeleteByPk(Database db, Int64 _SaleId)
        {
            return db.Delete<TestGen.SqlLite.Data.VwSale>(string.Format("SaleId={0}", _SaleId));
        }



    }

}