namespace ECommerceSecureApp.Services.Coupons
{
    public class InMemoryCouponService : ICouponService
    {
        private readonly Dictionary<string, decimal> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            { "SAVE10", 10m },
            { "SAVE20", 20m },
            { "WELCOME5", 5m }
        };

        public decimal ResolveCouponAmount(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return 0m;
            return _map.TryGetValue(code.Trim(), out var amt) ? amt : 0m;
        }
    }
}
