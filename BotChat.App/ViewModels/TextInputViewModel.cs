using BotChat.App.Services;
using BotChat.App.Views;
using ChatGPT;
using ChatGPT.Models;
using ChatGPT.Models.Responses;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.MauiMTAdmob;
using System.Globalization;

namespace BotChat.App.ViewModels
{
    public enum ImageRequestType
    {
        Generation,
        Edits,
        Variations
    }

    public partial class TextInputViewModel : ObservableObject
    {
        public TextInputType Type { get; set; }

        public ImageRequestType? RequestType { get; set; }

        [ObservableProperty]
        bool isVisibleMainImage;

        [ObservableProperty]
        bool isVisibleMaskImage;

        [ObservableProperty]
        bool isVisibleNumberCount;

        [ObservableProperty]
        bool isVisibleMicrophone;

        [ObservableProperty]
        int entryWidth = 40;

        [ObservableProperty]
        int viewHeight;

        [ObservableProperty]
        string mainImageName;

        [ObservableProperty]
        string maskImageName;

        [ObservableProperty]
        bool isEntryEnabled;

        [ObservableProperty]
        bool isSendButtonEnabled;

        [ObservableProperty]
        string message;

        [ObservableProperty]
        int imageNumber;

        [ObservableProperty]
        EventHandler onSendMessage;

        [ObservableProperty]
        EventHandler onSendImage;

        private readonly IChatGPTService _chatGPTService;
        private readonly ISpeechService _speechService;
        private readonly IUserService _userService;
        private readonly ISpeechToText _speechToText;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private MicrophonePopup _popup;
        private byte[] _mainImage;
        private byte[] _maskImage;

        public TextInputViewModel(IChatGPTService chatGPTService, ISpeechService speechService, IUserService userService, ISpeechToText speechToText)
        {
            _chatGPTService = chatGPTService;
            _speechService = speechService;
            _userService = userService;
            _speechToText = speechToText;
            IsEntryEnabled = true;
            IsSendButtonEnabled = true;
            IsVisibleMicrophone = true;
            IsVisibleMainImage = true;
            IsVisibleMaskImage = true;
            ViewHeight = 80;
            MainImageName = "Main image . . .";
            MaskImageName = "Mask image . . .";
            ImageNumber = 1;
            //CrossMauiMTAdmob.Current.LoadRewarded("ca-app-pub-1642689140493347/1966780640");
            CrossMauiMTAdmob.Current.LoadInterstitial("ca-app-pub-1642689140493347/9988471399");
            CrossMauiMTAdmob.Current.OnInterstitialClosed += (s, e) => { CrossMauiMTAdmob.Current.LoadInterstitial("ca-app-pub-1642689140493347/9988471399"); };


        }

