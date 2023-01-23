using BotChat.App.ViewModels;
using ChatGPT.Models;

namespace BotChat.App.Views.ViewCells;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MessageViewCell : ViewCell
{
    public static readonly BindableProperty TextProperty =
   BindableProperty.Create("Text", typeof(string), typeof(MessageViewCell), "");

    public static readonly BindableProperty TypeProperty =
   BindableProperty.Create("Type", typeof(ChatType), typeof(MessageViewCell));

    public static readonly BindableProperty DateProperty =
   BindableProperty.Create("Date", typeof(DateTime), typeof(MessageViewCell));

    public static readonly BindableProperty IsHumanProperty =
  BindableProperty.Create("IsHuman", typeof(bool), typeof(MessageViewCell));

    public static readonly BindableProperty IsAIProperty =
  BindableProperty.Create("IsAI", typeof(bool), typeof(MessageViewCell));

    public bool IsHuman
    {
        get { return (bool)GetValue(IsHumanProperty); }
        set { SetValue(IsHumanProperty, value); }
    }

    public bool IsAI
    {
        get { return (bool)GetValue(IsAIProperty); }
        set { SetValue(IsAIProperty, value); }
    }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public ChatType Type
    {
        get { return (ChatType)GetValue(TypeProperty); }
        set { SetValue(TypeProperty, value); }
    }

    public DateTime Date
    {
        get { return (DateTime)GetValue(DateProperty); }
        set { SetValue(DateProperty, value); }
    }

    public MessageViewCell()
    {
        InitializeComponent();
        MainVSL.Padding = new Thickness(5, 15, 0, 15);
    }
}