USE OnlineStore;
GO

/* ============================================
   Update Triggers for ModifiedDate
   ============================================ */

-- Products
CREATE TRIGGER trg_products_modified
ON Product.Products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p
    SET ModifiedDate = SYSDATETIME()
    FROM Product.Products p
    INNER JOIN inserted i ON p.ProductId = i.ProductId;
END;
GO

-- Product Reviews
CREATE TRIGGER trg_productreviews_modified
ON Product.ProductReviews
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE pr
    SET ModifiedDate = SYSDATETIME()
    FROM Product.ProductReviews pr
    INNER JOIN inserted i ON pr.ReviewId = i.ReviewId;
END;
GO

-- Carts
CREATE TRIGGER trg_carts_modified
ON Cart.Carts
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c
    SET ModifiedDate = SYSDATETIME()
    FROM Cart.Carts c
    INNER JOIN inserted i ON c.CartId = i.CartId;
END;
GO

-- Cart Items
CREATE TRIGGER trg_cartitems_modified
ON Cart.CartItems
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ci
    SET ModifiedDate = SYSDATETIME()
    FROM Cart.CartItems ci
    INNER JOIN inserted i ON ci.CartItemId = i.CartItemId;
END;
GO

-- Orders
CREATE TRIGGER trg_orders_modified
ON [Order].Orders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE o
    SET ModifiedDate = SYSDATETIME()
    FROM [Order].Orders o
    INNER JOIN inserted i ON o.OrderId = i.OrderId;
END;
GO

-- Order Items
CREATE TRIGGER trg_orderitems_modified
ON [Order].OrderItems
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE oi
    SET ModifiedDate = SYSDATETIME()
    FROM [Order].OrderItems oi
    INNER JOIN inserted i ON oi.OrderItemId = i.OrderItemId;
END;
GO

-- Payments
CREATE TRIGGER trg_payments_modified
ON Payment.Payments
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p
    SET ModifiedDate = SYSDATETIME()
    FROM Payment.Payments p
    INNER JOIN inserted i ON p.PaymentId = i.PaymentId;
END;
GO
