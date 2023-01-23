using System.Text.Json;

namespace BotChat.App.Services
{
    public interface ILocalSecureStorageService
    {
        Task<T> GetItem<T>(string key);
        Task SetItem<T>(string key, T value);
        bool RemoveItem(string key);
    }

    public class LocalSecureStorageService : ILocalSecureStorageService
    {

        public async Task<T> GetItem<T>(string key)
        {
            try
            {   
                var json = await SecureStorage.Default.GetAsync(key);

                if (json == null)
                    return default;


                return JsonSerializer.Deserialize<T>(json);
            }
            catch(Exception e)
            {

            }
            return default;
        }

        public async Task SetItem<T>(string key, T value)
        {
            try
            {
                await SecureStorage.Default.SetAsync(key, JsonSerializer.Serialize(value));
            }
            catch (Exception e) 
            {
            }
        }

        public bool RemoveItem(string key)
        {
            return SecureStorage.Default.Remove(key);
        }
    }
}
