using System;
using Component.DB.ViewModels;
using Xamarin.Forms;

namespace Component.DB.Views.ItemDetailPages
{
    public abstract class ItemDomainCatalogBaseDetailPage : ItemDetailPage
    {
        public ItemDomainCatalogBaseDetailPage(ItemDetailViewModel viewModel) : base(viewModel)
        {
            var inventoryButton = new Button
            {
                Text = "View Inventory",
            };

            inventoryButton.Clicked += HandleShowInventoryClicked;
            ButtonsStackLayout.Children.Insert(0, inventoryButton);
        }

        async void HandleShowInventoryClicked(object sender, System.EventArgs e)
        {
            var itemId = viewModel.Item.Id;

            ItemsPage itemsPage = new ItemsPage(MenuItemType.BrowseInventory, (int)itemId);

            await Navigation.PushAsync(itemsPage);
        }
    }
}
