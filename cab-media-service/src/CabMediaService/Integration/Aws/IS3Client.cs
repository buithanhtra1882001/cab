using Amazon.S3.Model;

namespace CabMediaService.Integration.Aws
{
    public interface IS3Client
    {
        string GeneratePreSignedUploadURL(string fileName);
        string GeneratePreSignedUploadURL(string fileName, string prefix);
        string GeneratePreSignedGetURL(string objectKey);
        string GeneratePreSignedGetURL(string bucket, string objectKey);
        string GetAbsoluteLink(string objectKey);
        string GetAbsoluteLink(string bucket, string objectKey);
        Task<PutObjectResponse> PutObjectToS3Async(Stream stream, string bucketName, string objectKey, string fileContent);
        Task<PutObjectResponse> PutObjectToS3Async(string filePath, string bucketName, string objectKey, string contentType);
        Task<PutObjectResponse> PutObjectToS3Async(string filePath, string objectKey, string contentType);
        Task<PutObjectResponse> PutBase64ContentToS3Async(string base64String, string bucketName, string objectKey, string fileContent);
        Task<string> UploadFileToS3Async(string filePath, string objectKey);
        Task<GetObjectResponse> GetObjectAsync(string bucket, string objectKey);
        Task<ListObjectsResponse> GetListObjectAsync(string bucket, string objectKey);
        Task<long> GetObjectSizeAsync(string bucket, string objectKey);
        Task<List<string>> GetFilesFromFolderAsync(string folderPath);
        Task DeleteObjectAsync(string bucket, string objectKey);
        Task DeleteObjectsAsync(string bucket, string objectKey);
        Task DeleteObjectsAsync(string bucket, IEnumerable<string> listObjectKeys);
        Task DeleteObjectsAsync(params string[] fileLinks);
        string GetObjectKeyFromLink(string fileUrl);
    }
}
