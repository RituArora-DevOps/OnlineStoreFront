using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.Repository
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateCreditCardAsync(decimal amount, string last4, string? brand, byte? expMonth, short? expYear);
        Task<Payment> CreatePayPalAsync(decimal amount, string? paypalEmail);
    }
}
