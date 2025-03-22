namespace Try_not_to_DIE.ServicesInterfaces;

public interface IJsonReaderService
{
    public Task<T> GetJsonDataAsync<T>(string pathToJson);
}
