using CitySecrets.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CitySecrets.Services
{
    public class FileUploadService : IFileUploadService
    {
        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            if (!ValidateImageAsync(file).Result)
                throw new Exception("Invalid image");

            var fileName = GenerateUniqueFileName(file.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return path;
        }

        public async Task<IEnumerable<string>> UploadMultipleImagesAsync(IEnumerable<IFormFile> files, string folder)
        {
            var paths = new List<string>();
            foreach (var file in files)
                paths.Add(await UploadImageAsync(file, folder));

            return paths;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (!File.Exists(imageUrl)) return false;
            File.Delete(imageUrl);
            return true;
        }

        public async Task<bool> ValidateImageAsync(IFormFile file)
        {
            return file.Length > 0 && file.ContentType.StartsWith("image/");
        }

        public string GenerateUniqueFileName(string originalFileName)
        {
            return $"{Guid.NewGuid()}_{originalFileName}";
        }
    }
}
