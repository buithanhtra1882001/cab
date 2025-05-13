using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace WCABNetwork.Cab.IdentityService.Models.Dtos.Requests
{
    public class ChangePasswordRequest
    {

        //[JsonIgnore]
        public Guid UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CurrentPassword { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string NewPassword { get; set; }
    }
}
