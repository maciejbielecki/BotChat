using BotChat.Shared;

namespace BotChat.App.Services
{
    public interface IUserService
    {
        Settings Settings { get; set; }
        Task Initialize();
        void SetIsEnabledAIVoice(bool value);
        void SetIsEnabledAutosend(bool value);
        void SetChatGPTAIModel(string value);
        void SetLanguage(string value);
        void SetSpeechOptionsPitch(double value);
        void SetSpeechOptionsVolume(double value);
    }

    public class UserService : IUserService
    {
        private readonly ILocalSecureStorageService _localSecureStorageService;

        public UserService(ILocalSecureStorageService localSecureStorageService)
        {
            _localSecureStorageService = localSecureStorageService;
            //Initialize();
        }

        private Settings _settings;

        public Settings Settings
        {
            get
            {

                return _settings;
            }
            set
            {
                _localSecureStorageService.SetItem<Settings>("settings", value);
                _settings = value;
            }
        }

        public async Task Initialize()
        {
            Settings = (await _localSecureStorageService.GetItem<Settings>("settings")) ?? new();
        }

        public void SetIsEnabledAIVoice(bool value)
        {
            var settings = Settings;
            settings.IsEnabledAIVoice = value;
            Settings = settings;
        }

        public void SetChatGPTAIModel(string value)
        {
            var settings = Settings;
            settings.ChatGPTAIModel = value;
            Settings = settings;
        }

        public void SetSpeechOptionsPitch(double value)
        {
            var settings = Settings;
            settings.SpeechOptionsPitch = value;
            Settings = settings;
        }

        public void SetSpeechOptionsVolume(double value)
        {
            var settings = Settings;
            settings.SpeechOptionsVolume = value;
            Settings = settings;
        }

        public void SetLanguage(string value)
        {
            var settings = Settings;
            settings.Language = value;
            Settings = settings;
        }

        public void SetIsEnabledAutosend(bool value)
        {
            var settings = Settings;
            settings.IsEnabledAutosend = value;
            Settings = settings;
        }
    }
}
