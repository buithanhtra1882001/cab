using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabPostService.Common.Providers
{
    public interface IJsonSerializer
    {
        #region Properties

        string Name { get; }

        #endregion

        #region Methods

        T Deserialize<T>(string json);

        string Serialize<T>(T obj);

        #endregion
    }
}
