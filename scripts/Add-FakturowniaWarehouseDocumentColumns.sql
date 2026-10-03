-- Adds warehouse document JSON fields that are not yet on Fakturownia_WarehouseDocument.
-- Run in the database that contains this table (VIR).
-- Safe to run more than once.

IF COL_LENGTH('dbo.Fakturownia_WarehouseDocument', 'warehouse_actions') IS NULL
    ALTER TABLE dbo.Fakturownia_WarehouseDocument ADD warehouse_actions nvarchar(max) NULL;
