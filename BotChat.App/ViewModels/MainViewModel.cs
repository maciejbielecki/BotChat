using BotChat.App.Services;
using ChatGPT;
using ChatGPT.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BotChat.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly TextInputViewModel _textInputViewModel;
        private readonly IChatGPTService _chatGPTService;
        private readonly ISpeechService _speechService;

        [ObservableProperty]
        ObservableCollection<ChatAnswer> answers;

        public ListView ChatListView { get; set; }


        public MainViewModel(TextInputViewModel textInputViewModel, IChatGPTService chatGPTService, ISpeechService speechService)
        {
            _speechService = speechService;
            _textInputViewModel = textInputViewModel;
            _textInputViewModel.OnSendMessage += OnSendMessage;

            _chatGPTService = chatGPTService;
            Answers = new(_chatGPTService.Conversations.First(c => c.Type == TextInputType.Text).Answers);
        }

        public async void OnSendMessage(object o, EventArgs e)
        {
            Answers = new(_chatGPTService.Conversations.First(c => c.Type == TextInputType.Text).Answers);
            ChatListView.ScrollTo(Answers.LastOrDefault(), ScrollToPosition.End, true);
        }

        public async void VolumeOff()
        {
            _speechService.Stop();
        }

        public async void VolumeUp(string text)
        {
            _speechService.Start(text, true);
        }

        public async void CopyContent(string text)
        {
            await Clipboard.SetTextAsync(text);
        }
    }
}
