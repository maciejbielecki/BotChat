namespace BotChat.Shared
{
    public class Settings
    {
        public string Language { get; set; }
        public bool IsEnabledAIVoice { get; set; }
        public string ChatGPTAIModel { get; set; }
        public double SpeechOptionsPitch { get; set; } = 1.5f;
        public double SpeechOptionsVolume { get; set; } = 0.75f;
    }
}
