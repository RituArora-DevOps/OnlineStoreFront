namespace ECommerceSecureApp.Services.Coupons
{
    public interface ICouponService
    {
        decimal ResolveCouponAmount(string? code);
    }
}
