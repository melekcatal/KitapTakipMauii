using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMauii.Models.Dtos;
using KitapTakipMauii.Pages;
using KitapTakipMauii.Services;

namespace KitapTakipMauii.ViewModels;

public partial class EditBookViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private int _bookId;

    public void SetBookId(int bookId) => _bookId = bookId;

    [ObservableProperty] private int id;
    [ObservableProperty] private string title;
    [ObservableProperty] private string author;
    [ObservableProperty] private string genre;
    [ObservableProperty] private string notes;
    [ObservableProperty] private string description;
    [ObservableProperty] private int? pageCount;
    [ObservableProperty] private string coverImage;
    [ObservableProperty] private DateTime startDate = DateTime.Today;
    [ObservableProperty] private DateTime endDate = DateTime.Today;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isNotBusy = true;

    public EditBookViewModel(ApiService apiService, int bookId)
    {
        _apiService = apiService;
        _bookId = bookId;
    }


    public async Task LoadBookAsync()
    {
        IsBusy = true;
        IsNotBusy = false;

        try
        {
            var response = await _apiService.GetBookByIdAsync(_bookId);
            if (response.Success && response.Data != null)
            {
                Id = response.Data.Id;
                Title = response.Data.Title;
                Author = response.Data.Author;
                Genre = response.Data.Genre;
                Description = response.Data.Description;
                PageCount = response.Data.PageCount;
                CoverImage = response.Data.CoverImage;
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", response.Message ?? "Kitap yüklenemedi.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", $"Kitap yüklenirken hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
            IsNotBusy = true;
        }
    }

    [RelayCommand]
    private async Task SaveBookAsync()
    {
        if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Author) &&
            string.IsNullOrWhiteSpace(Genre) && string.IsNullOrWhiteSpace(Notes) &&
            string.IsNullOrWhiteSpace(Description) && string.IsNullOrWhiteSpace(CoverImage) &&
            !PageCount.HasValue)
        {
            await Shell.Current.DisplayAlert("Hata", "Lütfen en az bir alanı güncelleyiniz.", "Tamam");
            return;
        }

        var bookUpdateDto = new BookUpdateDto
        {
            Id = Id,
            Title = Title,
            Author = Author,
            Genre = Genre,
            PageCount = PageCount,
            Description = Description,
            CoverImage = CoverImage,
        };

        IsBusy = true;
        try
        {
            var response = await _apiService.UpdateBookAsync(Id, bookUpdateDto);
            if (response.Success)
            {
                System.Diagnostics.Debug.WriteLine($"Yönlendirme yapılıyor, BookId: {Id}");
                await Shell.Current.DisplayAlert("Başarılı", "Kitap güncellendi.", "Tamam");
                await Shell.Current.Navigation.PushAsync(new UserBookDetailPage(new UserBookDetailViewModel(_apiService, bookUpdateDto.Id)));
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", response.Message, "Tamam");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", $"Kitap güncellenirken hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
            IsNotBusy = true;
        }
    }

	[RelayCommand]
	private async Task CancelAsync()
	{
		try
		{
			// Navigasyon yığınını logla
			var navStack = Shell.Current.Navigation.NavigationStack;
			Console.WriteLine("Navigation Stack:");
			foreach (var page in navStack)
			{
				Console.WriteLine(page?.GetType().Name);
			}

			// MyBooksPage'e git
			await Shell.Current.GoToAsync("//MyBooksPage");
			Console.WriteLine("Navigated to MyBooksPage successfully.");
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlert("Hata", $"Navigasyon hatası: {ex.Message}", "Tamam");
			Console.WriteLine($"Navigation error: {ex.Message}");
		}
	}
}
