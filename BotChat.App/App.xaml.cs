using BotChat.App.Services;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace BotChat.App
{
    public partial class App : Application
    {
        public App(IUserService userService, ISpeechService speechService)
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            base.OnStart();

            //AppCenter.Start("android=e1d4b925-46de-44a9-86b3-f44c4f07404e", typeof(Analytics), typeof(Crashes));
        }
    }
}