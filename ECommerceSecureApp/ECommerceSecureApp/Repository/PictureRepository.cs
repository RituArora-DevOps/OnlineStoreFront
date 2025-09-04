using ECommerceSecureApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Repository
{
    public class PictureRepository : IPictureRepository
    {
        private readonly OnlineStoreDbContext _context;

        public PictureRepository(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public async Task<Picture?> GetFirstByProductIdAsync(int productId)
        {
            return await _context.Pictures
                .Where(p => p.ProductId == productId)
                .FirstOrDefaultAsync();
        }
    }

}
