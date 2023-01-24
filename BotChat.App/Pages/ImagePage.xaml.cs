using BotChat.App.ViewModels;
using BotChat.App.Views;
using ChatGPT.Models;

namespace BotChat.App
{
    public partial class ImagePage : ContentPage
    {
        public ImagePage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Navigation.Add(new NavigationView(serviceProvider.GetService<HeaderViewModel>()));
            TextInput.Add(new TextInputView(serviceProvider.GetService<TextInputViewModel>(), TextInputType.Image));
        }

        private async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
        }
    }
}