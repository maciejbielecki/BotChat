using ChatGPT.Models;
using ChatGPT.Models.Requests;
using ChatGPT.Models.Responses;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

//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

//var obj1 = chatGPT.GetGtpModels().Result;
//var obj2 = chatGPT.Completions(new("text-davinci-003", "What is 2 + 2?", 0, 512)).Result;
//var obj3 = chatGPT.Edits(new("text-davinci-edit-001", "What day of th wek is it?", "Fix the spelling mistakes")).Result;
//var obj4 = chatGPT.ImageGeneration(new("A cute baby sea otter", 2)).Result;

//var bytes = File.ReadAllBytes("C:\\Users\\Maciej Bielecki\\Downloads\\icon.png");
//var obj5 = chatGPT.ImageEdits(new("A cute baby sea otter", bytes, bytes, 1)).Result;

namespace ChatGPT
{
    public interface IChatGPTService
    {
        List<GptModel> Models { get; }
        List<Conversation> Conversations { get; set; }
        void SetApiKey(string apiKey);
        Task<GtpModelsResponse> GetGtpModels();
        Task<GptModel> GetGtpModel(string model);
        Task<GptCompletionsResponse> Completions(GptCompletionsRequest obj, string lastRequestId = null);
        Task<GptCompletionsResponse> Edits(GptEditRequest obj);
        Task<GptImageGenerationResponse> ImageGeneration(GptImageGenerationRequest obj);
    }

    public class ChatGPTService : IChatGPTService
    {
        private readonly HttpClient _client;
        private const string _openApiOrganization = "org-Oy5TPHeuYK5Fm1yentxadKft";
        private string _apiKey = "";
        public List<Conversation> Conversations { get; set; } = new();

        public List<GptModel> Models { get; private set; }

        public ChatGPTService()
        {
            _client = HttpClientFactory.Create();
            _client.BaseAddress = new Uri("https://api.openai.com/v1/");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("OpenAI-Organization", _openApiOrganization);
            SetApiKey("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");
            if (!Conversations.Any())
            {
                Conversations.Add(new(TextInputType.Text));
                Conversations.Add(new(TextInputType.Image));
            }
            Task.Run(async () => { Models = (await GetGtpModels()).Data; });
        }

        public async Task<GtpModelsResponse> GetGtpModels()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "models");

            var response = await _client.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<GtpModelsResponse>();
        }

        public async Task<GptModel> GetGtpModel(string model)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"models/{model}");
            request.Headers.Add("OpenAI-Organization", _openApiOrganization);

            var response = await _client.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<GptModel>();
        }

        public async Task<GptCompletionsResponse> Completions(GptCompletionsRequest obj, string lastRequestId = null)
        {
            var json = JsonConvert.SerializeObject(obj);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "completions");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(lastRequestId))
            {
                request.Headers.Add("x-request-id", lastRequestId);
            }

            var response = await _client.SendAsync(request);

            //Console.WriteLine(await response.Content.ReadAsStringAsync());

            return await response.Content.ReadFromJsonAsync<GptCompletionsResponse>();
        }

        public async Task<GptCompletionsResponse> Edits(GptEditRequest obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "edits");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<GptCompletionsResponse>();
        }

        public async Task<GptImageGenerationResponse> ImageGeneration(GptImageGenerationRequest obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "images/generations");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<GptImageGenerationResponse>();
        }

        public async Task<GptImageGenerationResponse> ImageEdits(GptImageEditRequest obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "images/edit");

            request.Content = obj.FormData;

            var response = await _client.SendAsync(request);

            Console.WriteLine(await response.Content.ReadAsStringAsync());

            return await response.Content.ReadFromJsonAsync<GptImageGenerationResponse>();
        }

        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }
    }
}
