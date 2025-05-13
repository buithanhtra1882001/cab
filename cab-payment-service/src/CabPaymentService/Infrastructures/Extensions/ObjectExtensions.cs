using System.Globalization;
using System.Reflection;
using CabPaymentService.Model.Dtos;

namespace CabPaymentService.Infrastructures.Extensions
{
    /// <summary>
    /// Mapping object to dictionary and vice versa
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// mapping IDictionary<string, string> source to object
        /// value for nested object must be in query string (e.g. firstprop=firstvalue&secondprop=secondvalue&thirdprop=thirdvalue)
        /// 
        /// DateTime value format must be either parsable by DateTime.TryParse() or in yyyyMMddHHmmss format
        ///
        /// not support object with array/list type in property
        /// </summary>
        /// <typeparam name="TOut">Type of Object to receive</typeparam>
        /// <param name="source">key is property name, value is property value to parse into destination object</param>
        /// <returns></returns>
        public static TOut ToObject<TOut>(this IDictionary<string, string> source)
        where TOut : class, new()
        {
            var resultObject = Activator.CreateInstance(typeof(TOut)) as TOut;
            var resultObjType = resultObject.GetType();

            try
            {
                foreach (var item in source)
                {
                    var property = resultObjType.GetProperty(item.Key.ToString());
                    var propType = property.PropertyType;
                    if (propType == typeof(string)) // Base case: natively set string value to string type
                    {
                        property.SetValue(resultObject, item.Value);

                        continue;
                    }

                    if (propType == typeof(DateTime)) // Base case: natively set string value to string type
                    {
                        var canParse = DateTime.TryParse(item.Value, out DateTime value); // auto detect datetime format

                        if (!canParse) // if cannot auto detect format. Fallback to explicit datetime format. If fail, will throw exception
                        {
                            value = DateTime.ParseExact(item.Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                        }

                        property.SetValue(resultObject, value);

                        continue;
                    }

                    /*
                     if has Parse
                        yes: parse (base case)
                        no: recursive into each its prop and call ToObject
                     */
                    var parseMethod = propType.GetMethod("Parse", new [] {typeof(string)});



                    if (parseMethod != null) // base case: able to Parse
                    {
                        // Exception occur when require custom Parse. e.g. DateTime type may need different format params

                        // pseudo code: var value = SomeType.Parse( item.Value )
                        var value = parseMethod.Invoke(null, new object[] { item.Value });
                        property.SetValue(resultObject, value);

                        continue;
                    }

                    if (parseMethod == null) // recursive (drill) case: recursively parsing the current value to current property type
                    {
                        // Exception occur when no Parse found. And could not drill into the object

                        // parsing the string value into IDictionary<string, string>
                        IDictionary<string, string> recursiveSource = new SortedList<string, string>();
                        item.Value.ParseFromQueryStringToSortedList(recursiveSource);

                        // use reflection to obtain ToObject<>(...) because cannot directly invoke ToObject<propType>(...) due to generic cannot be a variable
                        var toObjectMethod = typeof(ObjectExtensions).GetMethod("ToObject");
                        var toObjectMethodWithPropTypeGeneric = toObjectMethod.MakeGenericMethod(new[] { propType });
                        var value = toObjectMethodWithPropTypeGeneric.Invoke(null, new object[] { recursiveSource }); // pseudo code: ToObject<propType>( recursiveSource )
                        property.SetValue(resultObject, value);

                        continue;
                    }

                }

                return resultObject;

            }
            catch (Exception e)
            {
                e.Data.Add("Custom Message", $"Failed to when trying to parse type {source.GetType().FullName} to type {typeof(TOut).FullName}");

                throw;
            }

        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}
