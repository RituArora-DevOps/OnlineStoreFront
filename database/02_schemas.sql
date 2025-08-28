USE OnlineStore;
GO

-- =========================================================
-- Schemas
-- =========================================================
IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'Product') DROP SCHEMA Product;
IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'Cart') DROP SCHEMA Cart;
IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'Order') DROP SCHEMA [Order];
IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'Payment') DROP SCHEMA Payment;
GO

CREATE SCHEMA Product AUTHORIZATION dbo;
CREATE SCHEMA Cart AUTHORIZATION dbo;
CREATE SCHEMA [Order] AUTHORIZATION dbo;
CREATE SCHEMA Payment AUTHORIZATION dbo;
GO
