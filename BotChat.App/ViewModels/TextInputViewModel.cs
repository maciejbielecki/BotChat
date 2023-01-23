using BotChat.App.Services;
using ChatGPT;
using ChatGPT.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BotChat.App.ViewModels
{
    public partial class TextInputViewModel : ObservableObject
    {
        [ObservableProperty]
        string message;

        [ObservableProperty]
        EventHandler onSendMessage;

        private readonly IChatGPTService _chatGPTService;
        private readonly ISpeechService _speechService;


        public TextInputViewModel(IChatGPTService chatGPTService, ISpeechService speechService)
        {
            _chatGPTService = chatGPTService;
            _speechService = speechService;
        }

        [RelayCommand]
        async void MicButtonClick()
        {
            //send
        }

        [RelayCommand]
        async void SendButtonClick()
        {
            _chatGPTService.Conversations.FirstOrDefault().Add(new() { Type = ChatType.Human, Text = Message, Created = DateTime.Now });
            Message = string.Empty;
            OnSendMessage.Invoke(null, null);
            var result = await _chatGPTService.Completions(new(string.Join('\n', _chatGPTService.Conversations.FirstOrDefault().Select(c => c.Text))));
            var answer = result.Choices.First().Text.Replace(string.Join('\n', _chatGPTService.Conversations.FirstOrDefault().Select(c => c.Text)), string.Empty).Trim();
            _chatGPTService.Conversations.FirstOrDefault().Add(new() { Type = ChatType.AI, Text = answer, Created = DateTime.Now });

            _speechService.Speak(answer);

            OnSendMessage.Invoke(null, null);
        }
    }
}
