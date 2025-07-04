using KitapTakipMauii.ViewModels;

namespace KitapTakipMauii.Pages;

public partial class BooksPage : ContentPage
{
    private readonly BooksPageViewModel _viewModel;

    public BooksPage(BooksPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private async void GenreEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        await _viewModel.FilterByGenreAsync();
    }

    private async void AuthorEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        await _viewModel.FilterByAuthorAsync();
    }

    private async void SearchTitle_TextChanged(object sender, TextChangedEventArgs e)
    {
        await _viewModel.SearchByTitleAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is BooksPageViewModel viewModel)
        {
            await viewModel.RefreshAsync();
        }
    }
}