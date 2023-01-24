using BotChat.App.Services;
using BotChat.App.ViewModels;
using BotChat.App.Views;
using ChatGPT.Models;

namespace BotChat.App
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;

            Navigation.Add(new NavigationView(serviceProvider.GetService<HeaderViewModel>()));
            TextInput.Add(new TextInputView(serviceProvider.GetService<TextInputViewModel>(), TextInputType.Text));
            _viewModel.ChatListView = ChatListView;

            MainThread.InvokeOnMainThreadAsync(async() =>
            {
                await serviceProvider.GetService<IUserService>().Initialize();
            });
        }

        private async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            //await Shell.Current.GoToAsync(nameof(ImagePage));
        }
    }
}