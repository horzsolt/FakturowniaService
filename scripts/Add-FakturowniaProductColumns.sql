-- Adds product JSON fields that are not yet on Fakturownia_Product.
-- Run in the database that contains this table (VIR).
-- Safe to run more than once.

IF COL_LENGTH('dbo.Fakturownia_Product', 'purchase_price_net') IS NULL
    ALTER TABLE dbo.Fakturownia_Product ADD purchase_price_net decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_Product', 'purchase_price_gross') IS NULL
    ALTER TABLE dbo.Fakturownia_Product ADD purchase_price_gross decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_Product', 'purchase_price_tax') IS NULL
    ALTER TABLE dbo.Fakturownia_Product ADD purchase_price_tax decimal(18, 2) NULL;

IF COL_LENGTH('dbo.Fakturownia_Product', 'purchase_tax') IS NULL
    ALTER TABLE dbo.Fakturownia_Product ADD purchase_tax nvarchar(10) NULL;

IF COL_LENGTH('dbo.Fakturownia_Product', 'purchase_tax2') IS NULL
    ALTER TABLE dbo.Fakturownia_Product ADD purchase_tax2 nvarchar(10) NULL;
