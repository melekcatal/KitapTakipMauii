// KitapTakipMauii/ViewModels/BookContentViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using KitapTakipMauii.Models.Dtos;
using KitapTakipMauii.Services;

namespace KitapTakipMauii.ViewModels;

public partial class BookContentViewModel : ObservableObject
{
	private readonly ApiService _apiService;
	private readonly int _bookId;

	[ObservableProperty]
	private BookDto book = new();

	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private bool isNotBusy = true;

	public BookContentViewModel(ApiService apiService, int bookId)
	{
		_apiService = apiService;
		_bookId = bookId;
		LoadBookContentAsync();
	}

	private async Task LoadBookContentAsync()
	{
		IsBusy = true;
		IsNotBusy = false;

		try
		{
			var response = await _apiService.GetBookPublicByIdAsync(_bookId);
			if (response.Success && response.Data != null)
			{
				Book = response.Data;
			}
			else
			{
				await Shell.Current.DisplayAlert("Hata", response.Message, "Tamam");
			}
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlert("Hata", $"Kitap içeriği yüklenirken hata oluştu: {ex.Message}", "Tamam");
		}
		finally
		{
			IsBusy = false;
			IsNotBusy = true;
		}
	}
}