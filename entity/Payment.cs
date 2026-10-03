using FakturowniaService.util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FakturowniaService
{
    public class Payment
    {
        public long? Id { get; set; }

        public long GetId()
        {
            return Id ?? 0;
        }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string Invoice_Comment { get; set; }
        public string Provider { get; set; }
        public string Provider_Title { get; set; }
        public string Provider_Status { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool? Paid { get; set; }

        [Newtonsoft.Json.JsonProperty("paid")]
        [Newtonsoft.Json.JsonConverter(typeof(PaymentPaidConverter))]
        public decimal? Paid_Amount { get; set; }
        public DateTimeOffset? Paid_Date { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price { get; set; }
        public string Currency { get; set; }
        public bool? Generate_Invoice { get; set; }
        public string Invoice_Name { get; set; }
        public string Invoice_Tax_No { get; set; }
        public string Post_Code { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public long? Invoice_Id { get; set; }
        public long? Client_Id { get; set; }
        public long? Department_Id { get; set; }
        public long? Product_Id { get; set; }
        public long? User_Id { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public string Oid { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public string Invoice_Country { get; set; }
        public string Invoice_Street { get; set; }
        public string Invoice_City { get; set; }
        public string Invoice_Post_Code { get; set; }
        public string Referrer { get; set; }

        [JsonConverter(typeof(IntegerStringConverter))]
        public int? Quantity { get; set; }
        public string Promocode { get; set; }
        public bool? Deleted { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Period { get; set; }
        public bool? Processed { get; set; }
        public long? App_Action_Id { get; set; }
        public string Kind { get; set; }
        public string Auto_Link_Error { get; set; }
        public string Transfer_Sender { get; set; }
        public string Transfer_Recipient { get; set; }
        public string Import_Details { get; set; }
        public long? Import_Id { get; set; }
        public string Note { get; set; }
        public bool? Closed { get; set; }
        public long? Creator_Id { get; set; }
        public long? Updater_Id { get; set; }
        public string Match_Status { get; set; }
        public string Code { get; set; }
        public long? Payment_Id { get; set; }
        public string Import_Md5 { get; set; }
        public string Account_Number { get; set; }
        public string Additional_Discount { get; set; }
        public string Transaction_Id { get; set; }
        public long? Subscription_Id { get; set; }
        public string Lang { get; set; }
        public string Partner { get; set; }
        public bool? Income { get; set; }
        public string Transaction_Kind { get; set; }
        public decimal? Commission { get; set; }
        public bool? Use_Moss { get; set; }
        public string Moss_Notice { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Tax { get; set; }
        public int? Attachments_Count { get; set; }
        public bool? Test { get; set; }
        public bool? Recurring { get; set; }
        public string Client_Bank_Account_Number { get; set; }
        public long? Bank_Account_Id { get; set; }
        //public string Additional_Fields { get; set; }
        
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Overpaid { get; set; }
        public string External_Payment_Id { get; set; }
        public string Cheque_Number { get; set; }
        public string Card_Number { get; set; }
        public string Bank { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Bank_Account_Balance { get; set; }
        public string Import_Kind { get; set; }
        public string Import_Ref { get; set; }
        public string InvoiceCompany { get; set; }
        public string No_Duplicate_Md5 { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Additional_Discount_Amount { get; set; }
        public string Gocardless_Payment_Id { get; set; }
        public string Payment_Callback { get; set; }
        public string Number { get; set; }
        public DateTime? Issue_Date { get; set; }
        public DateTime? Payment_To { get; set; }
        public string Payment_To_Kind { get; set; }
        public string Payment_Type { get; set; }
        public DateTime? Sell_Date { get; set; }
        public string Sell_Date_Kind { get; set; }
        public string Place { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Gross { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Net { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Tax { get; set; }
        public string Seller_Name { get; set; }
        public string Seller_Street { get; set; }
        public string Seller_Post_Code { get; set; }
        public string Seller_City { get; set; }
        public string Seller_Country { get; set; }
        public string Seller_Bank { get; set; }
        public string Seller_Bank_Account { get; set; }
        public long? Seller_Bank_Account_Id { get; set; }
        public string Seller_Email { get; set; }
        public string Seller_Fax { get; set; }
        public string Seller_Person { get; set; }
        public string Seller_Phone { get; set; }
        public string Seller_Tax_No { get; set; }
        public string Seller_Tax_No_Kind { get; set; }
        public string Seller_Www { get; set; }
        public string Delivery_Address { get; set; }
        public string Description_Footer { get; set; }
        public string Description_Long { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Discount { get; set; }
        public string Discount_Kind { get; set; }
        public string Exchange_Currency { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Exchange_Currency_Rate { get; set; }
        public DateTime? Exchange_Date { get; set; }
        public string Exchange_Kind { get; set; }
        public string Exchange_Note { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Exchange_Rate { get; set; }
        public bool? Buyer_Company { get; set; }
        public string Buyer_Name { get; set; }
        public string Buyer_Street { get; set; }
        public string Buyer_Post_Code { get; set; }
        public string Buyer_City { get; set; }
        public string Buyer_Country { get; set; }
        public string Buyer_Bank { get; set; }
        public string Buyer_Bank_Account { get; set; }
        public string Buyer_Email { get; set; }
        public string Buyer_Fax { get; set; }
        public string Buyer_Person { get; set; }
        public string Buyer_Phone { get; set; }
        public string Buyer_Tax_No { get; set; }
        public string Buyer_Tax_No_Kind { get; set; }
        public string Buyer_Www { get; set; }
        public bool? Show_Discount { get; set; }
        public int? Split_Payment { get; set; }
        public string Buyer_Mobile_Phone { get; set; }
        public string Seller_Bdo_No { get; set; }
        public string Seller_Ksef_Taxpayer_Status { get; set; }
        public string E_Receipt_View_Url { get; set; }
        public List<PaymentPosition> Positions { get; set; }
        public JToken Descriptions { get; set; }
    }

    public class PaymentPosition
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Additional_Info { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Quantity { get; set; }
        public string Quantity_Unit { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Discount { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Discount_Percent { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Net { get; set; }
        public string Tax { get; set; }
        public string Tax2 { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Tax { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Gross { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Total_Price_Net { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Total_Price_Tax { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Total_Price_Gross { get; set; }
    }

}
