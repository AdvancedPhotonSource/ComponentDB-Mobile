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
    public class CatalogItemsViewModel : ItemsViewModel
    {       
        public CatalogItemsViewModel(ItemsPage itemsPage) : base(itemsPage)
        {
        }

        public override async Task<List<Item>> getItems()
        {
            List<ItemDomainCatalog> itemCatalogList = await itemApi.GetCatalogItemsAsync();
            List<Item> itemList = itemCatalogList.ConvertAll(x => (Item)x); 

            return itemList; 
            //return await cdbRestApi.getCatalogItems(); 
        }

        public override string getTitle()
        {       
            return "Browse Catalog";
        }
    }
}