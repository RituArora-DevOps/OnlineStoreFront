USE OnlineStore;
GO

-- Product Reviews
ALTER TABLE Product.ProductReviews
ALTER COLUMN ExternalUserId NVARCHAR(128) NULL;

-- Carts
ALTER TABLE Cart.Carts
ALTER COLUMN ExternalUserId NVARCHAR(128) NULL;

-- Orders
ALTER TABLE [Order].Orders
ALTER COLUMN ExternalUserId NVARCHAR(128) NULL;