namespace CabMediaService.Integration.Dropbox
{
    public interface IDropboxService
    {
        string GetUploadVideoPath();
        string GetUploadImagePath();
        Task<string> GetToken();
    }
}
