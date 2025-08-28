USE OnlineStore;
GO

-- ===========================
-- Product constraints
-- ===========================
ALTER TABLE Product.Products
ADD CONSTRAINT CK_Products_Category
CHECK (Category IN ('Electronics', 'Grocery', 'Clothing'));

-- ===========================
-- Order constraints
-- ===========================
-- Order status (using lookup table, no duplicate status column)
ALTER TABLE [Order].Orders
ADD OrderStatusId INT NOT NULL 
    CONSTRAINT FK_Orders_OrderStatus
    REFERENCES [Order].OrderStatus(OrderStatusId);

-- Quantity > 0 for order items
ALTER TABLE [Order].OrderItems
ADD CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0);

-- Review rating 1–5
ALTER TABLE [Order].ProductReviews
ADD CONSTRAINT CK_ProductReviews_Rating CHECK (Rating BETWEEN 1 AND 5);

-- ===========================
-- Cart constraints
-- ===========================
ALTER TABLE Cart.CartItems
ADD CONSTRAINT CK_CartItems_Quantity CHECK (Quantity > 0);

