﻿namespace CabPostService.Models.Dtos
{
    public class CreatorResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
    }
}
