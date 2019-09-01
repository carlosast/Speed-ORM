using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using Speed;
using Speed.Data;

namespace TestGen.SqlServer.Data
{

    public partial class SaleDetail
    {

        public override string ToString()
        {
            return string.Format("{0} - {1}", DetailId, DetSaleId);
        }

    }

}
