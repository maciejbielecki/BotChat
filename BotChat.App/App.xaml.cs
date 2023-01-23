using BotChat.App.Services;

namespace BotChat.App
{
    public partial class App : Application
    {
        public App(IUserService userService, ISpeechService speechService)
        {
            InitializeComponent();

            MainPage = new AppShell();                       
        }
    }
}