        [RelayCommand]
        async void SendButtonClick()
        {
            if (string.IsNullOrEmpty(Message))
            {
                await Toast.Make("Message cannot be empty.").Show();
                return;
            }

            IsEntryEnabled = false;
            IsSendButtonEnabled = false;

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

        [RelayCommand]
        async void MainImageClick()
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo != null)
            {
                if (!photo.FileName.ToLower().Contains(".png"))
                {
                    await Toast.Make("Image have to be in PNG format.").Show();
                    return;
                }

                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);
                _mainImage = ReadFully(sourceStream);
                MainImageName = photo.FileName;
            }
            else
            {
                MainImageName = "Main image . . .";
                _mainImage = null;
            }
        }

        [RelayCommand]
        async void MaskImageClick()
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo != null)
            {
                if (!photo.FileName.ToLower().Contains(".png"))
                {
                    await Toast.Make("Image have to be in PNG format.").Show();
                    return;
                }

                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);
                _maskImage = ReadFully(sourceStream);
                MaskImageName = photo.FileName;
            }
            else
            {
                _maskImage = null;
                MaskImageName = "Main image . . .";
            }
        }

        partial void OnImageNumberChanged(int value)
        {
            if (value > 5) ImageNumber = 5;
            if (value < 1) ImageNumber = 1;
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

            ChatAnswer waitingAnswer = new() { Type = ChatType.Waiting, Text = "•  •  •", Created = DateTime.Now };

            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Add(waitingAnswer);

            Message = string.Empty;
            OnSendMessage.Invoke(null, null);

            var count = _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Where(a => a.IsAI == true || a.IsWaiting).Count();
            if (count % 3 == 0)
            {
                try
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CrossMauiMTAdmob.Current.ShowInterstitial();
                        //CrossMauiMTAdmob.Current.ShowRewarded();
                    });
                }
                catch (Exception ex)
                {

                }
            }

            var result = await _chatGPTService.Completions(new(string.Join('\n', _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Where(c => !c.IsWaiting).Select(c => c.Text))), _userService.Settings.ChatGPTAIModel);
            var answer = result.Choices == null
                ? "An error occurred while processing your request. Please try again."
                : result.Choices?.FirstOrDefault().Text.Replace(string.Join('\n', _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers.Where(c => !c.IsWaiting).Select(c => c.Text)), string.Empty).Trim();

            waitingAnswer.Type = ChatType.AI;
            waitingAnswer.Text = answer;

            _speechService.Start(answer);

            OnSendMessage.Invoke(null, null);

            IsEntryEnabled = true;
            IsSendButtonEnabled = true;
        }

        private async void SendImageTypeRequest()
        {
            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Add(new() { Type = ChatType.Human, Text = Message, Created = DateTime.Now });

            OnSendImage.Invoke(null, null);

            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Add(new() { Type = ChatType.Waiting, Text = "•  •  •", Created = DateTime.Now });

            OnSendImage.Invoke(null, null);

            var count = _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Where(a => a.IsAI == true || a.IsWaiting).Count();
            if (count % 3 == 0)
            {
                try
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CrossMauiMTAdmob.Current.ShowInterstitial();
                        //CrossMauiMTAdmob.Current.ShowRewarded();
                    });
                }
                catch (Exception ex)
                {

                }
            }

            GptImageGenerationResponse result = null;

            switch (RequestType)
            {
                case ImageRequestType.Generation:
                    result = await _chatGPTService.ImageGeneration(new(Message, ImageNumber));
                    break;
                case ImageRequestType.Edits:
                    result = await _chatGPTService.ImageEdits(new(Message, _mainImage, _maskImage, ImageNumber));
                    break;
                case ImageRequestType.Variations:
                    result = await _chatGPTService.ImageVariations(new(_mainImage, ImageNumber));
                    break;
            }

            _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Remove(_chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.FirstOrDefault(a => a.IsWaiting == true));

            if (result.Data.Select(d => d.Base64).Any())
            {
                foreach (var img in result.Data.Select(d => d.Base64).ToList())
                {
                    _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers.Add(new() { Created = DateTime.Now, Text = img, Type = ChatType.AI });
                }
            }

            OnSendImage.Invoke(null, null);

            Message = string.Empty;
            MainImageName = string.Empty;
            MaskImageName = string.Empty;
            _mainImage = null;
            _maskImage = null;
            SetImageRequestType(ImageRequestType.Generation);
        }

        public void SetTextType()
        {
            Type = TextInputType.Text;
            IsEntryEnabled = true;
            ViewHeight = 80;
        }

        public void SetImageRequestType(ImageRequestType type)
        {
            Type = TextInputType.Image;
            switch (RequestType = type)
            {
                case ImageRequestType.Generation:
                    Message = string.Empty;
                    IsVisibleMainImage = false;
                    IsVisibleMaskImage = false;
                    IsVisibleNumberCount = true;
                    IsEntryEnabled = true;
                    IsSendButtonEnabled = true;
                    IsVisibleMicrophone = true;
                    EntryWidth = 40;
                    ViewHeight = 140;
                    break;
                case ImageRequestType.Edits:
                    Message = string.Empty;
                    IsVisibleMainImage = true;
                    IsVisibleMaskImage = true;
                    IsVisibleNumberCount = true;
                    IsEntryEnabled = true;
                    IsSendButtonEnabled = true;
                    IsVisibleMicrophone = true;
                    EntryWidth = 40;
                    ViewHeight = 250;
                    break;
                case ImageRequestType.Variations:
                    Message = "Click to generate image variations";
                    IsVisibleMainImage = true;
                    IsVisibleMaskImage = false;
                    IsVisibleNumberCount = true;
                    IsEntryEnabled = false;
                    IsSendButtonEnabled = true;
                    IsVisibleMicrophone = false;
                    EntryWidth = 80;
                    ViewHeight = 200;
                    break;
            }
        }

        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[input.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                input.Position = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    ms.Write(buffer, 0, input.Read(buffer, 0, buffer.Length));
                }
                return ms.ToArray();
            }
        }
    }
}
