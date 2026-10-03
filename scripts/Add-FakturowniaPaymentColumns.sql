-- Adds payment JSON fields that are not yet on Fakturownia_Payment.
-- The existing paid column stays bit. paid_amount stores the decimal from the JSON.
-- Run in the database that contains this table (VIR).
-- Safe to run more than once.

IF COL_LENGTH('dbo.Fakturownia_Payment', 'paid_amount') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD paid_amount decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'number') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD number nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'issue_date') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD issue_date date NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'payment_to') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD payment_to date NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'payment_to_kind') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD payment_to_kind nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'payment_type') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD payment_type nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'sell_date') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD sell_date date NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'sell_date_kind') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD sell_date_kind nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'place') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD place nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'price_gross') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD price_gross decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'price_net') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD price_net decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'price_tax') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD price_tax decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_name') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_name nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_street') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_street nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_post_code') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_post_code nvarchar(20) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_city') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_city nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_country') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_country nvarchar(10) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_bank') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_bank nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_bank_account') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_bank_account nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_bank_account_id') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_bank_account_id bigint NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_email') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_email nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_fax') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_fax nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_person') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_person nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_phone') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_phone nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_tax_no') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_tax_no nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_tax_no_kind') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_tax_no_kind nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_www') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_www nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'delivery_address') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD delivery_address nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'description_footer') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD description_footer nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'description_long') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD description_long nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'discount') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD discount decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'discount_kind') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD discount_kind nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'exchange_currency') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD exchange_currency nvarchar(10) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'exchange_currency_rate') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD exchange_currency_rate decimal(18, 6) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'exchange_date') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD exchange_date date NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'exchange_kind') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD exchange_kind nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'exchange_note') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD exchange_note nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'exchange_rate') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD exchange_rate decimal(18, 6) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_company') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_company bit NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_name') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_name nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_street') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_street nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_post_code') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_post_code nvarchar(20) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_city') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_city nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_country') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_country nvarchar(10) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_bank') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_bank nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_bank_account') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_bank_account nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_email') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_email nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_fax') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_fax nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_person') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_person nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_phone') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_phone nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_tax_no') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_tax_no nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_tax_no_kind') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_tax_no_kind nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_www') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_www nvarchar(100) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'show_discount') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD show_discount bit NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'split_payment') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD split_payment int NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'buyer_mobile_phone') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD buyer_mobile_phone nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_bdo_no') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_bdo_no nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'seller_ksef_taxpayer_status') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD seller_ksef_taxpayer_status nvarchar(50) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'e_receipt_view_url') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD e_receipt_view_url nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'positions') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD positions nvarchar(max) NULL;

IF COL_LENGTH('dbo.Fakturownia_Payment', 'descriptions') IS NULL
    ALTER TABLE dbo.Fakturownia_Payment ADD descriptions nvarchar(max) NULL;
