USE OnlineStore;
GO

-- Drop tables if they exist (children before parents to avoid FK conflicts)

-- Payment schema
IF OBJECT_ID('Payment.PayPalPayments', 'U') IS NOT NULL DROP TABLE Payment.PayPalPayments;
IF OBJECT_ID('Payment.CreditCardPayments', 'U') IS NOT NULL DROP TABLE Payment.CreditCardPayments;
IF OBJECT_ID('Payment.Payments', 'U') IS NOT NULL DROP TABLE Payment.Payments;

-- Sales schema (Orders + Order Items + Order Status)
IF OBJECT_ID('Sales.OrderItems', 'U') IS NOT NULL DROP TABLE Sales.OrderItems;
IF OBJECT_ID('Sales.Orders', 'U') IS NOT NULL DROP TABLE Sales.Orders;
IF OBJECT_ID('Sales.OrderStatus', 'U') IS NOT NULL DROP TABLE Sales.OrderStatus;

-- Cart schema
IF OBJECT_ID('Cart.CartItems', 'U') IS NOT NULL DROP TABLE Cart.CartItems;
IF OBJECT_ID('Cart.Carts', 'U') IS NOT NULL DROP TABLE Cart.Carts;

-- Product schema
IF OBJECT_ID('Product.ProductReviews', 'U') IS NOT NULL DROP TABLE Product.ProductReviews;
IF OBJECT_ID('Product.Pictures', 'U') IS NOT NULL DROP TABLE Product.Pictures;
IF OBJECT_ID('Product.Clothings', 'U') IS NOT NULL DROP TABLE Product.Clothings;
IF OBJECT_ID('Product.Groceries', 'U') IS NOT NULL DROP TABLE Product.Groceries;
IF OBJECT_ID('Product.Electronics', 'U') IS NOT NULL DROP TABLE Product.Electronics;
IF OBJECT_ID('Product.Products', 'U') IS NOT NULL DROP TABLE Product.Products;
GO
