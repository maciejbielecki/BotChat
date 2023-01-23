using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BotChat.App.ViewModels
{
    public partial class HeaderViewModel : ObservableObject
    {
        private const bool _shouldAnimateNavigation = true;
        public HeaderViewModel()
        {

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
    }
}
