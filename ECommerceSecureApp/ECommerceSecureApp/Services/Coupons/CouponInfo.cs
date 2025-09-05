namespace ECommerceSecureApp.Services.Coupons
{
    public sealed class CouponInfo
    {
        public string Code { get; init; } = string.Empty;
        public decimal AmountOff { get; init; } = 0m;     // fixed $ off per unit
        public decimal PercentOff { get; init; } = 0m;    // percent 0..100 per unit
        public bool IsValid => AmountOff > 0 || PercentOff > 0;
    }
}
