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
        public String Name { get; set; }
        public String Tax_No { get; set; }
        public String Post_Code { get; set; }
        public String City { get; set; }
        public String Street { get; set; }
        public String First_Name { get; set; }
        public String Country { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public String Www { get; set; }
        public String Fax { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public String Street_No { get; set; }
        public String Kind { get; set; }
        public String Bank { get; set; }
        public String Bank_Account { get; set; }
        public long? Bank_Account_Id { get; set; }
        public String Shortcut { get; set; }
        public String Note { get; set; }
        public String Last_Name { get; set; }
        public String Referrer { get; set; }
        public String Token { get; set; }
        public String Fuid { get; set; }
        public String Fname { get; set; }
        public String Femail { get; set; }
        public long? Department_Id { get; set; }
        public String Import { get; set; }

        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Discount { get; set; }
        public string Payment_To_Kind { get; set; }
        public long? Category_Id { get; set; }
        public bool Use_Delivery_Address { get; set; }
        public String Delivery_Address { get; set; }
        public String Person { get; set; }
        public long? Panel_User_Id { get; set; }
        public bool? Use_Mass_Payment { get; set; }
        public String Mass_Payment_Code { get; set; }
        public String External_Id { get; set; }
        public bool Company { get; set; }
        public String Title { get; set; }
        public String Mobile_Phone { get; set; }
        public String Register_Number { get; set; }
        public String Tax_No_Check { get; set; }
        public int? Attachments_Count { get; set; }
        public String Default_Payment_Type { get; set; }
        public String Tax_No_Kind { get; set; }
        public String Accounting_Id { get; set; }
        public bool Disable_Auto_Reminders { get; set; }
        public long? Buyer_Id { get; set; }
        public long? Price_List_Id { get; set; }
        public String Panel_Url { get; set; }
    }

}
