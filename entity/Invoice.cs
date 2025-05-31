using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FakturowniaService
{
    public class Invoice
    {
        public long? Id { get; set; }

        public long GetId()
        {
            return Id ?? 0;
        }
        public long? UserId { get; set; }
        public long GetUserId()
        {
            return UserId ?? 0;
        }
        public string App { get; set; }
        public string Number { get; set; }
        public string Place { get; set; }
        public DateTime? Sell_Date { get; set; }
        public string Payment_Type { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Net { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Price_Gross { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Seller_Name { get; set; }
        public string Seller_Tax_No { get; set; }
        public string Seller_Street { get; set; }
        public string Seller_Post_Code { get; set; }
        public string Seller_City { get; set; }
        public string Seller_Country { get; set; }
        public string Seller_Email { get; set; }
        public string Seller_Phone { get; set; }
        public string Seller_Fax { get; set; }
        public string Seller_Www { get; set; }
        public string Seller_Person { get; set; }
        public string Seller_Bank { get; set; }
        public string Seller_Bank_Account { get; set; }
        public string Buyer_Name { get; set; }
        public string Buyer_Tax_No { get; set; }
        public string Buyer_Post_Code { get; set; }
        public string Buyer_City { get; set; }
        public string Buyer_Street { get; set; }
        public string Buyer_First_Name { get; set; }
        public string Buyer_Country { get; set; }
        public DateTimeOffset? Created_At { get; set; }
        public DateTimeOffset? Updated_At { get; set; }
        public string Token { get; set; }
        public string Buyer_Email { get; set; }
        public string Buyer_Www { get; set; }
        public string Buyer_Fax { get; set; }
        public string Buyer_Phone { get; set; }
        public string Kind { get; set; }
        public string Pattern { get; set; }
        public string Pattern_Nr { get; set; }
        public int? Pattern_Nr_M { get; set; }
        public string Pattern_Nr_D { get; set; }
        public long? Client_Id { get; set; }
        public DateTime? Payment_To { get; set; }
        public decimal? Paid { get; set; }
        public long? Seller_Bank_Account_Id { get; set; }
        public string Lang { get; set; }
        public DateTime? Issue_Date { get; set; }
        public decimal? Price_Tax { get; set; }
        public long? Department_Id { get; set; }
        public string Correction { get; set; }
        public string Buyer_Note { get; set; }
        public string Additional_Info_Desc { get; set; }
        public bool? Additional_Info { get; set; }
        public string Product_Cache { get; set; }
        public string Buyer_Last_Name { get; set; }
        public long? From_Invoice_Id { get; set; }
        public string Oid { get; set; }
        public decimal? Discount { get; set; }
        public bool? Show_Discount { get; set; }
        public DateTimeOffset? Sent_Time { get; set; }
        public DateTimeOffset? Print_Time { get; set; }
        public long? Recurring_Id { get; set; }
        public bool? Tax2_Visible { get; set; }
        public long? Warehouse_Id { get; set; }
        public DateTime? Paid_Date { get; set; }
        public long? Product_Id { get; set; }
        public int? Issue_Year { get; set; }
        public string Internal_Note { get; set; }
        public long? Invoice_Id { get; set; }
        public int? Invoice_Template_Id { get; set; }
        public string Description_Long { get; set; }
        public string Buyer_Tax_No_Kind { get; set; }
        public string Seller_Tax_No_Kind { get; set; }
        public string Description_Footer { get; set; }
        public string Sell_Date_Kind { get; set; }
        public string Payment_To_Kind { get; set; }
        public string Exchange_Currency { get; set; }
        public string Discount_Kind { get; set; }
        public bool? Income { get; set; }
        public bool? From_Api { get; set; }
        public long? Category_Id { get; set; }
        public long? Warehouse_Document_Id { get; set; }
        public string Exchange_Kind { get; set; }
        public decimal? Exchange_Rate { get; set; }
        public bool? Use_Delivery_Address { get; set; }
        public string Delivery_Address { get; set; }
        public string Accounting_Kind { get; set; }
        public string Buyer_Person { get; set; }
        public string Buyer_Bank_Account { get; set; }
        public string Buyer_Bank { get; set; }
        public string Buyer_Mass_Payment_Code { get; set; }
        public string Exchange_Note { get; set; }
        public bool? Buyer_Company { get; set; }
        public bool? Show_Attachments { get; set; }
        public decimal? Exchange_Currency_Rate { get; set; }
        public bool? Has_Attachments { get; set; }
        public DateTime? Exchange_Date { get; set; }
        public int? Attachments_Count { get; set; }
        public DateTime? Delivery_Date { get; set; }
        public string Fiscal_Status { get; set; }
        public bool? Use_Moss { get; set; }
        public DateTime? Transaction_Date { get; set; }
        public string Email_Status { get; set; }
        public bool? Exclude_From_Stock_Level { get; set; }
        public bool? Exclude_From_Accounting { get; set; }
        public decimal? Exchange_Rate_Den { get; set; }
        public decimal? Exchange_Currency_Rate_Den { get; set; }
        public string Accounting_Scheme { get; set; }
        public decimal? Exchange_Difference { get; set; }
        public bool? Not_Cost { get; set; }
        public bool? Reverse_Charge { get; set; }
        public string Issuer { get; set; }
        public bool? Use_Issuer { get; set; }
        public bool? Cancelled { get; set; }
        public long? Recipient_Id { get; set; }
        public string Recipient_Name { get; set; }
        public bool? Test { get; set; }
        public decimal? Discount_Net { get; set; }
        public string Approval_Status { get; set; }
        public DateTime? Accounting_Vat_Tax_Date { get; set; }
        public DateTime? Accounting_Income_Tax_Date { get; set; }
        public DateTime? Accounting_Other_Tax_Date { get; set; }
        public string Accounting_Status { get; set; }
        public string Normalized_Number { get; set; }
        public string Na_Tax_Kind { get; set; }
        public bool? Issued_To_Receipt { get; set; }
        public string Gov_Id { get; set; }
        public string Gov_Kind { get; set; }
        public string Gov_Status { get; set; }
        public string Sales_Code { get; set; }
        public string Additional_Invoice_Field { get; set; }
        public decimal? Products_Margin { get; set; }
        public string Payment_Url { get; set; }
        public string View_Url { get; set; }
        public string Buyer_Mobile_Phone { get; set; }
        public string Kind_Text { get; set; }
        public long? Invoice_For_Receipt_Id { get; set; }
        public long? Receipt_For_InvoiceId { get; set; }
        public bool? Recipient_Company { get; set; }
        public string Recipient_First_Name { get; set; }
        public string Recipient_Last_Name { get; set; }
        public string Recipient_Tax_No { get; set; }
        public string Recipient_Street { get; set; }
        public string Recipient_Post_Code { get; set; }
        public string Recipient_City { get; set; }
        public string Recipient_Country { get; set; }
        public string Recipient_Email { get; set; }
        public string Recipient_Phone { get; set; }
        public string Recipient_Note { get; set; }
        public bool? Overdue { get; set; }
        public string Get_Tax_Name { get; set; }
        public bool? Tax_Visible { get; set; }
        public string Tax_Name_Type { get; set; }
        public int? Split_Payment { get; set; }
        //public string Gtu_Codes { get; set; }
        //public string Procedure_Designations { get; set; }
        public List<InvoiceItem> Positions { get; set; }
    }
}
