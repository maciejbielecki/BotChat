using CommunityToolkit.Maui.Views;

namespace BotChat.App.Views;

public partial class MicrophonePopup : Popup
{ 
    public EventHandler OnStopClick { get; set; }
    public MicrophonePopup()
    {
        InitializeComponent();        
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        OnStopClick.Invoke(sender, e);
    }
}