/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using Component.DB.ViewModels;

namespace Component.DB.Views.ItemDetailPages
{
    public class ItemDomainInventoryDetailPage : ItemDetailPage
    {
        public ItemDomainInventoryDetailPage(ItemDetailViewModel viewModel) : base(viewModel)
        {
            ItemNameLabel.Text = "Tag:";
            // Show catalog item information 
            // TODO use catalog item attriibute 
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
