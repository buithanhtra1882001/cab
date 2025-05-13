namespace CabUserService.Models.Dtos
{
    public class UserFindByUserNameResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}
