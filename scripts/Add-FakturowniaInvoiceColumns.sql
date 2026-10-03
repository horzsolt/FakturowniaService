-- Adds invoice JSON fields that are not yet on the Fakturownia tables.
-- Run in the database that contains these tables (VIR).
-- Safe to run more than once.

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'accounting_doc') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD accounting_doc bit NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'payment_status') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD payment_status nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'get_tax2_name') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD get_tax2_name nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'calculating_strategy_position') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD calculating_strategy_position nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'calculating_strategy_sum') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD calculating_strategy_sum nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'calculating_strategy_invoice_form_price_kind') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD calculating_strategy_invoice_form_price_kind nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'delivery_terms') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD delivery_terms nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'agreed_exchange_rate') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD agreed_exchange_rate decimal(18, 6) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'agreed_currency') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD agreed_currency nvarchar(10) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'intermediary_entity') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD intermediary_entity nvarchar(200) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'use_oss') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD use_oss bit NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'document_posted') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD document_posted bit NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'adjust_invoice_price') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD adjust_invoice_price decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'check_fiscal_print') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD check_fiscal_print bit NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'fiscal_print_error') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD fiscal_print_error nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'seller_bdo_no') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD seller_bdo_no nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'seller_ksef_taxpayer_status') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD seller_ksef_taxpayer_status nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'procedure_vat_margin') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD procedure_vat_margin nvarchar(200) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'gov_link') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD gov_link nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'gov_verification_link') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD gov_verification_link nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'payment_to_description') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD payment_to_description nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'buyer_jst') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD buyer_jst nvarchar(10) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'buyer_gv') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD buyer_gv nvarchar(10) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'bank_accounts') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD bank_accounts nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'issuers') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD issuers nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'recipients') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD recipients nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'descriptions') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD descriptions nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'transaction_contracts') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD transaction_contracts nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'transaction_orders') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD transaction_orders nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceHead', 'transaction_batches') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceHead ADD transaction_batches nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_InvoiceItem', 'technical_tax') IS NULL
    ALTER TABLE dbo.Fakturownia_InvoiceItem ADD technical_tax nvarchar(50) NULL;
