using BotChat.App.ViewModels;
using BotChat.App.Views;

namespace BotChat.App
{
    public partial class ImagePage : ContentPage
    {
        public ImagePage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Navigation.Add(new NavigationView(serviceProvider.GetService<HeaderViewModel>()));
        }
    }
}