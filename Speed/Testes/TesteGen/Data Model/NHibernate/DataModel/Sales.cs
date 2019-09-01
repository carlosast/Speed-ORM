using System;

namespace TesteGen.Hibernate
{

    public class Sales
    {
        public virtual int TestPerfID { get; set; }
        public virtual String CarrierTrackingNumber { get; set; }
        public virtual Int32 OrderQty { get; set; }
        public virtual Int32 ProductID { get; set; }
        public virtual Int32 SpecialOfferID { get; set; }
        public virtual Decimal UnitPrice { get; set; }
        public virtual Decimal UnitPriceDiscount { get; set; }
        public virtual Decimal LineTotal { get; set; }
        public virtual Guid Rowguid { get; set; }
        public virtual String LongText { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
    }

}

