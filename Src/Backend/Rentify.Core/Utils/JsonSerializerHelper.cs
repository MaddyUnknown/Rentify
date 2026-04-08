using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rentify.Core.Utils
{
    public static class JsonSerializerHelper
    {
        public static string Serialize<T>(T data, JsonSerializerOptions? options = null)
        {
            return options == null ? JsonSerializer.Serialize(data) : JsonSerializer.Serialize(data, options);
        }

        public static T? Deserialize<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
