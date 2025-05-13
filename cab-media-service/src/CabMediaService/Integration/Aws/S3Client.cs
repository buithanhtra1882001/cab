using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CabMediaService.Integration.Configs;

namespace CabMediaService.Integration.Aws
{
    public class S3Client : IS3Client
    {
        private readonly S3Config _s3Config;

        public S3Client(
            AwsConfig awsConfig)
        {
            _s3Config = awsConfig.S3;
        }

        public string GeneratePreSignedUploadURL(string fileName)
        {
            return GeneratePreSignedUploadURL(fileName, null);
        }

        public string GeneratePreSignedUploadURL(string fileName, string prefix)
        {
            string objectKey = Guid.NewGuid().ToString();

            if (Path.HasExtension(fileName))
            {
                objectKey = $"{objectKey}{Path.GetExtension(fileName)}";
            }

            if (!string.IsNullOrEmpty(prefix))
            {
                objectKey = $"{prefix}/{objectKey}";
            }

            using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey, RegionEndpoint.GetBySystemName(_s3Config.Region)))
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _s3Config.Bucket,
                    Key = objectKey,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.Now.AddYears(10),
                };

                request.Parameters.Add("x-amz-acl", S3CannedACL.PublicRead);

                return client.GetPreSignedURL(request);
            }
        }

        public string GeneratePreSignedGetURL(string objectKey)
        {
            using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey, RegionEndpoint.GetBySystemName(_s3Config.Region)))
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _s3Config.Bucket,
                    Key = objectKey,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.Now.AddYears(10)
                };

                return client.GetPreSignedURL(request);
            }
        }

        public string GeneratePreSignedGetURL(string bucket, string objectKey)
        {
            using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey, RegionEndpoint.GetBySystemName(_s3Config.Region)))
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucket,
                    Key = objectKey,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.Now.AddYears(10)
                };

                return client.GetPreSignedURL(request);
            }
        }

        public async Task<PutObjectResponse> PutObjectToS3Async(Stream stream, string bucketName, string objectKey, string fileContent)
        {
            try
            {
                using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey, RegionEndpoint.GetBySystemName(_s3Config.Region)))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = _s3Config.Bucket,
                        Key = objectKey,
                        CannedACL = S3CannedACL.PublicRead,
                        InputStream = stream,
                        ContentType = fileContent
                    };

                    return await client.PutObjectAsync(request);
                }
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception(String.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message));
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message));
            }
        }

        public async Task<PutObjectResponse> PutObjectToS3Async(string filePath, string bucketName, string objectKey, string contentType)
        {
            try
            {
                using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey, RegionEndpoint.GetBySystemName(_s3Config.Region)))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = _s3Config.Bucket,
                        Key = objectKey,
                        CannedACL = S3CannedACL.PublicRead,
                        FilePath = filePath,
                        ContentType = contentType
                    };

                    return await client.PutObjectAsync(request);
                }
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception(String.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message));
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message));
            }
        }

        public async Task<PutObjectResponse> PutBase64ContentToS3Async(string base64String, string bucketName, string objectKey, string fileContent)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);
                using (var stream = new MemoryStream(bytes))
                {
                    var s3Response = await PutObjectToS3Async(stream, bucketName, objectKey, fileContent);
                    return s3Response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload file to S3 due to exception: {ex.Message}");
            }
        }

        public async Task<GetObjectResponse> GetObjectAsync(string bucket, string objectKey)
        {
            try
            {
                using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey, RegionEndpoint.GetBySystemName(_s3Config.Region)))
                {
                    var s3Response = await client.GetObjectAsync(bucket, objectKey);
                    return s3Response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get object due to exception: {ex.Message}");
            }
        }

        public async Task<ListObjectsResponse> GetListObjectAsync(string bucket, string objectKey)
        {
            try
            {
                using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey,
                    RegionEndpoint.GetBySystemName(_s3Config.Region)))
                {
                    var s3Response = await client.ListObjectsAsync(bucket, objectKey);
                    return s3Response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get object due to exception: {ex.Message}");
            }
        }

        public async Task<long> GetObjectSizeAsync(string bucket, string objectKey)
        {
            using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey,
                   RegionEndpoint.GetBySystemName(_s3Config.Region)))
            {
                var s3Response = await client.ListObjectsAsync(bucket, objectKey);
                return s3Response.S3Objects.Sum(x => x.Size);
            }
        }

        public async Task DeleteObjectAsync(string bucket, string objectKey)
        {
            using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey,
                   RegionEndpoint.GetBySystemName(_s3Config.Region)))
            {
                var deleteFolderRequest = new DeleteObjectRequest
                {
                    BucketName = bucket,
                    Key = objectKey
                };
                await client.DeleteObjectAsync(deleteFolderRequest);
            }
        }

        public async Task DeleteObjectsAsync(string bucket, string objectKey)
        {
            // Get all object in folder
            var listObjectInFolder = await GetListObjectAsync(bucket, objectKey);

            if (listObjectInFolder == null || !listObjectInFolder.S3Objects.Any())
                return;

            // Create list objects to delete
            var listDeleteObjects = new DeleteObjectsRequest()
            {
                BucketName = bucket
            };

            listObjectInFolder.S3Objects.ForEach(obj =>
                listDeleteObjects.AddKey(obj.Key)
            );

            using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey,
                RegionEndpoint.GetBySystemName(_s3Config.Region)))
            {
                await client.DeleteObjectsAsync(listDeleteObjects);
            }
        }

        public async Task DeleteObjectsAsync(string bucket, IEnumerable<string> listObjectKeys)
        {
            var listObjectInFolder = new List<ListObjectsResponse>();
            // Get all object in folder
            foreach (var objectKey in listObjectKeys)
            {
                if (objectKey == null || string.IsNullOrEmpty(objectKey))
                {
                    continue;
                }

                var listObjects = await GetListObjectAsync(bucket, objectKey);
                listObjectInFolder.Add(listObjects);
            }

            if (listObjectInFolder == null || !listObjectInFolder.Any())
                return;

            // Create list objects to delete
            var listDeleteObjects = new DeleteObjectsRequest()
            {
                BucketName = bucket
            };

            // Add object to delete
            foreach (var listObject in listObjectInFolder)
            {
                listObject.S3Objects.ForEach(obj =>
                    listDeleteObjects.AddKey(obj.Key)
                );
            }

            using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey,
                RegionEndpoint.GetBySystemName(_s3Config.Region)))
            {
                if (listDeleteObjects.Objects.Count > 0)
                {
                    await client.DeleteObjectsAsync(listDeleteObjects);
                }
            }
        }

        public string GetAbsoluteLink(string objectKey)
        {
            return $"https://{_s3Config.Bucket}.s3.{_s3Config.Region}.amazonaws.com/{objectKey}";
        }

        public async Task DeleteObjectsAsync(params string[] fileLinks)
        {
            var objectKeyList = fileLinks.Select(GetObjectKeyFromLink).ToList();
            await DeleteObjectsAsync(_s3Config.Bucket, objectKeyList);
        }

        public string GetObjectKeyFromLink(string fileUrl)
        {
            try
            {
                if (fileUrl == null || string.IsNullOrEmpty(fileUrl) || string.IsNullOrWhiteSpace(fileUrl))
                {
                    return null;
                }

                if (fileUrl.Contains("AWSAccessKeyId"))
                {
                    var startObjectKeyIndex = fileUrl.IndexOf("/f");
                    var questionIndex = fileUrl.LastIndexOf('?');
                    return fileUrl.Substring(startObjectKeyIndex + 1, questionIndex - startObjectKeyIndex - 1);
                }
                else
                {
                    var startObjectKeyIndex = fileUrl.IndexOf("/f");
                    return fileUrl.Substring(startObjectKeyIndex + 1, fileUrl.Length - startObjectKeyIndex - 1);
                }
            }
            catch
            {
                return null;
            }
        }

        public string GetAbsoluteLink(string bucket, string objectKey)
        {
            return $"https://{bucket}.s3.{_s3Config.Region}.amazonaws.com/{objectKey}";
        }

        public async Task<PutObjectResponse> PutObjectToS3Async(string filePath, string objectKey, string contentType)
        {
            try
            {
                using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey, RegionEndpoint.GetBySystemName(_s3Config.Region)))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = _s3Config.Bucket,
                        Key = objectKey,
                        CannedACL = S3CannedACL.PublicRead,
                        FilePath = filePath,
                        ContentType = contentType
                    };

                    return await client.PutObjectAsync(request);
                }
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception(String.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message));
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message));
            }
        }

        public async Task<string> UploadFileToS3Async(string filePath, string objectKey)
        {
            try
            {
                string bucketName = _s3Config.Bucket;
                string accessKeyId = _s3Config.AccessKey;
                string secretAccessKey = _s3Config.SecretKey;

                using (var client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(_s3Config.Region)))
                {
                    PutObjectRequest request = new()
                    {
                        BucketName = bucketName,
                        Key = objectKey,
                        FilePath = filePath,
                    };

                    await client.PutObjectAsync(request);
                    return GeneratePreSignedGetURL(objectKey);
                }
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception(string.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message));
            }
        }

        public async Task<List<string>> GetFilesFromFolderAsync(string folderPath)
        {
            var result = new List<string>();
            string bucketName = _s3Config.Bucket;

            using (var client = new AmazonS3Client(_s3Config.AccessKey, _s3Config.SecretKey,
                    RegionEndpoint.GetBySystemName(_s3Config.Region)))
            {
                ListObjectsRequest request = new()
                {
                    BucketName = bucketName,
                    Prefix = folderPath
                };

                ListObjectsResponse response = await client.ListObjectsAsync(request);

                foreach (S3Object obj in response.S3Objects)
                {
                    result.Add(GeneratePreSignedGetURL(obj.Key));
                }
            }

            return result;
        }
    }
}
