using Newtonsoft.Json.Linq;
using System;

namespace Talos.Extensions
{
    public static class JObjectExtensions
    {
        /// <summary>
        /// Attempts to get the value of the specified property and convert it to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to convert the property value to.</typeparam>
        /// <param name="jObject">The JObject instance.</param>
        /// <param name="key">The name of the property.</param>
        /// <param name="value">The converted value if successful; otherwise, the default value for the type.</param>
        /// <returns>True if the property exists and was successfully converted; otherwise, false.</returns>
        public static bool TryGetTypedValue<T>(this JObject jObject, string key, out T? value)
        {
            value = default;

            if (jObject == null)
                return false;

            // Use the built-in TryGetValue to get the JToken
            if (!jObject.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out JToken? token))
                return false;

            if (token.Type == JTokenType.Null)
                return false;

            try
            {
                // Convert the JToken to the desired type
                value = token.Value<T>();
                return true;
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                Console.WriteLine($"Error converting property '{key}' to type {typeof(T).Name}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified property exists and equals the expected value.
        /// </summary>
        /// <typeparam name="T">The type of the expected value.</typeparam>
        /// <param name="jObject">The JObject instance.</param>
        /// <param name="key">The name of the property.</param>
        /// <param name="expectedValue">The expected value of the property.</param>
        /// <returns>True if the property exists and equals the expected value; otherwise, false.</returns>
        public static bool HasValue<T>(this JObject jObject, string key, T expectedValue)
        {
            if (jObject == null)
                return false;

            if (!jObject.TryGetTypedValue(key, out T? value))
                return false;

            if (value == null)
                return false;

            return value.Equals(expectedValue);
        }
    }
}
