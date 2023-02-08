namespace BotChat.Shared
{
    public class Settings
    {
        public string Language { get; set; }
        public bool IsEnabledAIVoice { get; set; } = false;
        public bool IsEnabledAutosend { get; set; } = true;
        public string ChatGPTAIModel { get; set; } = "text-davinci-003";
        public double SpeechOptionsPitch { get; set; } = 1.5f;
        public double SpeechOptionsVolume { get; set; } = 0.75f;
    }
}
