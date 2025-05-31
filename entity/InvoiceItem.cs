using System;
using System.Text.Json.Serialization;

namespace FakturowniaService
{
    public class InvoiceItem
    {
        public long Id { get; set; }
        public long Invoice_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Net { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Quantity { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Total_Price_Gross { get; set; }
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Total_Price_Net { get; set; }
        public long? Account_Id { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public string Additional_Info { get; set; }
        public string Quantity_Unit { get; set; }
        public string Tax { get; set; }
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Gross { get; set; }
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Tax { get; set; }
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Total_Price_Tax { get; set; }
        public string Kind { get; set; }
        public long? Invoice_Position_Id { get; set; }
        public long? Product_Id { get; set; }
        public bool? Deleted { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Discount_Percent { get; set; }
        public string Tax2 { get; set; }
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Exchange_Rate { get; set; }
        public string Accounting_Tax_Kind { get; set; }
        public string Code { get; set; }
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Discount_Net { get; set; }
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Lump_Sum_Tax { get; set; }
        public string CorrectedPosKind { get; set; }
        public string GtuCode { get; set; }
    }
}
