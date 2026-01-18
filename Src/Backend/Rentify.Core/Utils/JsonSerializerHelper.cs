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
        public static string Serialize<T>(T data)
        {
            return JsonSerializer.Serialize<T>(data);
        }

        public static T? Deserialize<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
