using KitapTakipMauii.Models.Dtos;
using KitapTakipMauii.Services;

namespace KitapTakipMauii.Pages;

public partial class AddBookPage : ContentPage
{
    private readonly ApiService _apiService;

    public AddBookPage()
    {
        InitializeComponent();
        _apiService = new ApiService(new HttpClient()); // Gerekirse DI �zerinden de alabilirsin
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var title = TitleEntry.Text;
        var author = AuthorEntry.Text;
        var genre = GenreEntry.Text;
        var description = DescriptionEntry.Text;
        var imageUrl = ImageUrlEntry.Text;
        var pageCountText = PageCountEntry.Text;

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
        {
            await DisplayAlert("Hata", "L�tfen gerekli alanlar� doldurunuz.", "Tamam");
            return;
        }

        int? pageCount = null;
        if (!string.IsNullOrWhiteSpace(pageCountText) && int.TryParse(pageCountText, out var parsedPageCount))
        {
            pageCount = parsedPageCount;
        }
        else if (!string.IsNullOrWhiteSpace(pageCountText))
        {
            await DisplayAlert("Hata", "Sayfa say�s� ge�erli bir say� olmal�d�r.", "Tamam");
            return;
        }

        var bookDto = new BookCreateDto
        {
            Title = title,
            Author = author,
            Genre = genre,
            Description = description,
            PageCount = pageCount,
            CoverImage = imageUrl
        };

        try
        {
            var response = await _apiService.AddBookAsync(bookDto);

            if (response.Success)
            {
                await DisplayAlert("Ba�ar�l�", "Kitap ba�ar�yla eklendi.", "Tamam");
                await Navigation.PopAsync(); // �nceki sayfaya d�n
            }
            else
            {
                await DisplayAlert("Hata", response.Message ?? "Bir hata olu�tu", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"��lem s�ras�nda hata olu�tu:\n{ex.Message}", "Tamam");
        }
    }
}