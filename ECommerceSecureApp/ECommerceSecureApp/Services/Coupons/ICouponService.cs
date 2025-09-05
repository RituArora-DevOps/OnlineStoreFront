namespace ECommerceSecureApp.Services.Coupons
{
    public interface ICouponService
    {
        // Return coupon info; if unknown/invalid, return null or Info with IsValid=false
        CouponInfo? Resolve(string? code);
    }
}
