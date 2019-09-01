using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using Speed;
using Speed.Data;

namespace TestGen.Access.Data
{

    public partial class Customer
    {

        public override string ToString()
        {
            return string.Format("{0} - {1}", CustomerId, CustomerName);
        }

    }

}
