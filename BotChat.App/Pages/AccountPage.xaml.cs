using BotChat.App.ViewModels;
using BotChat.App.Views;

namespace BotChat.App
{
    public partial class AccountPage : ContentPage
    {
        public AccountPage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Navigation.Add(new NavigationView(serviceProvider.GetService<HeaderViewModel>()));
        }

        private async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}