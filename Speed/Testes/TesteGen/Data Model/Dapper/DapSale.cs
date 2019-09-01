using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteGen
{
    
    public class DapSale
    {

        public Int32 TestPerfID { get;set;}
        public String CarrierTrackingNumber { get;set;}
        public Int32 OrderQty { get;set;}
        public Int32 ProductID { get;set;}
        public Int32 SpecialOfferID { get;set;}
        public Decimal UnitPrice { get;set;}
        public Decimal UnitPriceDiscount { get;set;}
        public Decimal LineTotal { get;set;}
        public Guid Rowguid { get;set;}
        public String LongText { get; set; }
        public DateTime ModifiedDate { get; set; }

    }

}
