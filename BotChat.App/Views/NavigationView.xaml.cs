using BotChat.App.ViewModels;
using System.Threading.Tasks;

namespace BotChat.App.Views;

public partial class NavigationView : ContentView
{
    private readonly HeaderViewModel _viewModel;

    public NavigationView(HeaderViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}