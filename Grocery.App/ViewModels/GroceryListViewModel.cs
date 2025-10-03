using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        private readonly GlobalViewModel _globalViewModel;

        [ObservableProperty]
        string clientName = string.Empty;
        public Client Client { get; private set; }
        //public IAsyncRelayCommand ShowBoughtProductsCommand { get; }

        public GroceryListViewModel(IGroceryListService groceryListService, GlobalViewModel globalViewModel)
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _globalViewModel = globalViewModel;
            GroceryLists = new(_groceryListService.GetAll());
            //ShowBoughtProductsCommand = new AsyncRelayCommand(ShowBoughtProducts);

            //clientName = _globalViewModel.Client.Name;
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }
        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());

            Client = _globalViewModel.Client;
            ClientName = Client?.Name ?? string.Empty;
            OnPropertyChanged(nameof(Client));
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }
        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            if (_globalViewModel.Client.Role == Role.Admin)
            {
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
            }
        }
    }
}
