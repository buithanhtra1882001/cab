namespace CabPostService.Models.Dtos;

public class BaseResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public object Content { get; set; }
}