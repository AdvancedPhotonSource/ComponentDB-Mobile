using System;
using Component.DB.ViewModels;

namespace Component.DB.Views.ItemDetailPages
{
    public abstract class ItemDomainInventoryBaseDetailPage : ItemDetailPage
    {
        public ItemDomainInventoryBaseDetailPage(ItemDetailViewModel viewModel) : base(viewModel)
        {
            ItemNameLabel.Text = "Tag:";
            // Show catalog item information 
            addBindingToDetailsStackLayout("Catalog Item", "Item.DerivedFromItem.Name", 0);

            addBindingToDetailsStackLayout("QR Id", "FormattedQrId", 0);

            // Show status
            viewModel.LoadItemStatus();
            viewModel.LoadItemLocationInformation();

            addBindingToDetailsStackLayout("Location", "ItemLocationInformation.LocationString");
            addBindingToDetailsStackLayout("Location Details ", "ItemLocationInformation.LocationDetails");

            addBindingToDetailsStackLayout("Status", "ItemStatusString");
        }
    }
}
