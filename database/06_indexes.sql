USE OnlineStore;
GO

-- ============================================
-- Indexes for OnlineStore
-- ============================================

/* =======================
   Product schema
   ======================= */
-- Often filtered/grouped by Category
CREATE INDEX IX_Products_Category 
    ON Product.Products(Category);

-- Reviews are usually fetched by ProductId
CREATE INDEX IX_ProductReviews_ProductId 
    ON Product.ProductReviews(ProductId);

-- If you expect many reviews per user, keep this too
CREATE INDEX IX_ProductReviews_ExternalUserId 
    ON Product.ProductReviews(ExternalUserId);


/* =======================
   Cart schema
   ======================= */
-- Fetch carts by user (auth system user)
CREATE INDEX IX_Carts_ExternalUserId 
    ON Cart.Carts(ExternalUserId);

-- Items lookups by CartId
CREATE INDEX IX_CartItems_CartId 
    ON Cart.CartItems(CartId);

-- To quickly find products inside carts
CREATE INDEX IX_CartItems_ProductId 
    ON Cart.CartItems(ProductId);


/* =======================
   Order schema
   ======================= */
-- Common query: filter by status
CREATE INDEX IX_Orders_OrderStatusId 
    ON [Order].Orders(OrderStatusId);

-- Fetch orders by user
CREATE INDEX IX_Orders_ExternalUserId 
    ON [Order].Orders(ExternalUserId);

-- If you often query "orders per user by status"
CREATE INDEX IX_Orders_User_Status 
    ON [Order].Orders(ExternalUserId, OrderStatusId);

-- Join orders ↔ payments
CREATE INDEX IX_Orders_PaymentId 
    ON [Order].Orders(PaymentId);

-- OrderItems lookups
CREATE INDEX IX_OrderItems_OrderId 
    ON [Order].OrderItems(OrderId);

CREATE INDEX IX_OrderItems_ProductId 
    ON [Order].OrderItems(ProductId);
