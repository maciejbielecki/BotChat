using BotChat.Shared;

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

        private Locale _locale;

        private CancellationTokenSource cts;

        public SpeechService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Locale> GetLocale(string language)
        {
            return (await TextToSpeech.Default.GetLocalesAsync()).FirstOrDefault(l => l.Name == language);
        }
        public async Task<IEnumerable<Locale>> GetLocales()
        {
            return await TextToSpeech.Default.GetLocalesAsync();
        }

        public string RecordingToText()
        {
            throw new NotImplementedException();
        }

        public async void Start(string text, bool isManual = false)
        {
            if (_userService.Settings.IsEnabledAIVoice || isManual)
            {
                SpeechOptions options = new SpeechOptions()
                {
                    Pitch = 1.5f,   // 0.0 - 2.0
                    Volume = 0.75f, // 0.0 - 1.0
                    Locale = !string.IsNullOrEmpty(_userService.Settings?.Language) ? await GetLocale(_userService.Settings?.Language) : null
                };
                cts = new();

                await TextToSpeech.Default.SpeakAsync(text, options, cts.Token);
            }
        }

        public void Stop()
        {
            cts.Cancel();
        }
    }
}
