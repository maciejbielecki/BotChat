using BotChat.App.ViewModels;
using BotChat.App.Views;
using ChatGPT.Models;

namespace BotChat.App
{
    public partial class ImagePage : ContentPage
    {
        private readonly ImageViewModel _viewModel;
        public ImagePage(ImageViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;

            Navigation.Add(new NavigationView(serviceProvider.GetService<HeaderViewModel>()));
            TextInput.Add(new TextInputView(serviceProvider.GetService<TextInputViewModel>(), TextInputType.Image, ImageRequestType.Generation));

            _viewModel.ChatListView = ChatListView;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((TextInput.Children[0] as TextInputView).BindingContext as TextInputViewModel).SetImageRequestType(ImageRequestType.Generation);
        }

        private async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
        }

        async void GenerationButtonClick(object s, EventArgs e)
        {
            var bindingContext = ((TextInput.Children[0] as TextInputView).BindingContext as TextInputViewModel);
            bindingContext.SetImageRequestType(ImageRequestType.Generation);
        }

        async void EditsButtonClick(object s, EventArgs e)
        {
            var bindingContext = ((TextInput.Children[0] as TextInputView).BindingContext as TextInputViewModel);
            bindingContext.SetImageRequestType(ImageRequestType.Edits);
        }


        async void VariationsButtonClick(object s, EventArgs e)
        {
            var bindingContext = ((TextInput.Children[0] as TextInputView).BindingContext as TextInputViewModel);
            bindingContext.SetImageRequestType(ImageRequestType.Variations);
        }
    }
}