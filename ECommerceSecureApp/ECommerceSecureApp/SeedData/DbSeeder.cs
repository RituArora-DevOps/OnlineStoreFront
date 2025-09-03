using Bogus;
using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.SeedData
{
    public class DbSeeder
    {

        public static void Seed(OnlineStoreDbContext context)
        {
            List<Product> products;

            // Seed Products
            if (!context.Products.Any())
            {
                Console.WriteLine("Seeding Products...");
                products = GenerateFakeProducts(50);
                context.Products.AddRange(products);
                context.SaveChanges();

                var savedProducts = context.Products.ToList();

                context.Pictures.AddRange(GenerateFakePictures(savedProducts));
                context.Clothings.AddRange(GenerateClothing(savedProducts));
                context.Electronics.AddRange(GenerateElectronics(savedProducts));
                context.Groceries.AddRange(GenerateGroceries(savedProducts));
                context.SaveChanges();

                products = savedProducts;
            }
            else
            {
                products = context.Products.ToList();
            }

            // Seed Carts
            if (!context.Carts.Any())
            {
                Console.WriteLine("Seeding Carts...");
                var carts = GenerateCarts(products);
                context.Carts.AddRange(carts);
                context.SaveChanges();
            }

            // Seed Payments and Subtypes
            List<Payment> payments;
            if (!context.Payments.Any())
            {
                Console.WriteLine("Seeding Payments...");
                var (paymentsGenerated, creditCards, paypals) = GeneratePayments();

                context.Payments.AddRange(paymentsGenerated);
                context.CreditCardPayments.AddRange(creditCards);
                context.PayPalPayments.AddRange(paypals);
                context.SaveChanges();

                payments = context.Payments.ToList(); // Get real PaymentIds
            }
            else
            {
                payments = context.Payments.ToList();
            }

            // Seed Orders
            //if (!context.Orders.Any())
            //{
            //    Console.WriteLine("Seeding Orders...");
            //    var orders = GenerateOrders(products, payments);
            //    context.Orders.AddRange(orders);
            //    context.SaveChanges();

            //    var savedOrders = context.Orders.ToList();

            //    var orderItemFaker = new Faker<OrderItem>()
            //        .RuleFor(oi => oi.Quantity, f => f.Random.Int(1, 3))
            //        .RuleFor(oi => oi.PriceAtOrder, f => f.Finance.Amount(10, 300))
            //        .RuleFor(oi => oi.CreatedDate, f => f.Date.Past(1))
            //        .RuleFor(oi => oi.ModifiedDate, f => f.Date.Recent());

            //    var orderItems = new List<OrderItem>();

            //    foreach (var order in savedOrders)
            //    {
            //        var items = products.OrderBy(_ => Guid.NewGuid()).Take(2).ToList();
            //        foreach (var product in items)
            //        {
            //            var item = orderItemFaker.Generate();
            //            item.ProductId = product.ProductId;
            //            item.OrderId = order.OrderId;
            //            orderItems.Add(item);
            //        }
            //    }

            //    context.OrderItems.AddRange(orderItems);
            //    context.SaveChanges();
            //}

            // Seed Reviews
            if (!context.ProductReviews.Any())
            {
                Console.WriteLine("Seeding Reviews...");
                var reviews = GenerateReviews(products);
                context.ProductReviews.AddRange(reviews);
                context.SaveChanges();
            }
            Console.WriteLine("Seeded product count: " + context.Products.Count());

            Console.WriteLine(" Seeding completed successfully.");
        }


        public static List<Product> GenerateFakeProducts(int count = 100)
        {
            var categories = new[] { "Electronics", "Clothing", "Grocery" };

            var faker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => f.Random.Decimal(5, 500))
                .RuleFor(p => p.Category, f => f.PickRandom(categories))
                .RuleFor(p => p.CreatedDate, f => f.Date.Past(1))
                .RuleFor(p => p.ModifiedDate, f => f.Date.Recent());

            return faker.Generate(count);
        }

        public static List<Picture> GenerateFakePictures(List<Product> products, int picturesPerProduct = 2)
        {
            var faker = new Faker<Picture>()
                .RuleFor(p => p.PictureName, f => f.Commerce.ProductName())
                .RuleFor(p => p.PicFileName, f => f.System.FileName("jpg"))
                .RuleFor(p => p.PictureData, f => f.Random.Bytes(256)) // Simulated image data
                .RuleFor(p => p.CreatedDate, f => f.Date.Past(1))
                .RuleFor(p => p.ModifiedDate, f => f.Date.Recent());

            var pictures = new List<Picture>();

            foreach (var product in products)
            {
                for (int i = 0; i < picturesPerProduct; i++)
                {
                    var pic = faker.Generate();
                    pic.ProductId = product.ProductId;
                    pictures.Add(pic);
                }
            }

            return pictures;
        }

        public static List<Clothing> GenerateClothing(List<Product> products)
        {
            var faker = new Faker<Clothing>()
                .RuleFor(c => c.Size, f => f.PickRandom(new[] { "S", "M", "L", "XL" }))
                .RuleFor(c => c.Color, f => f.Commerce.Color());

            return products
                .Where(p => p.Category == "Clothing")
                .Select(p =>
                {
                    var item = faker.Generate();
                    item.ProductId = p.ProductId;
                    return item;
                }).ToList();
        }

        public static List<Electronic> GenerateElectronics(List<Product> products)
        {
            var faker = new Faker<Electronic>()
                .RuleFor(e => e.WarrantyPeriod, f => f.Random.Int(6, 36)); // months

            return products
                .Where(p => p.Category == "Electronics")
                .Select(p =>
                {
                    var item = faker.Generate();
                    item.ProductId = p.ProductId;
                    return item;
                }).ToList();
        }

        public static List<Grocery> GenerateGroceries(List<Product> products)
        {
            var faker = new Faker<Grocery>()
                .RuleFor(g => g.ExpirationDate, f => DateOnly.FromDateTime(f.Date.Future(1)));

            return products
                .Where(p => p.Category == "Grocery")
                .Select(p =>
                {
                    var item = faker.Generate();
                    item.ProductId = p.ProductId;
                    return item;
                }).ToList();
        }

        public static List<Cart> GenerateCarts(List<Product> products, int cartCount = 10)
        {
            var faker = new Faker<Cart>()
                .RuleFor(c => c.ExternalUserId, f => f.Random.Guid().ToString())
                .RuleFor(c => c.CreatedDate, f => f.Date.Past(1))
                .RuleFor(c => c.ModifiedDate, f => f.Date.Recent());

            var carts = faker.Generate(cartCount);

            var cartItemsFaker = new Faker<CartItem>()
                .RuleFor(ci => ci.Quantity, f => f.Random.Int(1, 5))
                .RuleFor(ci => ci.CreatedDate, f => f.Date.Past(1))
                .RuleFor(ci => ci.ModifiedDate, f => f.Date.Recent());

            foreach (var cart in carts)
            {
                var items = products.OrderBy(_ => Guid.NewGuid()).Take(3).ToList();
                foreach (var product in items)
                {
                    var item = cartItemsFaker.Generate();
                    item.ProductId = product.ProductId;
                    item.Cart = cart;
                    cart.CartItems.Add(item);
                }
            }

            return carts;
        }

        public static (List<Payment> payments, List<CreditCardPayment> creditCards, List<PayPalPayment> paypals) GeneratePayments(int count = 10)
        {
            var payments = new List<Payment>();
            var creditCards = new List<CreditCardPayment>();
            var paypals = new List<PayPalPayment>();

            var paymentFaker = new Faker<Payment>()
                .RuleFor(p => p.Amount, f => f.Finance.Amount(20, 500))
                .RuleFor(p => p.CreatedDate, f => f.Date.Past(1))
                .RuleFor(p => p.ModifiedDate, f => f.Date.Recent());

            var ccFaker = new Faker<CreditCardPayment>()
                .RuleFor(cc => cc.Last4, f => f.Random.Replace("####"))
                .RuleFor(cc => cc.CardBrand, f => f.PickRandom(new[] { "Visa", "MasterCard", "Amex", "Discover" }))
                .RuleFor(cc => cc.ExpirationMonth, f => (byte)f.Date.Future().Month)
                .RuleFor(cc => cc.ExpirationYear, f => (short)f.Date.Future().Year)
                .RuleFor(cc => cc.CreatedDate, f => f.Date.Past(1));

            var paypalFaker = new Faker<PayPalPayment>()
                .RuleFor(pp => pp.PayPalEmail, f => f.Internet.Email())
                .RuleFor(pp => pp.CreatedDate, f => f.Date.Past(1));

            for (int i = 0; i < count; i++)
            {
                var payment = paymentFaker.Generate();
                payment.PaymentId = 0; // Let EF generate it
                payments.Add(payment);

                if (i % 2 == 0)
                {
                    var cc = ccFaker.Generate();
                    cc.Payment = payment;
                    creditCards.Add(cc);
                }
                else
                {
                    var pp = paypalFaker.Generate();
                    pp.Payment = payment;
                    paypals.Add(pp);
                }
            }

            return (payments, creditCards, paypals);
        }

        //public static List<Order> GenerateOrders(List<Product> products, List<Payment> payments, int count = 10)
        //{
        //    var faker = new Faker<Order>()
        //        .RuleFor(o => o.ExternalUserId, f => f.Random.Guid().ToString())
        //        .RuleFor(o => o.OrderStatusId, f => f.PickRandom(new[] { 1, 2, 3, 4 }))
        //        .RuleFor(o => o.CreatedDate, f => f.Date.Past(1))
        //        .RuleFor(o => o.ModifiedDate, f => f.Date.Recent());

        //    var orders = faker.Generate(count);

        //    for (int i = 0; i < orders.Count; i++)
        //    {
        //        orders[i].PaymentId = payments[i % payments.Count].PaymentId;
        //    }

        //    return orders;
        //}

        public static List<ProductReview> GenerateReviews(List<Product> products, int count = 30)
        {
            var faker = new Faker<ProductReview>()
                .RuleFor(r => r.ExternalUserId, f => f.Random.Guid().ToString())
                .RuleFor(r => r.Rating, f => f.Random.Int(1, 5))
                .RuleFor(r => r.Comment, f => f.Lorem.Sentences(2))
                .RuleFor(r => r.CreatedDate, f => f.Date.Past(1))
                .RuleFor(r => r.ModifiedDate, f => f.Date.Recent());

            var reviews = new List<ProductReview>();

            foreach (var product in products.OrderBy(_ => Guid.NewGuid()).Take(count))
            {
                var review = faker.Generate();
                review.ProductId = product.ProductId;
                reviews.Add(review);
            }

            return reviews;
        }


    }
}
