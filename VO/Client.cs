using System;
using System.Text.Json.Serialization;

namespace FakturowniaService
{
    public class Client
    {
        public long? Id { get; set; }

        public long GetId()
        {
            return Id ?? 0;
        }
        public string Name { get; set; }
        public string Tax_No { get; set; }
        public string Post_Code { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string First_Name { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Www { get; set; }
        public string Fax { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public string Street_No { get; set; }
        public string Kind { get; set; }
        public string Bank { get; set; }
        public string Bank_Account { get; set; }
        public long? Bank_Account_Id { get; set; }
        public string Shortcut { get; set; }
        public string Note { get; set; }
        public string Last_Name { get; set; }
        public string Referrer { get; set; }
        public string Token { get; set; }
        public string Fuid { get; set; }
        public string Fname { get; set; }
        public string Femail { get; set; }
        public long? Department_Id { get; set; }
        public string Import { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Discount { get; set; }
        public string Payment_To_Kind { get; set; }
        public long? Category_Id { get; set; }
        public bool Use_Delivery_Address { get; set; }
        public string Delivery_Address { get; set; }
        public string Person { get; set; }
        public long? Panel_User_Id { get; set; }
        public bool Use_Mass_Payment { get; set; }
        public string Mass_Payment_Code { get; set; }
        public string External_Id { get; set; }
        public bool Company { get; set; }
        public string Title { get; set; }
        public string Mobile_Phone { get; set; }
        public string Register_Number { get; set; }
        public string Tax_No_Check { get; set; }
        public int Attachments_Count { get; set; }
        public string Default_Payment_Type { get; set; }
        public string Tax_No_Kind { get; set; }
        public string Accounting_Id { get; set; }
        public bool Disable_Auto_Reminders { get; set; }
        public long? Buyer_Id { get; set; }
        public long? Price_List_Id { get; set; }
        public string Panel_Url { get; set; }
    }

}
