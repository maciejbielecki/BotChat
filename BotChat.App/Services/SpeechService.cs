using BotChat.Shared;

namespace BotChat.App.Services
{
    public interface ISpeechService
    {
        Task<Locale> GetLocale(string language);
        Task<IEnumerable<Locale>> GetLocales();
        void Speak(string text);
        string RecordingToText();
        Task Initialize();
    }
    public class SpeechService : ISpeechService
    {
        private readonly IUserService _userService;

        private Locale _locale;

        public SpeechService(IUserService userService)
        {
            _userService = userService;
            //Initialize();
        }

        public async Task<Locale> GetLocale(string language)
        {
            return (await TextToSpeech.Default.GetLocalesAsync()).FirstOrDefault(l => l.Language == language);
        }
        public async Task<IEnumerable<Locale>> GetLocales()
        {
            return await TextToSpeech.Default.GetLocalesAsync();
        }

        public async Task Initialize()
        {
            _locale = !string.IsNullOrEmpty(_userService.Settings?.Language) ? await GetLocale(_userService.Settings?.Language) : null;
        }

        public string RecordingToText()
        {
            throw new NotImplementedException();
        }

        public async void Speak(string text)
        {
            if (_userService.Settings.IsEnabledAIVoice)
            {
                SpeechOptions options = new SpeechOptions()
                {
                    Pitch = 1.5f,   // 0.0 - 2.0
                    Volume = 0.75f, // 0.0 - 1.0
                    Locale = _locale
                };

                await TextToSpeech.Default.SpeakAsync(text, options);
            }
        }
    }
}
