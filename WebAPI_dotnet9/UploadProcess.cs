using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebAPI_dotnet9
{
    public static class UploadProcess
    {
        public static async Task<bool> Upload(IFormFile file)
        {

            await Task.Delay(30000);
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");


            var filePath = Path.Combine(uploadsDirectory, file.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);                
                    
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                return false;
            }
            

            return true;

        }

    }
}
