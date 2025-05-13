using CabUserService.Infrastructures.Constants;
using CabUserService.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Infrastructures.Attributes
{
    public class SexValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var user = (UserCreateUpdateRequest)validationContext.ObjectInstance;

            if (string.IsNullOrEmpty(user.Sex))
            {
                return new ValidationResult("Sex is required.");
            }

            var sexes = new List<string>
            {
                SexConstant.Male.ToLower(),
                SexConstant.Female.ToLower(),
                SexConstant.Other.ToLower()
            };
            if (sexes.Contains(user.Sex.ToLower()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Sex should be Male, Female or Other");
            }
        }
    }
}
