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
    }
}