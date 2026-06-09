using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Models.ConstantsAndExtensions
{
    internal static class JsonSerializerHelper
    {
        public static bool TryDeserialize<T>(string input, out T? value)
        {
            try
            {
                value = JsonSerializer.Deserialize<T>(input);
                return value is null ? false : true;
            }
            catch (JsonException)
            {
                value = default;
                return false;
            }
        }
    }
}
