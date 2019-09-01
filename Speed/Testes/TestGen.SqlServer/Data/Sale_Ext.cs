using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using Speed;
using Speed.Data;

namespace TestGen.SqlServer.Data
{

    public partial class Sale
    {

        public override string ToString()
        {
            return string.Format("{0} - {1}", SaleId, SaleCustomerId);
        }

    }

}
