using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMauii.Models.Dtos;
using KitapTakipMauii.Services;
using KitapTakipMauii.Pages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KitapTakipMauii.ViewModels;

public partial class UserBookDetailViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly int _bookId;

    [ObservableProperty]
    private BookDto book = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isNotBusy = true;

	[ObservableProperty]
	private bool canEditNotes;

	[ObservableProperty]
	private string notes;

	public UserBookDetailViewModel(ApiService apiService, int bookId)
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
                Book = response.Data;
				Notes = Book.Notes ?? string.Empty; // Notlar null ise boş
				Console.WriteLine($"Book.Id: {Book.Id}, Book.UserId: {Book.UserId}, Notes: {Notes}");
				await CheckUserAuthorizationAsync();
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
        }
    }


	private async Task CheckUserAuthorizationAsync()
	{
		try
		{
			var token = await _apiService.GetTokenAsync();
			if (string.IsNullOrEmpty(token))
			{
				CanEditNotes = false;
				Console.WriteLine("Token boş, CanEditNotes: false");
				return;
			}

			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);
			var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userId))
			{
				CanEditNotes = false;
				Console.WriteLine("Token'da userId bulunamadı, CanEditNotes: false");
				return;
			}

			if (string.IsNullOrEmpty(Book.UserId))
			{
				CanEditNotes = false;
				Console.WriteLine("Book.UserId boş, CanEditNotes: false");
				return;
			}

			CanEditNotes = userId == Book.UserId;
			Console.WriteLine($"Token UserId: {userId}, Book.UserId: {Book.UserId}, CanEditNotes: {CanEditNotes}");
		}
		catch (Exception ex)
		{
			CanEditNotes = false;
			await Shell.Current.DisplayAlert("Hata", $"Yetki kontrolü sırasında hata: {ex.Message}", "Tamam");
			Console.WriteLine($"Yetki kontrolü hatası: {ex.Message}");
		}
	}

	[RelayCommand]
	private async Task SaveNoteAsync()
	{
		if (!CanEditNotes)
		{
			await Shell.Current.DisplayAlert("Hata", "Bu kitaba not ekleme/düzenleme yetkiniz yok.", "Tamam");
			return;
		}

		IsBusy = true;
		IsNotBusy = false;

		try
		{
			var noteDto = new BookUpdateNoteDto
			{
				Id = _bookId,
				Notes = Notes
			};

			var response = await _apiService.UpdateBookNoteAsync(_bookId, noteDto);
			if (response.Success && response.Data != null)
			{
				Book = response.Data;
				Notes = Book.Notes ?? string.Empty;
				await Shell.Current.DisplayAlert("Başarılı", "Not başarıyla kaydedildi.", "Tamam");
			}
			else
			{
				await Shell.Current.DisplayAlert("Hata", response.Message, "Tamam");
			}
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlert("Hata", $"Not kaydedilirken hata: {ex.Message}", "Tamam");
		}
		finally
		{
			IsBusy = false;
			IsNotBusy = true;
		}
	}


	[RelayCommand]
    private async Task EditBookAsync()
    {
        await Shell.Current.Navigation.PushAsync(new EditBookPage(new EditBookViewModel(_apiService, _bookId)));
    }

    [RelayCommand]
    private async Task DeleteBookAsync()
    {
        if (Book == null) return;

        var confirm = await Shell.Current.DisplayAlert("Onay", $"{Book.Title} kitabını silmek istediğinizden emin misiniz?", "Evet", "Hayır");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            var response = await _apiService.DeleteBookAsync(Book.Id);
            if (response.Success)
            {
                await Shell.Current.DisplayAlert("Başarılı", "Kitap silindi.", "Tamam");
                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", response.Message, "Tamam");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", $"Kitap silinirken hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
            IsNotBusy = true;
        }
    }

	[RelayCommand]
	private async Task BackAsync()
	{
		try
		{
			await Shell.Current.GoToAsync("///MyBooksPage");
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlert("Hata", $"Geri dönüş hatası: {ex.Message}", "Tamam");
		}
	}


}