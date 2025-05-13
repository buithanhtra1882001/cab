using System.Text;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;

namespace CabMediaService.Infrastructures.Extensions
{
    public static class FormFileExtensions
    {
        public const int MinimumBytes = 512;

        public static bool IsValidImage(this IFormFile file)
        {
            // Check the image mime types
            var allowedContentTypes = new[] {
                "image/jpg",
                "image/jpeg",
                "image/pjpeg",
                "image/x-png",
                "image/png",
                "image/apng",
                "image/heic",
                "image/gif"
            };
            if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
            {
                return false;
            }

            // Check the image extension
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                return false;
            }

            try
            {
                if (!file.OpenReadStream().CanRead)
                {
                    return false;
                }

                if (file.Length < MinimumBytes)
                {
                    return false;
                }

                var buffer = new byte[MinimumBytes];
                file.OpenReadStream().Read(buffer, 0, MinimumBytes);

                var content = Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            // Try to instantiate a new Bitmap, if .NET throws an exception
            // we can assume that it's not a valid image
            try
            {
                using (var image = Image.Load(file.OpenReadStream()))
                {
                    // Perform any additional checks if needed
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                file.OpenReadStream().Position = 0;
            }

            return true;
        }
    }
}