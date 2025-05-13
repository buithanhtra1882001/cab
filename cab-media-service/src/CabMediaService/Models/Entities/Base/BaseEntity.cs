using Newtonsoft.Json;

namespace CabMediaService.Models.Entities.Base
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string ToJson()
        {
            if (this is null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            });
        }

    }
}