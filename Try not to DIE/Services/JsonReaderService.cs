using Microsoft.Extensions.Options;
using System.Text.Json;
using Try_not_to_DIE.Configuration;
using Try_not_to_DIE.Models.Icd10;
using Try_not_to_DIE.ServicesInterfaces;

namespace Try_not_to_DIE.Services
{
    public class JsonReaderService: IJsonReaderService
    {

        public JsonReaderService()
        {

        }

        public async Task<T> GetJsonDataAsync<T>(string pathToJson)
        {
            var json = await File.ReadAllTextAsync(pathToJson);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
