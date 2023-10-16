using System;
using Component.DB.ViewModels;

namespace Component.DB.Views.ItemDetailPages
{
    public abstract class ItemDomainInventoryBaseDetailPage : ItemDetailPage
    {
        public ItemDomainInventoryBaseDetailPage(ItemDetailViewModel viewModel) : base(viewModel)
        {
            viewModel.LoadItemStatus();
            viewModel.LoadItemLocationInformation();
            viewModel.LoadItemMemberships();

            ItemNameLabel.Text = "Inventory Tag:";
            // Show catalog item information 
            addBindingToDetailsStackLayout("Catalog Item", "Item.DerivedFromItem.Name", 0);            

            addBindingToDetailsStackLayout("QR Id", "FormattedQrId", 0);
            addBindingToDetailsStackLayout("Machine Item", "HousingMachineItem.Name", index: 0,
                valueFontSize: Xamarin.Forms.NamedSize.Subtitle, valueFontAttributes: Xamarin.Forms.FontAttributes.Bold);

            addBindingToDetailsStackLayout("Location", "ItemLocationInformation.LocationString");
            addBindingToDetailsStackLayout("Location Details ", "ItemLocationInformation.LocationDetails");

            addBindingToDetailsStackLayout("Status", "ItemStatusString");
        }
    }
}
