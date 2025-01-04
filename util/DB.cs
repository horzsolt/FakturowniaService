using FakturowniaService.task;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Data;


namespace FakturowniaService
{
    public static class DB
    {

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

        public static void InsertClientImportLog(SqlConnection connection, SqlTransaction transaction, int executionTime)
        {
            using (SqlCommand command = new SqlCommand("[dbo].[sp_Insert_Fakturownia_Client_Import_Log]", connection, transaction))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ExecutionTime", SqlDbType.Int) { Value = executionTime });
                command.ExecuteNonQuery();
            }
        }

        public static void InsertProductImportLog(SqlConnection connection, SqlTransaction transaction, int executionTime)
        {
            using (SqlCommand command = new SqlCommand("[dbo].[sp_Insert_Fakturownia_Product_Import_Log]", connection, transaction))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ExecutionTime", SqlDbType.Int) { Value = executionTime });
                command.ExecuteNonQuery();
            }
        }

        public static void InsertPaymentImportLog(SqlConnection connection, SqlTransaction transaction, int executionTime)
        {
            using (SqlCommand command = new SqlCommand("[dbo].[sp_Insert_Fakturownia_Payment_Import_Log]", connection, transaction))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ExecutionTime", SqlDbType.Int) { Value = executionTime });
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

        public static void InsertProduct(Product product, SqlConnection connection, SqlTransaction transaction, ILogger<ImportTask> log)
        {
            log.LogInformation($"Inserting product {product.Id} into the database.");

            var query = @"
        INSERT INTO Fakturownia_Product (
            id, name, description, price_net, price_gross, price_tax, tax, created_at, updated_at,
            automatic_sales, limited, warehouse_quantity, available_from, available_to, payment_callback,
            payment_url_ok, payment_url_error, token, quantity, quantity_unit, additional_info, disabled,
            form_fields_horizontal, form_fields, form_name, form_description, quantity_sold_outside,
            form_kind, form_template, elastic_price, next_product_id, quantity_sold_in_invoices, deleted, 
            code, currency, ecommerce, period, show_elastic_price, elastic_price_details, elastic_price_date_trigger,
            iid, use_formula, formula, formula_test_field, stock_level, sync, category_id, kind, package, 
            package_product_ids, department_id, use_product_warehouses, service, use_quantity_discount,
            quantity_discount_details, price_net_on_payment, warehouse_numbers_updated_at, ean_code, weight, 
            weight_unit, size_height, size_width, size, size_unit, auto_payment_department_id, attachments_count, 
            image_url, tax2, supplier_code, package_products_details, siteor_disabled, use_moss, subscription_id,
            accounting_id, status, restricted_to_warehouses, gtu_codes, tag_list, gtu_code, electronic_service, 
            is_delivery
        ) VALUES (
            @Id, @Name, @Description, @PriceNet, @PriceGross, @PriceTax, @Tax, @CreatedAt, @UpdatedAt,
            @AutomaticSales, @Limited, @WarehouseQuantity, @AvailableFrom, @AvailableTo, @PaymentCallback,
            @PaymentUrlOk, @PaymentUrlError, @Token, @Quantity, @QuantityUnit, @AdditionalInfo, @Disabled,
            @FormFieldsHorizontal, @FormFields, @FormName, @FormDescription, @QuantitySoldOutside, 
            @FormKind, @FormTemplate, @ElasticPrice, @NextProductId, @QuantitySoldInInvoices, @Deleted,
            @Code, @Currency, @Ecommerce, @Period, @ShowElasticPrice, @ElasticPriceDetails, @ElasticPriceDateTrigger,
            @Iid, @UseFormula, @Formula, @FormulaTestField, @StockLevel, @Sync, @CategoryId, @Kind, @Package, 
            @PackageProductIds, @DepartmentId, @UseProductWarehouses, @Service, @UseQuantityDiscount,
            @QuantityDiscountDetails, @PriceNetOnPayment, @WarehouseNumbersUpdatedAt, @EanCode, @Weight,
            @WeightUnit, @SizeHeight, @SizeWidth, @Size, @SizeUnit, @AutoPaymentDepartmentId, @AttachmentsCount,
            @ImageUrl, @Tax2, @SupplierCode, @PackageProductsDetails, @SiteorDisabled, @UseMoss, @SubscriptionId,
            @AccountingId, @Status, @RestrictedToWarehouses, @GtuCodes, @TagList, @GtuCode, @ElectronicService, 
            @IsDelivery
        )";

            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Id", product.Id);
                command.Parameters.AddWithValue("@Name", product.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PriceNet", product.Price_Net);
                command.Parameters.AddWithValue("@PriceGross", product.Price_Gross);
                command.Parameters.AddWithValue("@PriceTax", product.Price_Tax);
                command.Parameters.AddWithValue("@Tax", product.Tax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedAt", product.Created_At);
                command.Parameters.AddWithValue("@UpdatedAt", product.Updated_At);
                command.Parameters.AddWithValue("@AutomaticSales", product.Automatic_Sales);
                command.Parameters.AddWithValue("@Limited", product.Limited);
                command.Parameters.AddWithValue("@WarehouseQuantity", product.Warehouse_Quantity);
                command.Parameters.AddWithValue("@AvailableFrom", product.Available_From ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AvailableTo", product.Available_To ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PaymentCallback", product.Payment_Callback ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PaymentUrlOk", product.Payment_Url_Ok ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PaymentUrlError", product.Payment_Url_Error ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Token", product.Token);
                command.Parameters.AddWithValue("@Quantity", product.Quantity);
                command.Parameters.AddWithValue("@QuantityUnit", product.Quantity_Unit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AdditionalInfo", product.Additional_Info ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Disabled", product.Disabled);
                command.Parameters.AddWithValue("@FormFieldsHorizontal", product.Form_Fields_Horizontal);
                command.Parameters.AddWithValue("@FormFields", product.Form_Fields ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FormName", product.Form_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FormDescription", product.Form_Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@QuantitySoldOutside", product.Quantity_Sold_Outside ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FormKind", product.Form_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FormTemplate", product.Form_Template ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ElasticPrice", product.Elastic_Price);
                command.Parameters.AddWithValue("@NextProductId", product.Next_Product_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@QuantitySoldInInvoices", product.Quantity_Sold_In_Invoices);
                command.Parameters.AddWithValue("@Deleted", product.Deleted);
                command.Parameters.AddWithValue("@Code", product.Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Currency", product.Currency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Ecommerce", product.Ecommerce);
                command.Parameters.AddWithValue("@Period", product.Period ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShowElasticPrice", product.Show_Elastic_Price);
                command.Parameters.AddWithValue("@ElasticPriceDetails", product.Elastic_Price_Details ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ElasticPriceDateTrigger", product.Elastic_Price_Date_Trigger ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Iid", product.Iid ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UseFormula", product.Use_Formula);
                command.Parameters.AddWithValue("@Formula", product.Formula ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FormulaTestField", product.Formula_Test_Field ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@StockLevel", product.Stock_Level);
                command.Parameters.AddWithValue("@Sync", product.Sync);
                command.Parameters.AddWithValue("@CategoryId", product.Category_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Kind", product.Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Package", product.Package);
                command.Parameters.AddWithValue("@PackageProductIds", product.Package_Product_Ids ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DepartmentId", product.Department_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UseProductWarehouses", product.Use_Product_Warehouses);
                command.Parameters.AddWithValue("@Service", product.Service);
                command.Parameters.AddWithValue("@UseQuantityDiscount", product.Use_Quantity_Discount);
                command.Parameters.AddWithValue("@QuantityDiscountDetails", product.Quantity_Discount_Details ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PriceNetOnPayment", product.Price_Net_On_Payment);
                command.Parameters.AddWithValue("@WarehouseNumbersUpdatedAt", product.Warehouse_Numbers_Updated_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EanCode", product.Ean_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Weight", product.Weight ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@WeightUnit", product.Weight_Unit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SizeHeight", product.Size_Height ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SizeWidth", product.Size_Width ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Size", product.Size ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SizeUnit", product.Size_Unit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AutoPaymentDepartmentId", product.Auto_Payment_Department_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AttachmentsCount", product.Attachments_Count);
                command.Parameters.AddWithValue("@ImageUrl", product.Image_Url ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Tax2", product.Tax2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SupplierCode", product.Supplier_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PackageProductsDetails", product.Package_Products_Details ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SiteorDisabled", product.Siteor_Disabled);
                command.Parameters.AddWithValue("@UseMoss", product.Use_Moss);
                command.Parameters.AddWithValue("@SubscriptionId", product.Subscription_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AccountingId", product.Accounting_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Status", product.Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@RestrictedToWarehouses", product.Restricted_To_Warehouses);
                command.Parameters.AddWithValue("@GtuCodes", (object)DBNull.Value);
                command.Parameters.AddWithValue("@TagList", (object)DBNull.Value);
                command.Parameters.AddWithValue("@GtuCode", product.Gtu_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ElectronicService", product.Electronic_Service ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsDelivery", product.Is_Delivery ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public static void InsertPayment(Payment payment, SqlConnection connection, SqlTransaction transaction, ILogger<ImportTask> log)
        {
            log.LogInformation($"Inserting payment {payment.Id} into the database.");

            var query = @"
            INSERT INTO Fakturownia_Payment (
                id, description, comment, invoice_comment, provider, provider_title, provider_status,
                paid, paid_date, price, currency, generate_invoice, invoice_name, invoice_tax_no, 
                post_code, city, street, country, email, phone, first_name, last_name, invoice_id, 
                client_id, department_id, product_id, user_id, token, name, oid, created_at, updated_at, 
                invoice_country, invoice_street, invoice_city, invoice_post_code, referrer, quantity, 
                promocode, deleted, field1, field2, field3, field4, field5, period, processed, 
                app_action_id, kind, auto_link_error, transfer_sender, transfer_recipient, import_details, 
                import_id, note, closed, creator_id, updater_id, match_status, code, payment_id, 
                import_md5, account_number, additional_discount, transaction_id, subscription_id, lang, 
                partner, income, transaction_kind, commission, use_moss, moss_notice, tax, attachments_count, 
                test, recurring, client_bank_account_number, bank_account_id, additional_fields, overpaid, 
                external_payment_id, cheque_number, card_number, bank, bank_account_balance, import_kind, 
                import_ref, invoice_company, no_duplicate_md5, additional_discount_amount, gocardless_payment_id, 
                payment_callback
            ) VALUES (
                @id, @description, @comment, @invoice_comment, @provider, @provider_title, @provider_status, 
                @paid, @paid_date, @price, @currency, @generate_invoice, @invoice_name, @invoice_tax_no, 
                @post_code, @city, @street, @country, @email, @phone, @first_name, @last_name, @invoice_id, 
                @client_id, @department_id, @product_id, @user_id, @token, @name, @oid, @created_at, @updated_at, 
                @invoice_country, @invoice_street, @invoice_city, @invoice_post_code, @referrer, @quantity, 
                @promocode, @deleted, @field1, @field2, @field3, @field4, @field5, @period, @processed, 
                @app_action_id, @kind, @auto_link_error, @transfer_sender, @transfer_recipient, @import_details, 
                @import_id, @note, @closed, @creator_id, @updater_id, @match_status, @code, @payment_id, 
                @import_md5, @account_number, @additional_discount, @transaction_id, @subscription_id, @lang, 
                @partner, @income, @transaction_kind, @commission, @use_moss, @moss_notice, @tax, @attachments_count, 
                @test, @recurring, @client_bank_account_number, @bank_account_id, @additional_fields, @overpaid, 
                @external_payment_id, @cheque_number, @card_number, @bank, @bank_account_balance, @import_kind, 
                @import_ref, @invoice_company, @no_duplicate_md5, @additional_discount_amount, @gocardless_payment_id, 
                @payment_callback
            );";

            using (var command = new SqlCommand(query, connection, transaction))
            {

                command.Parameters.AddWithValue("@id", payment.Id);
                command.Parameters.AddWithValue("@description", payment.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@comment", payment.Comment ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_comment", payment.Invoice_Comment ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@provider", payment.Provider ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@provider_title", payment.Provider_Title ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@provider_status", payment.Provider_Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@paid", payment.Paid ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@paid_date", payment.Paid_Date ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@price", payment.Price ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@currency", payment.Currency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@generate_invoice", payment.Generate_Invoice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_name", payment.Invoice_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_tax_no", payment.Invoice_Tax_No ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@post_code", payment.Post_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@city", payment.City ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@street", payment.Street ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@country", payment.Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email", payment.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@phone", payment.Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@first_name", payment.First_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@last_name", payment.Last_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_id", payment.Invoice_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@client_id", payment.Client_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@department_id", payment.Department_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@product_id", payment.Product_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@user_id", payment.User_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@token", payment.Token ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@name", payment.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@oid", payment.Oid ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@created_at", payment.Created_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@updated_at", payment.Updated_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_country", payment.Invoice_Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_street", payment.Invoice_Street ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_city", payment.Invoice_City ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_post_code", payment.Invoice_Post_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@referrer", payment.Referrer ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@quantity", payment.Quantity ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@promocode", payment.Promocode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@deleted", payment.Deleted ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@field1", payment.Field1 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@field2", payment.Field2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@field3", payment.Field3 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@field4", payment.Field4 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@field5", payment.Field5 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@period", payment.Period ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@processed", payment.Processed ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@app_action_id", payment.App_Action_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@kind", payment.Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@auto_link_error", payment.Auto_Link_Error ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@transfer_sender", payment.Transfer_Sender ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@transfer_recipient", payment.Transfer_Recipient ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@import_details", payment.Import_Details ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@import_id", payment.Import_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@note", payment.Note ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@closed", payment.Closed ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@creator_id", payment.Creator_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@updater_id", payment.Updater_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@match_status", payment.Match_Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@code", payment.Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@payment_id", payment.Payment_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@import_md5", payment.Import_Md5 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@account_number", payment.Account_Number ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@additional_discount", payment.Additional_Discount ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@transaction_id", payment.Transaction_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@subscription_id", payment.Subscription_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@lang", payment.Lang ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@partner", payment.Partner ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@income", payment.Income ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@transaction_kind", payment.Transaction_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@commission", payment.Commission ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@use_moss", payment.Use_Moss ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@moss_notice", payment.Moss_Notice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tax", payment.Tax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@attachments_count", payment.Attachments_Count ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@test", payment.Test ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@recurring", payment.Recurring ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@client_bank_account_number", payment.Client_Bank_Account_Number ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@bank_account_id", payment.Bank_Account_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@additional_fields", (object)DBNull.Value);
                command.Parameters.AddWithValue("@overpaid", payment.Overpaid ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@external_payment_id", payment.External_Payment_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cheque_number", payment.Cheque_Number ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@card_number", payment.Card_Number ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@bank", payment.Bank ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@bank_account_balance", payment.Bank_Account_Balance ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@import_kind", payment.Import_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@import_ref", payment.Import_Ref ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@invoice_company", payment.InvoiceCompany ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@no_duplicate_md5", payment.No_Duplicate_Md5 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@additional_discount_amount", payment.Additional_Discount_Amount ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gocardless_payment_id", payment.Gocardless_Payment_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@payment_callback", payment.Payment_Callback ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }

        }

        public static void InsertInvoiceHeader(Invoice invoice, SqlConnection connection, SqlTransaction transaction, ILogger<ImportTask> log)
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

            log.LogInformation($"Inserting invoice {invoice.Id} into the database.");

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
        public static void InsertClient(Client client, SqlConnection connection, SqlTransaction transaction, ILogger<ImportTask> log)
        {

            log.LogInformation($"Inserting client {client.Id} into the database.");

            var query = @"
        INSERT INTO Fakturownia_Client (
            id, name, tax_no, post_code, city, street, first_name, country, email, phone, www, fax, 
            created_at, updated_at, street_no, kind, bank, bank_account, bank_account_id, shortcut, note, 
            last_name, referrer, token, fuid, fname, femail, department_id, import, discount, payment_to_kind, 
            category_id, use_delivery_address, delivery_address, person, panel_user_id, use_mass_payment, 
            mass_payment_code, external_id, company, title, mobile_phone, register_number, tax_no_check, 
            attachments_count, default_payment_type, tax_no_kind, accounting_id, disable_auto_reminders, 
            buyer_id, price_list_id, panel_url
        ) VALUES (
            @Id, @Name, @TaxNo, @PostCode, @City, @Street, @FirstName, @Country, @Email, @Phone, @Www, @Fax, 
            @CreatedAt, @UpdatedAt, @StreetNo, @Kind, @Bank, @BankAccount, @BankAccountId, @Shortcut, @Note, 
            @LastName, @Referrer, @Token, @Fuid, @Fname, @Femail, @DepartmentId, @Import, @Discount, @PaymentToKind, 
            @CategoryId, @UseDeliveryAddress, @DeliveryAddress, @Person, @PanelUserId, @UseMassPayment, 
            @MassPaymentCode, @ExternalId, @Company, @Title, @MobilePhone, @RegisterNumber, @TaxNoCheck, 
            @AttachmentsCount, @DefaultPaymentType, @TaxNoKind, @AccountingId, @DisableAutoReminders, 
            @BuyerId, @PriceListId, @PanelUrl
        )";

            using (var command = new SqlCommand(query, connection, transaction))
            {

                command.Parameters.AddWithValue("@Id", client.Id);
                command.Parameters.AddWithValue("@Name", client.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TaxNo", client.Tax_No ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PostCode", client.Post_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@City", client.City ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Street", client.Street ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FirstName", client.First_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Country", client.Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Email", client.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Phone", client.Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Www", client.Www ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Fax", client.Fax ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedAt", client.Created_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedAt", client.Updated_At ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@StreetNo", client.Street_No ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Kind", client.Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Bank", client.Bank ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BankAccount", client.Bank_Account ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BankAccountId", client.Bank_Account_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Shortcut", client.Shortcut ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Note", client.Note ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LastName", client.Last_Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Referrer", client.Referrer ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Token", client.Token ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Fuid", client.Fuid ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Fname", client.Fname ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Femail", client.Femail ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DepartmentId", client.Department_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Import", client.Import ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Discount", client.Discount ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PaymentToKind", client.Payment_To_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CategoryId", client.Category_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UseDeliveryAddress", client.Use_Delivery_Address);
                command.Parameters.AddWithValue("@DeliveryAddress", client.Delivery_Address ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Person", client.Person ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PanelUserId", client.Panel_User_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UseMassPayment", client.Use_Mass_Payment ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MassPaymentCode", client.Mass_Payment_Code ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ExternalId", client.External_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Company", client.Company);
                command.Parameters.AddWithValue("@Title", client.Title ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MobilePhone", client.Mobile_Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@RegisterNumber", client.Register_Number ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AttachmentsCount", client.Attachments_Count ?? 0);
                command.Parameters.AddWithValue("@DefaultPaymentType", client.Default_Payment_Type ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TaxNoKind", client.Tax_No_Kind ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TaxNoCheck", client.Tax_No_Check ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AccountingId", client.Accounting_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DisableAutoReminders", client.Disable_Auto_Reminders);
                command.Parameters.AddWithValue("@BuyerId", client.Buyer_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PriceListId", client.Price_List_Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PanelUrl", client.Panel_Url ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }
    }
}
