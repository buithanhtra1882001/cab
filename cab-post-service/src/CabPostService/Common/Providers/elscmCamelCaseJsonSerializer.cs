using CabPostService.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabPostService.Common.Providers
{
    public class CamelCaseJsonSerializer : IJsonSerializer
    {
        #region Constructor

        public CamelCaseJsonSerializer()
        {
            Name = JsonSerializerNames.CamelCase;
        }

        #endregion

        #region Properties

        public string Name { get; }

        #endregion

        #region Methods

        public virtual T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public virtual string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        #endregion
    }
}
