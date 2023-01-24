using BotChat.App.Services;
using BotChat.App.Views;
using ChatGPT;
using ChatGPT.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace BotChat.App.ViewModels
{   
    public partial class TextInputViewModel : ObservableObject
    {
        public TextInputType Type { get; set; }

        [ObservableProperty]
        string message;

        [ObservableProperty]
        EventHandler onSendMessage;

        private readonly IChatGPTService _chatGPTService;
        private readonly ISpeechService _speechService;
        private readonly IUserService _userService;
        private readonly ISpeechToText _speechToText;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private MicrophonePopup _popup;

        public TextInputViewModel(IChatGPTService chatGPTService, ISpeechService speechService, IUserService userService, ISpeechToText speechToText)
        {
            _chatGPTService = chatGPTService;
            _speechService = speechService;
            _userService = userService;
            _speechToText = speechToText;
        }

        [RelayCommand]
        async void SendButtonClick()
        {
            if (string.IsNullOrEmpty(Message))
            {
                await Toast.Make("Message cannot be empty.").Show();
                return;
            }

            switch (Type)
            {
                case TextInputType.Text:
                    SendTextTypeRequest();
                    break;
                case TextInputType.Image:
                    SendImageTypeRequest();
                    break;
            }            
        }

        [RelayCommand]
        async void MicButtonClick()
        {
            _popup = new MicrophonePopup();
            _popup.OnStopClick += ListenCancel;
            var isAuthorized = await _speechToText.RequestPermissions();
            if (isAuthorized)
            {
                Shell.Current.ShowPopupAsync(_popup);

                try
                {
                    Message = await _speechToText.Listen(CultureInfo.GetCultureInfo("en-us"),
                        new Progress<string>(partialText =>
                        {
                            if (DeviceInfo.Platform == DevicePlatform.Android)
                            {
                                Message = partialText;
                            }
                            else
                            {
                                Message += partialText + " ";
                            }

                            OnPropertyChanged(nameof(Message));
                        }), tokenSource.Token);
                }
                catch (Exception ex)
                {
                    //await DisplayAlert("Error", ex.Message, "OK");
                    try { _popup.Close(); } catch { }
                }
            }
            else
            {
                //await DisplayAlert("Permission Error", "No microphone access", "OK");
            }

            try { _popup.Close(); } catch { }

            if (_userService.Settings.IsEnabledAutosend && !string.IsNullOrEmpty(Message))
            {
                SendButtonClick();
            }
        }

        void ListenCancel(object s, EventArgs e)
        {
            tokenSource?.Cancel();
            try { _popup.Close(); } catch { }
        }

        private async void SendTextTypeRequest()
        {
            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Add(new() { Type = ChatType.Human, Text = Message, Created = DateTime.Now });

            OnSendMessage.Invoke(null, null);

            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Add(new() { Type = ChatType.Waiting, Text = "•  •  •", Created = DateTime.Now });

            Message = string.Empty;
            OnSendMessage.Invoke(null, null);
            var result = await _chatGPTService.Completions(new(string.Join('\n', _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Where(c => !c.IsWaiting).Select(c => c.Text))), _userService.Settings.ChatGPTAIModel);
            var answer = result.Choices.First().Text.Replace(string.Join('\n', _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Where(c => !c.IsWaiting).Select(c => c.Text)), string.Empty).Trim();

            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Remove(_chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.FirstOrDefault(c => c.IsWaiting));
            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Add(new() { Type = ChatType.AI, Text = answer, Created = DateTime.Now });

            _speechService.Start(answer);

            OnSendMessage.Invoke(null, null);
        }

        private async void SendImageTypeRequest()
        {
            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Add(new() { Type = ChatType.Human, Text = Message, Created = DateTime.Now });

            OnSendMessage.Invoke(null, null);

            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Add(new() { Type = ChatType.Waiting, Text = "•  •  •", Created = DateTime.Now });

            Message = string.Empty;
            OnSendMessage.Invoke(null, null);

            var result = await _chatGPTService.ImageGeneration(new(Message, 2));
            var answer = string.Join('\n', result.Data.Select(d => d.Url)).Trim();

            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Remove(_chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.FirstOrDefault(c => c.IsWaiting));
            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Add(new() { Type = ChatType.AI, Text = answer, Created = DateTime.Now });

            _speechService.Start(answer);

            OnSendMessage.Invoke(null, null);
        }
    }
}
