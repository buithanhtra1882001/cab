namespace CabMediaService.Models.Dtos
{
    public class UploadFileResponse
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string Url { get; set; }

        public long Size { get; set; }
    }
}
