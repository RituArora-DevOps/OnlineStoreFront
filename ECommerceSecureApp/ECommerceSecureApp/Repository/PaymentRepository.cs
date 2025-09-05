using ECommerceSecureApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly OnlineStoreDbContext _context;
        public PaymentRepository(OnlineStoreDbContext context) { _context = context; }

        public async Task<Payment> CreateCreditCardAsync(decimal amount, string last4, string? brand, byte? expMonth, short? expYear)
        {
            var payment = new Payment { Amount = amount, CreatedDate = DateTime.UtcNow }; // Payment.Amount
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var cc = new CreditCardPayment
            {
                PaymentId = payment.PaymentId,
                Last4 = last4,
                CardBrand = brand,
                ExpirationMonth = expMonth,
                ExpirationYear = expYear,
                CreatedDate = DateTime.UtcNow
            };
            _context.CreditCardPayments.Add(cc); // CreditCardPayment
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> CreatePayPalAsync(decimal amount, string? paypalEmail)
        {
            var payment = new Payment { Amount = amount, CreatedDate = DateTime.UtcNow };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var pp = new PayPalPayment
            {
                PaymentId = payment.PaymentId,
                PayPalEmail = paypalEmail,
                CreatedDate = DateTime.UtcNow
            };
            _context.PayPalPayments.Add(pp); // PayPalPayment
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
