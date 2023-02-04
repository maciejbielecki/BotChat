using ChatGPT.Models;
using ChatGPT;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotChat.App.ViewModels
{
    public partial class ImageViewModel : ObservableObject
    {
        private readonly TextInputViewModel _textInputViewModel;
        private readonly IChatGPTService _chatGPTService;

        [ObservableProperty]
        ObservableCollection<ChatAnswer> answers;

        public ListView ChatListView { get; set; }
        public ImageViewModel(TextInputViewModel textInputViewModel, IChatGPTService chatGPTService)
        {
            _textInputViewModel = textInputViewModel;
            _textInputViewModel.OnSendImage += OnSendImage;

            _chatGPTService = chatGPTService;
            Answers = new(_chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers);
        }

        public async void OnSendImage(object o, EventArgs e)
        {
            Answers = new(_chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers);
            ChatListView.ScrollTo(Answers.LastOrDefault(), ScrollToPosition.End, true);
        }

        public async void CopyContent(string url)
        {
            //await Clipboard.SetTextAsync(text);
        }
    }
}
