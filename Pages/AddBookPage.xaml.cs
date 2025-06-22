using KitapTakipMauii.Models.Dtos;
using KitapTakipMauii.Services;

namespace KitapTakipMauii.Pages;

public partial class AddBookPage : ContentPage
{
    private readonly ApiService _apiService;
    private DateTime _startDate; // Baþlangýç tarihi
    private DateTime _endDate;   // Bitiþ tarihi

    public AddBookPage()
    {
        InitializeComponent();
        _apiService = new ApiService(new HttpClient());
        // Varsayýlan tarih ayarlarý
        if (StartDatePicker != null && EndDatePicker != null)
        {
            _startDate = StartDatePicker.Date;
            _endDate = EndDatePicker.Date;
        }
    }

    // Baþlangýç tarihi seçildiðinde
    private void OnStartDateSelected(object sender, DateChangedEventArgs e)
    {
        _startDate = e.NewDate;
        // Bitiþ tarihi, baþlangýç tarihinden önceyse uyarý ver
        if (_endDate < _startDate)
        {
            DisplayAlert("Uyarý", "Bitiþ tarihi, baþlangýç tarihinden önce olamaz.", "Tamam");
            EndDatePicker.Date = _startDate; // Bitiþ tarihini baþlangýç tarihine eþitle
            _endDate = _startDate;
        }
    }

    // Bitiþ tarihi seçildiðinde
    private void OnEndDateSelected(object sender, DateChangedEventArgs e)
    {
        _endDate = e.NewDate;
        // Bitiþ tarihi, baþlangýç tarihinden önceyse uyarý ver
        if (_endDate < _startDate)
        {
            DisplayAlert("Uyarý", "Bitiþ tarihi, baþlangýç tarihinden önce olamaz.", "Tamam");
            EndDatePicker.Date = _startDate; // Bitiþ tarihini baþlangýç tarihine eþitle
            _endDate = _startDate;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var title = TitleEntry.Text;
        var author = AuthorEntry.Text;
        var genre = GenreEntry.Text;
        var description = DescriptionEntry.Text;
        var imageUrl = ImageUrlEntry.Text;
        var pageCountText = PageCountEntry.Text;

        // Zorunlu alan kontrolü
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(genre))
        {
            await DisplayAlert("Hata", "Lütfen gerekli alanlarý doldurunuz.", "Tamam");
            return;
        }

        int? pageCount = null;
        if (!string.IsNullOrWhiteSpace(pageCountText))
        {
            if (int.TryParse(pageCountText, out var parsedPageCount))
            {
                pageCount = parsedPageCount;
            }
            else
            {
                await DisplayAlert("Hata", "Sayfa sayýsý geçerli bir sayý olmalýdýr.", "Tamam");
                return;
            }
        }

        // Tarih kontrolü
        if (_endDate < _startDate)
        {
            await DisplayAlert("Hata", "Bitiþ tarihi, baþlangýç tarihinden önce olamaz.", "Tamam");
            return;
        }

        var bookDto = new BookCreateDto
        {
            Title = title,
            Author = author,
            Genre = genre,
            Description = description,
            Notes = description, // Notes ve Description aynýysa
            PageCount = pageCount,
            CoverImage = imageUrl,
            StartDate = _startDate, // Baþlangýç tarihi
            EndDate = _endDate      // Bitiþ tarihi
        };

        try
        {
            var response = await _apiService.AddBookAsync(bookDto);

            if (response.Success)
            {
                await DisplayAlert("Baþarýlý", "Kitap baþarýyla eklendi.", "Tamam");
                // Formu sýfýrla
                TitleEntry.Text = AuthorEntry.Text = GenreEntry.Text = DescriptionEntry.Text = PageCountEntry.Text = ImageUrlEntry.Text = string.Empty;
                StartDatePicker.Date = DateTime.Today;
                EndDatePicker.Date = DateTime.Today;
                _startDate = _endDate = DateTime.Today;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Hata", response.Message ?? "Bir hata oluþtu", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Ýþlem sýrasýnda hata oluþtu: {ex.Message}", "Tamam");
        }
    }
}