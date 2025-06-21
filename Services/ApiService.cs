using System.Net.Http.Headers;
using System.Net.Http.Json;
using KitapTakipMauii.Models.Dtos;
using KitapTakipMauii.Models.Responses;

namespace KitapTakipMauii.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://localhost:7220/api/";

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task SetTokenAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await SecureStorage.SetAsync("jwt_token", token);
    }

    public async Task<string> GetTokenAsync()
    {
        return await SecureStorage.GetAsync("jwt_token") ?? string.Empty;
    }

    public async Task ClearTokenAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        SecureStorage.Remove("jwt_token");
    }

    public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/register", registerDto);
        return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }

    public async Task<ApiResponse<string>> LoginAsync(LoginDto loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        if (result.Success)
        {
            await SetTokenAsync(result.Data);
        }
        return result;
    }

    public async Task<ApiResponse<string>> LoginAdminAsync(LoginDto loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login-admin", loginDto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        if (result.Success)
        {
            await SetTokenAsync(result.Data);
        }
        return result;
    }

    public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/change-password", changePasswordDto);
        return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }

    public async Task<ApiResponse<string>> UpdateProfileAsync(UpdateProfileDto updateProfileDto)
    {
        var response = await _httpClient.PutAsJsonAsync("auth/profile", updateProfileDto);
        return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }

    public async Task<ApiResponse<List<UserDto>>> GetAllUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<ApiResponse<List<UserDto>>>("auth/users");
    }

    public async Task<ApiResponse<string>> DeleteUserAsync(string userName)
    {
        var response = await _httpClient.DeleteAsync($"auth/users/{userName}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }

    public async Task<ApiResponse<List<BookDto>>> GetBooksAsync()
    {
        return await _httpClient.GetFromJsonAsync<ApiResponse<List<BookDto>>>("books");
    }

    public async Task<ApiResponse<BookDto>> GetBookByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ApiResponse<BookDto>>($"books/{id}");
    }

    public async Task<ApiResponse<List<BookDto>>> GetUserBooksAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            return new ApiResponse<List<BookDto>>
            {
                Success = false,
                Message = "Oturum açılmamış. Lütfen giriş yapın.",
                Data = null
            };
        }
        await SetTokenAsync(token);
        try
        {
            var response = await _httpClient.GetAsync("books");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new ApiResponse<List<BookDto>>
                {
                    Success = false,
                    Message = "Yetkisiz erişim: Geçersiz veya eksik token.",
                    Data = null
                };
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<BookDto>>>();
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<BookDto>>
            {
                Success = false,
                Message = $"Kitaplar getirilirken hata: {ex.Message}",
                Data = null
            };
        }
    }

    public async Task<ApiResponse<List<BookDto>>> GetBooksByAuthorNameAsync(string authorName)
    {
        return await _httpClient.GetFromJsonAsync<ApiResponse<List<BookDto>>>("books/by-author?authorName=" + Uri.EscapeDataString(authorName));
    }

    public async Task<ApiResponse<List<BookDto>>> GetBooksByGenreAsync(string genre)
    {
        return await _httpClient.GetFromJsonAsync<ApiResponse<List<BookDto>>>("books/by-genre?genre=" + Uri.EscapeDataString(genre));
    }

    public async Task<ApiResponse<List<BookDto>>> GetBooksByTitleAsync(string title)
    {
        return await _httpClient.GetFromJsonAsync<ApiResponse<List<BookDto>>>("books/by-title?title=" + Uri.EscapeDataString(title));
    }

    public async Task<ApiResponse<BookDto>> AddBookAsync(BookCreateDto bookDto)
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            return new ApiResponse<BookDto>
            {
                Success = false,
                Message = "Oturum açılmamış. Lütfen giriş yapın.",
                Data = null
            };
        }

        // Token'ı yeniden ekle (güvenli olması için)
        await SetTokenAsync(token);

        var response = await _httpClient.PostAsJsonAsync("books", bookDto);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return new ApiResponse<BookDto>
            {
                Success = false,
                Message = "Yetkisiz erişim: Geçersiz veya eksik token.",
                Data = null
            };
        }

        return await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>();
    }



	public async Task<ApiResponse<BookDto>> GetBookPublicByIdAsync(int id)
	{
		try
		{
			var response = await _httpClient.GetAsync($"books/public/{id}");
			if (!response.IsSuccessStatusCode)
			{
				return new ApiResponse<BookDto>
				{
					Success = false,
					Message = $"İstek başarısız: {response.ReasonPhrase}",
					Data = null
				};
			}

		
	    return await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>() ?? new ApiResponse<BookDto>
	    {
		    Success = false,
		    Message = "Veri çözümlenemedi.",
		    Data = null
	    };
		    }
		    catch (Exception ex)
		    {
			    return new ApiResponse<BookDto>
			    {
				    Success = false,
				    Message = $"Kitap getirilemedi: {ex.Message}",
				    Data = null
			    };
		    }
	}
	public async Task<ApiResponse<List<BookDto>>> GetAllBooksAsync() // Yeni eklenen metod
    {
        try
        {
            var response = await _httpClient.GetAsync("books/all");
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<BookDto>>
                {
                    Success = false,
                    Message = $"İstek başarısız: {response.ReasonPhrase}",
                    Data = null
                };
            }
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<BookDto>>>() ?? new ApiResponse<List<BookDto>> { Success = false, Message = "Veri alınamadı", Data = null };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<BookDto>>
            {
                Success = false,
                Message = $"Tüm kitaplar getirilirken hata: {ex.Message}",
                Data = null
            };
        }
    }

    public async Task<ApiResponse<BookDto>> UpdateBookAsync(int id, BookUpdateDto bookDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"books/{id}", bookDto);
        return await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>();
    }


	// KitapTakipMauii/Services/ApiService.cs
	public async Task<ApiResponse<BookDto>> UpdateBookNoteAsync(int id, BookUpdateNoteDto noteDto)
	{
		var token = await GetTokenAsync();
		if (string.IsNullOrEmpty(token))
		{
			return new ApiResponse<BookDto>
			{
				Success = false,
				Message = "Oturum açılmamış. Lütfen giriş yapın.",
				Data = null
			};
		}

		await SetTokenAsync(token);
		try
		{
			var response = await _httpClient.PatchAsJsonAsync($"/api/books/{id}/note", noteDto);
			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				return new ApiResponse<BookDto>
				{
					Success = false,
					Message = "Yetkisiz erişim: Geçersiz veya eksik token.",
					Data = null
				};
			}
			if (!response.IsSuccessStatusCode)
			{
				return new ApiResponse<BookDto>
				{
					Success = false,
					Message = $"Not güncellenemedi: {response.ReasonPhrase}",
					Data = null
				};
			}
			return await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>();
		}
		catch (Exception ex)
		{
			return new ApiResponse<BookDto>
			{
				Success = false,
				Message = $"Not güncellenirken hata: {ex.Message}",
				Data = null
			};
		}
	}

	public async Task<ApiResponse<bool>> DeleteBookAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"books/{id}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}