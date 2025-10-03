using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

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

        public GroceryListViewModel(IGroceryListService groceryListService, GlobalViewModel globalViewModel)
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _globalViewModel = globalViewModel;
            GroceryLists = new(_groceryListService.GetAll());
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
            Debug.WriteLine("ShowBoughtProdcuts gestart....");
            if (_globalViewModel.Client.Role == Role.Admin)
            {
                Debug.WriteLine("Admin heeft ingelogd...");
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
            }
        }
    }
}
