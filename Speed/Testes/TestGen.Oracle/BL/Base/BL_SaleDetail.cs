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
using TestGen.Oracle.Data;

namespace TestGen.Oracle.BL
{

    //[System.Diagnostics.DebuggerStepThrough]
    public partial class BL_SaleDetail : BLClass<TestGen.Oracle.Data.SaleDetail>
    {

        public static TestGen.Oracle.Data.SaleDetail SelectByPk(Database db, Decimal _DetailId)
        {
            return db.SelectSingle<TestGen.Oracle.Data.SaleDetail>(string.Format("DETAIL_ID={0}", _DetailId));
        }

        public static int DeleteByPk(Database db, Decimal _DetailId)
        {
            return db.Delete<TestGen.Oracle.Data.SaleDetail>(string.Format("DETAIL_ID={0}", _DetailId));
        }

        // ParentRelations
        public static TestGen.Oracle.Data.Sale Select_Parent_Sale(Database db, Decimal _SaleId)
        {
            return db.SelectSingle<Sale>(string.Format("SALE_ID={0}", _SaleId));
        }
        public static TestGen.Oracle.Data.Sale Select_Parent_Sale(Database db, TestGen.Oracle.Data.SaleDetail rec)
        {
            return Select_Parent_Sale(db, rec.DetSaleId);
        }


    }

}