// KitapTakipMauii/ViewModels/BookDetailViewModel.cs
using System.IdentityModel.Tokens.Jwt;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMauii.Models.Dtos;
using KitapTakipMauii.Services;
using System.Security.Claims;
using Microsoft.Maui.Controls;
using KitapTakipMauii.Pages;

namespace KitapTakipMauii.ViewModels;

public partial class BookDetailViewModel : ObservableObject
{
	private readonly ApiService _apiService;
	private readonly int _bookId;

	[ObservableProperty]
	private BookDto book = new();

	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private bool isNotBusy = true;

	

	public BookDetailViewModel(ApiService apiService, int bookId)
	{
		_apiService = apiService;
		_bookId = bookId;
		LoadBookDetailsAsync();
	}

	private async Task LoadBookDetailsAsync()
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
				Console.WriteLine($"API hata: {response.Message}");
			}
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlert("Hata", $"Kitap detayları yüklenirken hata oluştu: {ex.Message}", "Tamam");
			Console.WriteLine($"LoadBookDetailsAsync hata: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
			IsNotBusy = true;
		}
	}

	

	[RelayCommand]
	private async Task NavigateToContentAsync(int bookId)
	{
		var viewModel = new BookContentViewModel(_apiService, bookId);
		var page = new BookContent(viewModel);
		await Shell.Current.Navigation.PushAsync(page);
	}
}