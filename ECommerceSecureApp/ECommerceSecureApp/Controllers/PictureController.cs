using ECommerceSecureApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Controllers
{
    public class PictureController : Controller
    {

        private readonly IPictureRepository _pictureRepository;

        public PictureController(IPictureRepository pictureRepository)
        {
            _pictureRepository = pictureRepository;
        }

        [Route("Picture/ProductImage/{productId}")]
        public async Task<IActionResult> ProductImage(int productId)
        {
            var picture = await _pictureRepository.GetFirstByProductIdAsync(productId);

            if (picture?.PictureData != null)
            {
                return File(picture.PictureData, "image/jpeg");
            }

            var fallbackPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products/no-image.png");
            var fallbackBytes = System.IO.File.ReadAllBytes(fallbackPath);
            return File(fallbackBytes, "image/png");
        }

    }
}
