using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            List<BestSellingProducts> bestSellingProducts = new List<BestSellingProducts>();
            var groceriesList = _groceriesRepository.GetAll();

            //For each productId in groceriesList, count how many times it appears
            Dictionary<int, int> productSales = new Dictionary<int, int>();  
            foreach (var product in groceriesList)
            {
                if (productSales.ContainsKey(product.ProductId))
                {
                    productSales[product.ProductId]+= product.Amount;
                }
                else
                {
                    productSales[product.ProductId] = product.Amount;
                }
            }
            //Order the productSales dictionary by value and take the topX
            var topSellingProducts = productSales.OrderByDescending(x => x.Value).Take(topX).ToDictionary(p => p.Key, p => p.Value);

            //Put the first rank on 1 and increase it for each product
            int rank = 1;
            foreach (var entry in topSellingProducts)
            {
                var prod = _productRepository.Get(entry.Key);
                if (prod != null)
                {
                    bestSellingProducts.Add(new BestSellingProducts(
                        prod.Id,          // ProductId
                        prod.Name,        // Naam
                        prod.Stock,       // Voorraad
                        entry.Value,      // NrOfSells
                        rank              // Ranking
                    ));
                    rank++;
                }
            }
            return bestSellingProducts;
        }
        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
