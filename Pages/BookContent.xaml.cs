using KitapTakipMauii.ViewModels;
namespace KitapTakipMauii.Pages;

public partial class BookContent : ContentPage
{
	public BookContent(BookContentViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}