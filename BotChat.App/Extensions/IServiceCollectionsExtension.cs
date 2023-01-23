using BotChat.App.Services;
using BotChat.App.ViewModels;
using BotChat.App.Views;
using ChatGPT;

namespace BotChat.App.Extensions
{
    public static class IServiceCollectionsExtension
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            
            return services
                .AddScoped<SettingsViewModel>()
                .AddScoped<LoginViewModel>()
                .AddScoped<RegisterViewModel>()
                .AddScoped<HeaderViewModel>()
                .AddScoped<AccountViewModel>()
                .AddScoped<ImageViewModel>()
                .AddScoped<TextInputViewModel>()
                .AddScoped<MessageViewCellViewModel>()
                .AddScoped<MainViewModel>();
        }

        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            return services
                .AddScoped<TextInputView>()
                .AddScoped<NavigationView>();
        }

        public static IServiceCollection AddPages(this IServiceCollection services)
        {
            return services
                 .AddTransient<SettingsPage>()
                 .AddTransient<LoginPage>()
                 .AddTransient<RegisterPage>()
                 .AddTransient<AccountPage>()
                 .AddTransient<ImagePage>()
                 .AddTransient<MainPage>();
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            return services
                .AddScoped<ILocalSecureStorageService, LocalSecureStorageService>()
                .AddScoped<IHttpClientService, HttpClientService>()
                .AddScoped<IChatGPTService, ChatGPTService>()
                .AddScoped<ISpeechService, SpeechService>()
                .AddScoped<IUserService, UserService>();
        }
    }
}
