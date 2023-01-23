using BotChat.App.ViewModels;

namespace BotChat.App.Views;

public partial class TextInputView : ContentView
{
    private readonly TextInputViewModel _viewModel;

    public TextInputView(TextInputViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;
        MessageEntry.WidthRequest = (DeviceDisplay.MainDisplayInfo.Width - (140 * DeviceDisplay.MainDisplayInfo.Density)) / DeviceDisplay.MainDisplayInfo.Density;
    }
}