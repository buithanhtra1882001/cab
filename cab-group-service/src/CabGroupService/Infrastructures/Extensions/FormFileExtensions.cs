namespace CabGroupService.Infrastructures.Extensions;

/// <summary>
/// Extension methods for IFormFile class.
/// </summary>
public static class FormFileExtensions
{
    /// <summary>
    /// Convert IFormFile to bytes array.
    /// </summary>
    public static async Task<byte[]> GetBytesAsync(this IFormFile formFile)
    {
        using (var ms = new MemoryStream())
        {
            await formFile.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}