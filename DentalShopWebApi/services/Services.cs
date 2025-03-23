
namespace DentalShopWebApi.AllServices
{


    using System;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Formats.Png;
    using System.Threading.Tasks;

    public class Services
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Services(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveImageAsync(string base64Image, Guid id, string suffix , string folderName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64Image))
                    return null;

                // Convert base64 string to byte array
                byte[] pictureBytes = Convert.FromBase64String(base64Image);

                // Define file path
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", folderName);
                Directory.CreateDirectory(folderPath); // Ensure directory exists
                string filePath = Path.Combine(folderPath, $"{id}_{suffix}.png");

                // Save image using ImageSharp
                using (var image = Image.Load(pictureBytes))
                {
                    // Optional: Resize or apply processing
                    image.Mutate(x => x.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(500, 500) }));

                    await image.SaveAsync(filePath, new PngEncoder());
                }

                // Return the relative file path (for storing in DB or returning to the client)
                return $"/images/{folderName}/{id}_{suffix}.png";
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> DeleteImageAsync(string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                    return false;

                // Construct full file path
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }

                return false; // File not found
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error deleting image: {ex.Message}");
                return false;
            }
        }


    }

}