using log4net;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace FakturowniaService
{
    public static class DBUtil
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void DeleteAllRows(string tableName, SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand($"DELETE FROM {tableName}", connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void DisableForeignKeyCheck(SqlConnection connection, SqlTransaction transaction, string tableName, string foreignKeyName)
        {
            using (var command = new SqlCommand($"ALTER TABLE {tableName} NOCHECK CONSTRAINT {foreignKeyName}", connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void EnableForeignKeyCheck(SqlConnection connection, SqlTransaction transaction, string tableName, string foreignKeyName)
        {
            using (var command = new SqlCommand($"ALTER TABLE {tableName} CHECK CONSTRAINT {foreignKeyName}", connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }
        public static void TruncateTable(string tableName, SqlConnection connection, SqlTransaction transaction)
        {
            string query = $"TRUNCATE TABLE {tableName}";

            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void InsertInvoiceImportLog(SqlConnection connection, SqlTransaction transaction, int executionTime)
        {
            using (SqlCommand command = new SqlCommand("[dbo].[sp_Insert_Fakturownia_Invoice_Import_Log]", connection, transaction))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ExecutionTime", SqlDbType.Int) { Value = executionTime });
                command.ExecuteNonQuery();
            }
        }
        public static void InsertInvoiceHeader(Invoice invoice, SqlConnection connection, SqlTransaction transaction)
        {
            var query = @"
        INSERT INTO [dbo].[Fakturownia_InvoiceHead] (
            id, user_id, app, number, place, sell_date, payment_type, price_net, price_gross, currency, status, description, seller_name, 
            seller_tax_no, seller_street, seller_post_code, seller_city, seller_country, seller_email, seller_phone, seller_fax, seller_www, 
            seller_person, seller_bank, seller_bank_account, buyer_name, buyer_tax_no, buyer_post_code, buyer_city, buyer_street, 
            buyer_first_name, buyer_country, created_at, updated_at, token, buyer_email, buyer_www, buyer_fax, buyer_phone, kind, pattern, 
            pattern_nr, pattern_nr_m, pattern_nr_d, client_id, payment_to, paid, seller_bank_account_id, lang, issue_date, price_tax, 
            department_id, correction, buyer_note, additional_info_desc, additional_info, product_cache, buyer_last_name, from_invoice_id, 
            oid, discount, show_discount, sent_time, print_time, recurring_id, tax2_visible, warehouse_id, paid_date, product_id, 
            issue_year, internal_note, invoice_id, invoice_template_id, description_long, buyer_tax_no_kind, seller_tax_no_kind, 
            description_footer, sell_date_kind, payment_to_kind, exchange_currency, discount_kind, income, from_api, category_id, 
            warehouse_document_id, exchange_kind, exchange_rate, use_delivery_address, delivery_address, accounting_kind, buyer_person, 
            buyer_bank_account, buyer_bank, buyer_mass_payment_code, exchange_note, buyer_company, show_attachments, exchange_currency_rate, 
            has_attachments, exchange_date, attachments_count, delivery_date, fiscal_status, use_moss, transaction_date, email_status, 
            exclude_from_stock_level, exclude_from_accounting, exchange_rate_den, exchange_currency_rate_den, accounting_scheme, 
            exchange_difference, not_cost, reverse_charge, issuer, use_issuer, cancelled, recipient_id, recipient_name, test, discount_net, 
            approval_status, accounting_vat_tax_date, accounting_income_tax_date, accounting_other_tax_date, accounting_status, 
            normalized_number, na_tax_kind, issued_to_receipt, gov_id, gov_kind, gov_status, sales_code, additional_invoice_field, 
            products_margin, payment_url, view_url, buyer_mobile_phone, kind_text, invoice_for_receipt_id, receipt_for_invoice_id, 
            recipient_company, recipient_first_name, recipient_last_name, recipient_tax_no, recipient_street, recipient_post_code, 
            recipient_city, recipient_country, recipient_email, recipient_phone, recipient_note, overdue, get_tax_name, tax_visible, 
            tax_name_type, split_payment, gtu_codes, procedure_designations
        ) VALUES (
            @id, @user_id, @app, @number, @place, @sell_date, @payment_type, @price_net, @price_gross, @currency, @status, @description, 
            @seller_name, @seller_tax_no, @seller_street, @seller_post_code, @seller_city, @seller_country, @seller_email, @seller_phone, 
            @seller_fax, @seller_www, @seller_person, @seller_bank, @seller_bank_account, @buyer_name, @buyer_tax_no, @buyer_post_code, 
            @buyer_city, @buyer_street, @buyer_first_name, @buyer_country, @created_at, @updated_at, @token, @buyer_email, @buyer_www, 
            @buyer_fax, @buyer_phone, @kind, @pattern, @pattern_nr, @pattern_nr_m, @pattern_nr_d, @client_id, @payment_to, @paid, 
            @seller_bank_account_id, @lang, @issue_date, @price_tax, @department_id, @correction, @buyer_note, @additional_info_desc, 
            @additional_info, @product_cache, @buyer_last_name, @from_invoice_id, @oid, @discount, @show_discount, @sent_time, @print_time, 
            @recurring_id, @tax2_visible, @warehouse_id, @paid_date, @product_id, @issue_year, @internal_note, @invoice_id, 
            @invoice_template_id, @description_long, @buyer_tax_no_kind, @seller_tax_no_kind, @description_footer, @sell_date_kind, 
            @payment_to_kind, @exchange_currency, @discount_kind, @income, @from_api, @category_id, @warehouse_document_id, @exchange_kind, 
            @exchange_rate, @use_delivery_address, @delivery_address, @accounting_kind, @buyer_person, @buyer_bank_account, @buyer_bank, 
            @buyer_mass_payment_code, @exchange_note, @buyer_company, @show_attachments, @exchange_currency_rate, @has_attachments, 
            @exchange_date, @attachments_count, @delivery_date, @fiscal_status, @use_moss, @transaction_date, @email_status, 
            @exclude_from_stock_level, @exclude_from_accounting, @exchange_rate_den, @exchange_currency_rate_den, @accounting_scheme, 
            @exchange_difference, @not_cost, @reverse_charge, @issuer, @use_issuer, @cancelled, @recipient_id, @recipient_name, @test, 
            @discount_net, @approval_status, @accounting_vat_tax_date, @accounting_income_tax_date, @accounting_other_tax_date, 
            @accounting_status, @normalized_number, @na_tax_kind, @issued_to_receipt, @gov_id, @gov_kind, @gov_status, @sales_code, 
            @additional_invoice_field, @products_margin, @payment_url, @view_url, @buyer_mobile_phone, @kind_text, @invoice_for_receipt_id, 
            @receipt_for_invoice_id, @recipient_company, @recipient_first_name, @recipient_last_name, @recipient_tax_no, @recipient_street, 
            @recipient_post_code, @recipient_city, @recipient_country, @recipient_email, @recipient_phone, @recipient_note, @overdue, 
            @get_tax_name, @tax_visible, @tax_name_type, @split_payment, @gtu_codes, @procedure_designations
        );";

            log.Info($"Inserting invoice {invoice.Id} into database.");

            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@id", invoice.Id ?? 0);
                command.Parameters.AddWithValue("@user_id", invoice.UserId ?? 0);
                command.Parameters.AddWithValue("@app", invoice.App ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@number", invoice.Number ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@place", invoice.Place ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@sell_date", invoice.Sell_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@payment_type", invoice.Payment_Type ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@price_net", invoice.Price_Net);
                command.Parameters.AddWithValue("@price_gross", invoice.Price_Gross);
                command.Parameters.AddWithValue("@currency", invoice.Currency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@status", invoice.Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@description", invoice.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_name", invoice.Seller_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_tax_no", invoice.Seller_Tax_No ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_street", invoice.Seller_Street ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_post_code", invoice.Seller_Post_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_city", invoice.Seller_City ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_country", invoice.Seller_Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_email", invoice.Seller_Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_phone", invoice.Seller_Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_fax", invoice.Seller_Fax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_www", invoice.Seller_Www ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_person", invoice.Seller_Person ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_bank", invoice.Seller_Bank ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_bank_account", invoice.Seller_Bank_Account ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_name", invoice.Buyer_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_tax_no", invoice.Buyer_Tax_No ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_post_code", invoice.Buyer_Post_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_city", invoice.Buyer_City ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_street", invoice.Buyer_Street ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_first_name", invoice.Buyer_First_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_country", invoice.Buyer_Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@created_at", invoice.Created_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@updated_at", invoice.Updated_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@token", invoice.Token ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_email", invoice.Buyer_Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_www", invoice.Buyer_Www ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_fax", invoice.Buyer_Fax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_phone", invoice.Buyer_Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@kind", invoice.Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pattern", invoice.Pattern ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pattern_nr", invoice.Pattern_Nr ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pattern_nr_m", invoice.Pattern_Nr_M ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pattern_nr_d", invoice.Pattern_Nr_D ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@client_id", invoice.Client_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@payment_to", invoice.Payment_To ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@paid", invoice.Paid ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_bank_account_id", invoice.Seller_Bank_Account_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@lang", invoice.Lang ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@issue_date", invoice.Issue_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@price_tax", invoice.Price_Tax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@department_id", invoice.Department_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@correction", invoice.Correction ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_note", invoice.Buyer_Note ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@additional_info_desc", invoice.Additional_Info_Desc ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@additional_info", invoice.Additional_Info ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@product_cache", invoice.Product_Cache ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_last_name", invoice.Buyer_Last_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@from_invoice_id", invoice.From_Invoice_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@oid", invoice.Oid ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@discount", invoice.Discount ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@show_discount", invoice.Show_Discount ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@sent_time", invoice.Sent_Time ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@print_time", invoice.Print_Time ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recurring_id", invoice.Recurring_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tax2_visible", invoice.Tax2_Visible ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@warehouse_id", invoice.Warehouse_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@paid_date", invoice.Paid_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@product_id", invoice.Product_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@issue_year", invoice.Issue_Year ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@internal_note", invoice.Internal_Note ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_id", invoice.Invoice_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_template_id", invoice.Invoice_Template_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@description_long", invoice.Description_Long ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_tax_no_kind", invoice.Buyer_Tax_No_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@seller_tax_no_kind", invoice.Seller_Tax_No_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@description_footer", invoice.Description_Footer ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@sell_date_kind", invoice.Sell_Date_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@payment_to_kind", invoice.Payment_To_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_currency", invoice.Exchange_Currency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@discount_kind", invoice.Discount_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@income", invoice.Income ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@from_api", invoice.From_Api ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@category_id", invoice.Category_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@warehouse_document_id", invoice.Warehouse_Document_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_kind", invoice.Exchange_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_rate", invoice.Exchange_Rate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@use_delivery_address", invoice.Use_Delivery_Address ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@delivery_address", invoice.Delivery_Address ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@accounting_kind", invoice.Accounting_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_person", invoice.Buyer_Person ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_bank_account", invoice.Buyer_Bank_Account ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_bank", invoice.Buyer_Bank ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_mass_payment_code", invoice.Buyer_Mass_Payment_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_note", invoice.Exchange_Note ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_company", invoice.Buyer_Company ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@show_attachments", invoice.Show_Attachments ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_currency_rate", invoice.Exchange_Currency_Rate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@has_attachments", invoice.Has_Attachments ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_date", invoice.Exchange_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@attachments_count", invoice.Attachments_Count ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@delivery_date", invoice.Delivery_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fiscal_status", invoice.Fiscal_Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@use_moss", invoice.Use_Moss ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@transaction_date", invoice.Transaction_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email_status", invoice.Email_Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exclude_from_stock_level", invoice.Exclude_From_Stock_Level ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exclude_from_accounting", invoice.Exclude_From_Accounting ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_rate_den", invoice.Exchange_Rate_Den ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_currency_rate_den", invoice.Exchange_Currency_Rate_Den ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@accounting_scheme", invoice.Accounting_Scheme ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_difference", invoice.Exchange_Difference ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@not_cost", invoice.Not_Cost ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@reverse_charge", invoice.Reverse_Charge ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@issuer", invoice.Issuer ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@use_issuer", invoice.Use_Issuer ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cancelled", invoice.Cancelled ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_id", invoice.Recipient_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_name", invoice.Recipient_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@test", invoice.Test ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@discount_net", invoice.Discount_Net ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@approval_status", invoice.Approval_Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@accounting_vat_tax_date", invoice.Accounting_Vat_Tax_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@accounting_income_tax_date", invoice.Accounting_Income_Tax_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@accounting_other_tax_date", invoice.Accounting_Other_Tax_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@accounting_status", invoice.Accounting_Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@normalized_number", invoice.Normalized_Number ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@na_tax_kind", invoice.Na_Tax_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@issued_to_receipt", invoice.Issued_To_Receipt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gov_id", invoice.Gov_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gov_kind", invoice.Gov_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gov_status", invoice.Gov_Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@sales_code", invoice.Sales_Code ?? (object)DBNull.Value);

                command.Parameters.AddWithValue("@additional_invoice_field", invoice.Additional_Invoice_Field ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@products_margin", invoice.Products_Margin ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@payment_url", invoice.Payment_Url ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@view_url", invoice.View_Url ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@buyer_mobile_phone", invoice.Buyer_Mobile_Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@kind_text", invoice.Kind_Text ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_for_receipt_id", invoice.Invoice_For_Receipt_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@receipt_for_invoice_id", invoice.Receipt_For_InvoiceId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_company", invoice.Recipient_Company ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_first_name", invoice.Recipient_First_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_last_name", invoice.Recipient_Last_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_tax_no", invoice.Recipient_Tax_No ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_street", invoice.Recipient_Street ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_post_code", invoice.Recipient_Post_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_city", invoice.Recipient_City ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_country", invoice.Recipient_Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_email", invoice.Recipient_Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_phone", invoice.Recipient_Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recipient_note", invoice.Recipient_Note ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@overdue", invoice.Overdue ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@get_tax_name", invoice.Get_Tax_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tax_visible", invoice.Tax_Visible ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tax_name_type", invoice.Tax_Name_Type ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@split_payment", invoice.Split_Payment ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gtu_codes", (object)DBNull.Value);
                command.Parameters.AddWithValue("@procedure_designations", (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public static void InsertInvoiceItem(InvoiceItem item, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"
        INSERT INTO Fakturownia_InvoiceItem (
            id, invoice_id, name, description, price_net, quantity, total_price_gross, total_price_net, account_id, created_at, 
            updated_at, additional_info, quantity_unit, tax, price_gross, price_tax, total_price_tax, kind, invoice_position_id, 
            product_id, deleted, discount, discount_percent, tax2, exchange_rate, accounting_tax_kind, code, discount_net, 
            lump_sum_tax, corrected_pos_kind, gtu_code
        ) VALUES (
            @id, @invoice_id, @name, @description, @price_net, @quantity, @total_price_gross, @total_price_net, @account_id, @created_at, 
            @updated_at, @additional_info, @quantity_unit, @tax, @price_gross, @price_tax, @total_price_tax, @kind, @invoice_position_id, 
            @product_id, @deleted, @discount, @discount_percent, @tax2, @exchange_rate, @accounting_tax_kind, @code, @discount_net, 
            @lump_sum_tax, @corrected_pos_kind, @gtu_code
        )";

            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@invoice_Id", item.Invoice_Id);
                command.Parameters.AddWithValue("@name", (object)item.Name ?? DBNull.Value);
                command.Parameters.AddWithValue("@description", (object)item.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@price_net", (object)item.Price_Net ?? DBNull.Value);
                command.Parameters.AddWithValue("@quantity", (object)item.Quantity ?? DBNull.Value);
                command.Parameters.AddWithValue("@total_price_gross", (object)item.Total_Price_Gross ?? DBNull.Value);
                command.Parameters.AddWithValue("@total_price_net", (object)item.Total_Price_Net ?? DBNull.Value);
                command.Parameters.AddWithValue("@account_id", item.Account_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@created_at", item.Created_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@updated_at", item.Updated_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@additional_info", item.Additional_Info ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@quantity_unit", item.Quantity_Unit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tax", item.Tax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@price_gross", item.Price_Gross ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@price_tax", item.Price_Tax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@total_price_tax", item.Total_Price_Tax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@kind", item.Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_position_id", item.Invoice_Position_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@product_id", item.Product_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@deleted", item.Deleted ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@discount", item.Discount ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@discount_percent", item.Discount_Percent ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tax2", item.Tax2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@exchange_rate", item.Exchange_Rate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@accounting_tax_kind", item.Accounting_Tax_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@code", item.Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@discount_net", item.Discount_Net ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@lump_sum_tax", item.Lump_Sum_Tax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@corrected_pos_kind", (object)DBNull.Value);
                command.Parameters.AddWithValue("@gtu_code", (object)DBNull.Value);
                command.ExecuteNonQuery();
            }
        }
    }
}
