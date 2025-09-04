using ECommerceSecureApp.Models;

namespace ECommerceSecureApp.Repository
{
    public interface IPictureRepository
    {
        Task<Picture?> GetFirstByProductIdAsync(int productId);
    }
}
