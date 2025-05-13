namespace CabGroupService.Models.Dtos
{
    public class ResultResponse
    {
        public int HttpCode { get; set; } = 200;
        public string Message { get; set; } = "Success";
        public object Data { get; set; }

        public void HandleException(Exception e, int code = 500)
        {
            Message = e.Message;
            HttpCode = code;
        }
    }
}
