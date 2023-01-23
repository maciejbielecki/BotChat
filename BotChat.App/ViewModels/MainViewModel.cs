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

        [ObservableProperty]
        ObservableCollection<ChatAnswer> answers;

        public ListView ChatListView { get; set; }


        public MainViewModel(TextInputViewModel textInputViewModel, IChatGPTService chatGPTService)
        {
            _textInputViewModel = textInputViewModel;
            _textInputViewModel.OnSendMessage += OnSendMessage;

            _chatGPTService = chatGPTService;
            Answers = new(_chatGPTService.Conversations.First());
        }

        public async void OnSendMessage(object o, EventArgs e)
        {
            Answers = new(_chatGPTService.Conversations.First());
            ChatListView.ScrollTo(Answers.LastOrDefault(), ScrollToPosition.End, true);
        }
    }
}
