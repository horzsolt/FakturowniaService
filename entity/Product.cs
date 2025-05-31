using System;
using System.Text.Json.Serialization;

namespace FakturowniaService
{
    public class Product
    {
        public long? Id { get; set; }

        public long GetId()
        {
            return Id ?? 0;
        }
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Net { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Gross { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Tax { get; set; }
        public string Tax { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public bool Automatic_Sales { get; set; }
        public bool Limited { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Warehouse_Quantity { get; set; }
        public DateTime? Available_From { get; set; }
        public DateTime? Available_To { get; set; }
        public string Payment_Callback { get; set; }
        public string Payment_Url_Ok { get; set; }
        public string Payment_Url_Error { get; set; }
        public string Token { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Quantity { get; set; }
        public string Quantity_Unit { get; set; }
        public string Additional_Info { get; set; }
        public bool Disabled { get; set; }
        public bool Form_Fields_Horizontal { get; set; }
        public string Form_Fields { get; set; }
        public string Form_Name { get; set; }
        public string Form_Description { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Quantity_Sold_Outside { get; set; }
        public string Form_Kind { get; set; }
        public string Form_Template { get; set; }
        public bool Elastic_Price { get; set; }
        public long? Next_Product_Id { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Quantity_Sold_In_Invoices { get; set; }
        public bool Deleted { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public bool Ecommerce { get; set; }
        public string Period { get; set; }
        public bool Show_Elastic_Price { get; set; }
        public string Elastic_Price_Details { get; set; }
        public DateTime? Elastic_Price_Date_Trigger { get; set; }
        public long? Iid { get; set; }
        public bool Use_Formula { get; set; }
        public string Formula { get; set; }
        public string Formula_Test_Field { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Stock_Level { get; set; }
        public bool Sync { get; set; }
        public long? Category_Id { get; set; }
        public string Kind { get; set; }
        public bool Package { get; set; }
        public string Package_Product_Ids { get; set; }
        public long? Department_Id { get; set; }
        public bool Use_Product_Warehouses { get; set; }
        public bool Service { get; set; }
        public bool Use_Quantity_Discount { get; set; }
        public string Quantity_Discount_Details { get; set; }
        public bool Price_Net_On_Payment { get; set; }
        public DateTime? Warehouse_Numbers_Updated_At { get; set; }
        public string Ean_Code { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Weight { get; set; }
        public string Weight_Unit { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Size_Height { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Size_Width { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Size { get; set; }
        public string Size_Unit { get; set; }
        public long? Auto_Payment_Department_Id { get; set; }
        public int Attachments_Count { get; set; }
        public string Image_Url { get; set; }
        public string Tax2 { get; set; }
        public string Supplier_Code { get; set; }
        public string Package_Products_Details { get; set; }
        public bool Siteor_Disabled { get; set; }
        public bool Use_Moss { get; set; }
        public long? Subscription_Id { get; set; }
        public string Accounting_Id { get; set; }
        public string Status { get; set; }
        public bool Restricted_To_Warehouses { get; set; }
        //public string Gtu_Codes { get; set; }
        //public string Tag_List { get; set; }
        public string Gtu_Code { get; set; }
        public string Electronic_Service { get; set; }
        public bool? Is_Delivery { get; set; }
    }

}
