using FakturowniaService.util;
using System;
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
        public bool? Paid { get; set; }
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
    }

}
