using ChatGPT.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BotChat.App.ViewModels
{
    public partial class MessageViewCellViewModel : ObservableObject
    {
        [ObservableProperty]
        string text;

        [ObservableProperty]
        ChatType type;

        [ObservableProperty]
        DateTime date;

        public MessageViewCellViewModel()
        {
        }
    }
}
