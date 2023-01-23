using BotChat.App.Helpers;
using BotChat.Shared;
using CommunityToolkit.Maui.Alerts;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BotChat.App.Services
{
    public interface IHttpClientService
    {
        Task<T> Get<T>(string uri);
        Task Post(string uri, object value);
        Task<T> Post<T>(string uri, object value);
        //Task<T> PostWithFile<T>(string uri, object value, IBrowserFile file);
        Task Put(string uri, object value);
        Task<T> Put<T>(string uri, object value);
        Task Delete(string uri);
        Task<T> Delete<T>(string uri);
        string GetBaseUrl();
    }

    public class HttpClientService : IHttpClientService
    {
        public HttpClient _httpClient;
        private readonly ILocalSecureStorageService _localSecureStorageService;

        public HttpClientService(HttpClient httpClient, ILocalSecureStorageService localSecureStorageService)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(860);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _localSecureStorageService = localSecureStorageService;
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request);
        }

        public async Task Post(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            await sendRequest(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            return await sendRequest<T>(request);
        }

        public async Task Put(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            await sendRequest(request);
        }

        public async Task<T> Put<T>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            return await sendRequest<T>(request);
        }

        public async Task Delete(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            await sendRequest(request);
        }

        public async Task<T> Delete<T>(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            return await sendRequest<T>(request);
        }

        private HttpRequestMessage createRequest(HttpMethod method, string uri, object value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
            {
                if (value is MultipartFormDataContent multipartFormDataContent)
                {
                    request.Content = multipartFormDataContent;
                }
                else
                {
                    request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
                }
            }

            return request;
        }

        private async Task sendRequest(HttpRequestMessage request)
        {

            await addJwtHeader(request);
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
                return;
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return;
            }

            await handleErrors(response);
        }

        private async Task<T> sendRequest<T>(HttpRequestMessage request)
        {

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Toast.Make("Brak internetu. Aplikacja jest w trybie offline.").Show();
                return default;
            }

            await addJwtHeader(request);
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Shell.Current.GoToAsync(nameof(LoginPage));
                return default;
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return default;
            }

            await handleErrors(response);

            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new StringConverter());
            return await response.Content.ReadFromJsonAsync<T>(options);
        }

        private async Task addJwtHeader(HttpRequestMessage request)
        {
            var user = await _localSecureStorageService.GetItem<UserDTO>("user");
            var isApiUrl = !request.RequestUri.IsAbsoluteUri;
            if (user != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
        }

        private async Task handleErrors(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                await Toast.Make(error["message"]).Show();
            }
        }

        public string GetBaseUrl()
        {
            return _httpClient.BaseAddress.ToString();
        }
    }

    public class ResponseMessage
    {
        public double Version { get; set; }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}
