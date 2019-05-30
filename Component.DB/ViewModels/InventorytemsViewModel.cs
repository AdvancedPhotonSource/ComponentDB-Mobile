/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;


using Component.DB.Views;
using System.Collections.Generic;
using Gov.ANL.APS.CDB.Model;

namespace Component.DB.ViewModels
{
    public class InventoryItemsViewModel : ItemsViewModel
    {       
        public InventoryItemsViewModel()
        {
        }

        public override async Task<List<Item>> getItems()
        {
            List<ItemDomainInventory> inventoryItems = await itemApi.GetInventoryItemsAsync();

            List<Item> itemList = inventoryItems.ConvertAll(x => (Item)x);

            return itemList;
        }

        public override string getTitle()
        {
            return "Browse Inventory";
        }
    }
}