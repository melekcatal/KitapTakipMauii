using KitapTakipMauii.Models.Dtos;
using KitapTakipMauii.Services;

namespace KitapTakipMauii.Pages;

public partial class AddBookPage : ContentPage
{
    private readonly ApiService _apiService;
    private DateTime _startDate; // Ba�lang�� tarihi
    private DateTime _endDate;   // Biti� tarihi

    public AddBookPage()
    {
        InitializeComponent();
        _apiService = new ApiService(new HttpClient());
        // Varsay�lan tarih ayarlar�
        if (StartDatePicker != null && EndDatePicker != null)
        {
            _startDate = StartDatePicker.Date;
            _endDate = EndDatePicker.Date;
        }
    }

    // Ba�lang�� tarihi se�ildi�inde
    private void OnStartDateSelected(object sender, DateChangedEventArgs e)
    {
        _startDate = e.NewDate;
        // Biti� tarihi, ba�lang�� tarihinden �nceyse uyar� ver
        if (_endDate < _startDate)
        {
            DisplayAlert("Uyar�", "Biti� tarihi, ba�lang�� tarihinden �nce olamaz.", "Tamam");
            EndDatePicker.Date = _startDate; // Biti� tarihini ba�lang�� tarihine e�itle
            _endDate = _startDate;
        }
    }

    // Biti� tarihi se�ildi�inde
    private void OnEndDateSelected(object sender, DateChangedEventArgs e)
    {
        _endDate = e.NewDate;
        // Biti� tarihi, ba�lang�� tarihinden �nceyse uyar� ver
        if (_endDate < _startDate)
        {
            DisplayAlert("Uyar�", "Biti� tarihi, ba�lang�� tarihinden �nce olamaz.", "Tamam");
            EndDatePicker.Date = _startDate; // Biti� tarihini ba�lang�� tarihine e�itle
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

        // Zorunlu alan kontrol�
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(genre))
        {
            await DisplayAlert("Hata", "L�tfen gerekli alanlar� doldurunuz.", "Tamam");
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
                await DisplayAlert("Hata", "Sayfa say�s� ge�erli bir say� olmal�d�r.", "Tamam");
                return;
            }
        }

        // Tarih kontrol�
        if (_endDate < _startDate)
        {
            await DisplayAlert("Hata", "Biti� tarihi, ba�lang�� tarihinden �nce olamaz.", "Tamam");
            return;
        }

        var bookDto = new BookCreateDto
        {
            Title = title,
            Author = author,
            Genre = genre,
            Description = description,
            Notes = description, // Notes ve Description ayn�ysa
            PageCount = pageCount,
            CoverImage = imageUrl,
            StartDate = _startDate, // Ba�lang�� tarihi
            EndDate = _endDate      // Biti� tarihi
        };

        try
        {
            var response = await _apiService.AddBookAsync(bookDto);

            if (response.Success)
            {
                await DisplayAlert("Ba�ar�l�", "Kitap ba�ar�yla eklendi.", "Tamam");
                // Formu s�f�rla
                TitleEntry.Text = AuthorEntry.Text = GenreEntry.Text = DescriptionEntry.Text = PageCountEntry.Text = ImageUrlEntry.Text = string.Empty;
                StartDatePicker.Date = DateTime.Today;
                EndDatePicker.Date = DateTime.Today;
                _startDate = _endDate = DateTime.Today;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Hata", response.Message ?? "Bir hata olu�tu", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"��lem s�ras�nda hata olu�tu: {ex.Message}", "Tamam");
        }
    }
}