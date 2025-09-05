using System.Globalization;

namespace ECommerceSecureApp.Services.Coupons
{
    public class InMemoryCouponService : ICouponService
    {
        // Examples:
        // SAVE10  => $10 off
        // SAVE20  => $20 off
        // SAVE15P => 15% off
        private readonly Dictionary<string, CouponInfo> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            { "SAVE10", new CouponInfo{ Code="SAVE10", AmountOff=10m } },
            { "SAVE20", new CouponInfo{ Code="SAVE20", AmountOff=20m } },
            { "SAVE15P", new CouponInfo{ Code="SAVE15P", PercentOff=15m } },
            // add more here as needed
        };

        public CouponInfo? Resolve(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            if (_map.TryGetValue(code.Trim(), out var info))
                return info;

            // Optional: support dynamic percent like SAVE10P, SAVE25P
            if (code.EndsWith("P", StringComparison.OrdinalIgnoreCase))
            {
                var pctText = code[..^1].Trim().TrimStart('S', 'A', 'V', 'E'); // crude parse "SAVE##P"
                if (decimal.TryParse(pctText, NumberStyles.Number, CultureInfo.InvariantCulture, out var pct) && pct > 0)
                {
                    return new CouponInfo { Code = code.Trim(), PercentOff = pct };
                }
            }
            return null;
        }
    }
}
