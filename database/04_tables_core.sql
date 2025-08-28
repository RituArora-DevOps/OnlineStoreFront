USE OnlineStore;
GO

/* =======================
   Product schema
   ======================= */
CREATE TABLE Product.Products (
    ProductId     INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    Price         DECIMAL(10,2) NOT NULL,
    Category      NVARCHAR(50)  NOT NULL,
    Name          NVARCHAR(100) NOT NULL,
    Description   NVARCHAR(255),
    CreatedDate   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedDate  DATETIME2 NULL  DEFAULT NULL
);

CREATE TABLE Product.Electronics (
    ProductId      INT PRIMARY KEY NOT NULL,
    WarrantyPeriod INT DEFAULT 12,
    FOREIGN KEY (ProductId) REFERENCES Product.Products(ProductId) ON DELETE CASCADE
);

CREATE TABLE Product.Grocery (
    ProductId      INT PRIMARY KEY NOT NULL,
    ExpirationDate DATE NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Product.Products(ProductId) ON DELETE CASCADE
);

CREATE TABLE Product.Clothing (
    ProductId INT PRIMARY KEY NOT NULL,
    Size      NVARCHAR(20),
    Color     NVARCHAR(50),
    FOREIGN KEY (ProductId) REFERENCES Product.Products(ProductId) ON DELETE CASCADE
);

CREATE TABLE Product.Pictures (
    PictureId     INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ProductId     INT NOT NULL,
    PictureName   NVARCHAR(40)  NOT NULL,
    PicFileName   NVARCHAR(100),
    PictureData   VARBINARY(MAX),
    CreatedDate   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedDate  DATETIME2 NULL  DEFAULT NULL,
    FOREIGN KEY (ProductId) REFERENCES Product.Products(ProductId) ON DELETE CASCADE
);

CREATE TABLE Product.ProductReviews (
    ReviewId     INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ProductId    INT NOT NULL,
    ExternalUserId NVARCHAR(128) NOT NULL,  -- from auth system
    Rating       INT
    Comment      NVARCHAR(1000),
    CreatedDate  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedDate DATETIME2 NULL  DEFAULT NULL,
    FOREIGN KEY (ProductId) REFERENCES Product.Products(ProductId)
);

/* =======================
   Cart schema
   ======================= */
CREATE TABLE Cart.Carts (
    CartId       INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ExternalUserId NVARCHAR(128) NOT NULL,  -- owner from auth system
    CreatedDate  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedDate DATETIME2 NULL  DEFAULT NULL
);

CREATE TABLE Cart.CartItems (
    CartItemId   INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    CartId       INT NOT NULL,
    ProductId    INT NOT NULL,
    Quantity     INT NOT NULL DEFAULT 1,
    CreatedDate  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedDate DATETIME2 NULL  DEFAULT NULL,
    FOREIGN KEY (CartId)    REFERENCES Cart.Carts(CartId)         ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Product.Products(ProductId)
);

/* =======================
   Payment schema
   ======================= */
CREATE TABLE Payment.Payments (
    PaymentId    INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    Amount       DECIMAL(10,2) NOT NULL,
    CreatedDate  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedDate DATETIME2 NULL  DEFAULT NULL
);

CREATE TABLE Payment.CreditCardPayments (
    PaymentId     INT PRIMARY KEY NOT NULL,
    -- CardNumber    NVARCHAR(16) NOT NULL,  --  Prefer token/last4; avoid storing full PAN
    -- ExpirationDate NVARCHAR(5),
    -- CVV           NVARCHAR(4),            --  DO NOT STORE CVV in production (PCI DSS)
    -- FIXME: Use tokenization for card details in real systems - only if we integrate any payment gateway
    -- CardToken NVARCHAR(255) NOT NULL,     -- tokenized card ref from gateway (e.g., Stripe) 
    Last4 NVARCHAR(4) NOT NULL,           -- last 4 digits only
    CardBrand NVARCHAR(20) NULL,          -- e.g., 'Visa', 'Mastercard'
    ExpirationMonth TINYINT NULL,
    ExpirationYear SMALLINT NULL,
    CreatedDate   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    FOREIGN KEY (PaymentId) REFERENCES Payment.Payments(PaymentId) ON DELETE CASCADE
);

CREATE TABLE Payment.PayPalPayments (
    PaymentId    INT PRIMARY KEY NOT NULL,
    PayPalEmail  NVARCHAR(100),
    CreatedDate  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    FOREIGN KEY (PaymentId) REFERENCES Payment.Payments(PaymentId) ON DELETE CASCADE
);

/* =======================
   Order schema
   ======================= */
CREATE TABLE [Order].OrderStatus (
    OrderStatusId INT IDENTITY(1,1) PRIMARY KEY,
    Status NVARCHAR(20) UNIQUE NOT NULL
);

INSERT INTO [Order].OrderStatus (Status) VALUES ('Pending'), ('Shipped'), ('Delivered'), ('Cancelled');

CREATE TABLE [Order].Orders (
    OrderId       BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ExternalUserId NVARCHAR(128) NOT NULL,  -- customer from auth system
    PaymentId     INT NULL,
    OrderStatusId  INT NOT NULL DEFAULT 1,  -- default 'Pending'
    CreatedDate   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedDate  DATETIME2 NULL  DEFAULT NULL,
    CONSTRAINT FK_Orders_Payment     FOREIGN KEY (PaymentId) REFERENCES Payment.Payments(PaymentId),
    CONSTRAINT FK_Orders_OrderStatus FOREIGN KEY (Status)    REFERENCES [Order].OrderStatus(OrderStatusId)
);

CREATE TABLE [Order].OrderItems (
    OrderItemId   BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    OrderId       BIGINT NOT NULL,
    ProductId     INT NOT NULL,
    Quantity      INT NOT NULL DEFAULT 1,
    PriceAtOrder  DECIMAL(10,2) NOT NULL,
    CreatedDate   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedDate  DATETIME2 NULL  DEFAULT NULL,
    FOREIGN KEY (OrderId)  REFERENCES [Order].Orders(OrderId)       ON DELETE CASCADE,
    FOREIGN KEY (ProductId)REFERENCES Product.Products(ProductId)
);


