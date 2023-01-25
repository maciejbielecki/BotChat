namespace BotChat.App.Views;

public partial class LoadingMessageView : ContentView
{ 
    public LoadingMessageView()
    {
        InitializeComponent();
        MainThread.InvokeOnMainThreadAsync(() => DotOneAnimationUp());
    }

    private void DotOneAnimationUp()
    {
        var animation = new Animation((size) => { DotOne.FontSize = size; }, 14, 24, Easing.Linear, () => { DotOneAnimationDown(); DotTwoAnimationUp(); });
        DotOne.Animate("dotOneAnimate", animation, length: 500);
    }

    private void DotOneAnimationDown()
    {
        var animation = new Animation((size) => { DotOne.FontSize = size; }, 24, 14, Easing.Linear);
        DotOne.Animate("dotOneAnimate", animation, length: 500);
    }

    private void DotTwoAnimationUp()
    {
        var animation = new Animation((size) => { DotOne.FontSize = size; }, 14, 24, Easing.Linear, () => { DotTwoAnimationDown(); DotThreeAnimationUp(); });
        DotOne.Animate("dotOneAnimate", animation, length: 500);
    }

    private void DotTwoAnimationDown()
    {
        var animation = new Animation((size) => { DotOne.FontSize = size; }, 24, 14, Easing.Linear);
        DotOne.Animate("dotOneAnimate", animation, length: 500);
    }

    private void DotThreeAnimationUp()
    {
        var animation = new Animation((size) => { DotOne.FontSize = size; }, 14, 24, Easing.Linear, () => { DotTwoAnimationDown(); DotOneAnimationUp(); });
        DotOne.Animate("dotOneAnimate", animation, length: 500);
    }

    private void DotThreeAnimationDown()
    {
        var animation = new Animation((size) => { DotOne.FontSize = size; }, 24, 14, Easing.Linear);
        DotOne.Animate("dotOneAnimate", animation, length: 500);
    }
}