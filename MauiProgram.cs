using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KitapTakipMauii.Services;
using KitapTakipMauii.ViewModels;
using KitapTakipMauii.Pages;

namespace KitapTakipMauii;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Servis kayıtları
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<ApiService>();

        // ViewModel kayıtları
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<BooksViewModel>();
        builder.Services.AddTransient<EditBookViewModel>();
        builder.Services.AddTransient<BookAddViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<BooksPageViewModel>();
        builder.Services.AddTransient<BookDetailViewModel>();
        builder.Services.AddTransient<MyBooksPageViewModel>();
        builder.Services.AddTransient<UserBookDetailViewModel>();
        builder.Services.AddTransient<BookContentViewModel>();

        // Page kayıtları
        builder.Services.AddTransient<Pages.LoginPage>();
        builder.Services.AddTransient<Pages.RegisterPage>();
        builder.Services.AddTransient<Pages.BooksPage>();
        builder.Services.AddTransient<Pages.ProfilePage>();
        builder.Services.AddTransient<Pages.BookDetailPage>();
        builder.Services.AddTransient<Pages.AddBookPage>();
        builder.Services.AddTransient<Pages.EditBookPage>();
        builder.Services.AddTransient<Pages.UserBookDetailPage>();
        builder.Services.AddTransient<AdminPanelViewModel>();
        builder.Services.AddTransient<AdminPanel>();
        builder.Services.AddTransient<MyBooksPage>();
		builder.Services.AddTransient<Pages.BookContent>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}