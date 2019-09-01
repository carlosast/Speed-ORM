using FluentNHibernate.Mapping;

namespace TesteGen.Hibernate

{
    public class SalesMap : ClassMap<Sales>
    {
        public SalesMap()
        {
            Table("dbo.Sales");
            Id(x => x.TestPerfID).Column("TestPerfID").Index("Id"); //.GeneratedBy.Identity();
            //Id(x => x.TestPerfID).UnsavedValue(0).GeneratedBy.Identity(); // Column("TestPerfID").GeneratedBy.Identity();
            //Map(x => x.TestPerfID).Column("TestPerfID");
            Map(x => x.CarrierTrackingNumber).Column("CarrierTrackingNumber");
            Map(x => x.OrderQty).Column("OrderQty");
            Map(x => x.ProductID).Column("ProductID");
            Map(x => x.SpecialOfferID).Column("SpecialOfferID");
            Map(x => x.UnitPrice).Column("UnitPrice");
            Map(x => x.UnitPriceDiscount).Column("UnitPriceDiscount");
            Map(x => x.LineTotal).Column("LineTotal").ReadOnly(); // computed column
            Map(x => x.Rowguid).Column("rowguid");
            Map(x => x.LongText).Column("LongText");
            Map(x => x.ModifiedDate).Column("ModifiedDate");

        }
    }
}