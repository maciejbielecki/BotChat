using ChatGPT;
using ChatGPT.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AppCenter.Crashes;

namespace BotChat.App.ViewModels
{
    public partial class HeaderViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isAccountButtonVisible;

        [ObservableProperty]
        bool isClearButtonVisible;

        private const bool _shouldAnimateNavigation = true;
        private readonly IChatGPTService _chatGPTService;

        public HeaderViewModel(IChatGPTService chatGPTService)
        {
            _chatGPTService = chatGPTService;
            IsAccountButtonVisible = false;
            IsClearButtonVisible = true;
        }

        [RelayCommand]
        async void AccountButtonClick()
        {
            if (Shell.Current.CurrentPage is not AccountPage)
            {
                await Shell.Current.GoToAsync($"{nameof(AccountPage)}", _shouldAnimateNavigation);
            }
        }

        [RelayCommand]
        async void SettingsButtonClick()
        {
            if (Shell.Current.CurrentPage is not SettingsPage)
            {
                await Shell.Current.GoToAsync($"{nameof(SettingsPage)}", _shouldAnimateNavigation);
            }
        }

        [RelayCommand]
        async void ChatButtonClick()
        {
            if (Shell.Current.CurrentPage is not MainPage)
            {
                await Shell.Current.GoToAsync($"///{nameof(MainPage)}", _shouldAnimateNavigation);
            }
        }

        [RelayCommand]
        async void ImagesButtonClick()
        {
            if (Shell.Current.CurrentPage is not ImagePage)
            {
                await Shell.Current.GoToAsync($"{nameof(ImagePage)}", _shouldAnimateNavigation);
            }
        }

        [RelayCommand]
        async void ClearMessages()
        {
            var page = Shell.Current.CurrentPage;
            if (await page.DisplayAlert(string.Empty, "Would you like to clear messages?", "Yes", "No"))
            {
                try
                {
                    if (page is MainPage)
                    {
                        _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Text).Answers = new();
                        ((page as MainPage).BindingContext as MainViewModel).OnSendMessage(null, null);
                    }
                    else
                    {
                        _chatGPTService.Conversations.FirstOrDefault(c => c.Type == TextInputType.Image).Answers = new();
                        ((page as ImagePage).BindingContext as ImageViewModel).OnSendImage(null, null);
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
        }
    }
}
