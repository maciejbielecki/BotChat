using BotChat.App.ViewModels;
using ChatGPT.Models;

namespace BotChat.App.Views;

public partial class TextInputView : ContentView
{
    private readonly TextInputViewModel _viewModel;

    public TextInputView(TextInputViewModel viewModel, TextInputType type, ImageRequestType? requestType = null)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        MessageEntry.WidthRequest = (DeviceDisplay.MainDisplayInfo.Width - (140 * DeviceDisplay.MainDisplayInfo.Density)) / DeviceDisplay.MainDisplayInfo.Density;
        _viewModel.Type = type;
        if (type == TextInputType.Image) _viewModel.SetImageRequestType((ImageRequestType)requestType);
    }
}