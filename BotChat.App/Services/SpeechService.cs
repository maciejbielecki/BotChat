using BotChat.Shared;
using static System.Net.Mime.MediaTypeNames;

namespace BotChat.App.Services
{
    public interface ISpeechService
    {
        Task<Locale> GetLocale(string language);
        Task<IEnumerable<Locale>> GetLocales();
        void Start(string text, bool isManual = false);
        void Stop();
        string RecordingToText();
    }
    public class SpeechService : ISpeechService
    {
        private readonly IUserService _userService;
        private CancellationTokenSource cts;

        public SpeechService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Locale> GetLocale(string language)
        {
            try
            {
                return (await TextToSpeech.Default.GetLocalesAsync()).FirstOrDefault(l => l.Name == language);
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<Locale>> GetLocales()
        {
            try
            {
                return await TextToSpeech.Default.GetLocalesAsync();
            }
            catch
            {
                return new List<Locale>();
            }
        }

        public string RecordingToText()
        {
            throw new NotImplementedException();
        }

        public async void Start(string text, bool isManual = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (_userService.Settings.IsEnabledAIVoice || isManual)
            {
                SpeechOptions options = new SpeechOptions()
                {
                    Pitch = 1.5f,   // 0.0 - 2.0
                    Volume = 0.75f, // 0.0 - 1.0
                    Locale = !string.IsNullOrEmpty(_userService.Settings?.Language) ? await GetLocale(_userService.Settings?.Language) : null
                };
                cts = new();

                try
                {
                    await TextToSpeech.Default.SpeakAsync(text, options, cts.Token);
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("TextToSpeech.Default.SpeakAsync error");
                }
            }
        }

        public void Stop()
        {
            try
            {
                cts.Cancel();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("CancellationTokenSource.Cancel() error");
            }
        }
    }
}
