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
using Gov.ANL.APS.CDB.Client;

namespace Component.DB.ViewModels
{
    public class CatalogItemsViewModel : ItemsViewModel
    {       
        public CatalogItemsViewModel(ItemsPage itemsPage, int parentItemId = -1) : base(itemsPage, parentItemId)
        {
        }

        public override async Task<List<Item>> getItems()
        {
            List<ItemDomainCatalog> itemCatalogList = null;

            var result = AppStorage.GetCatalogBrowseMode();
            if (result == Services.CdbMobileAppStorage.BrowseMode.Favorites)
            {

                Title = getTitle() + " (Favorites)";
                try
                {
                    itemCatalogList = await itemApi.GetFavoriteCatalogItemsAsync();

                    if (itemCatalogList == null)
                    {
                        FireViewModelMessageEvent("It seems that current user does not have favorites. Loading full list.");
                    }
                } catch (ApiException ex)
                {
                    // Unauthorized Exception
                    if (ex.ErrorCode == 401)
                    {
                        // Ignore unauthroized exception but notify user. 
                        // To do, transfer message to view via event. 
                        FireViewModelMessageEvent("Cannot Load Favorites. Please log in and try again.");
                    } else
                    {
                        throw ex;
                    }
                    Debug.WriteLine(ex);
                }
            }

            if (itemCatalogList == null)
            {
                Title = getTitle();
                itemCatalogList = await itemApi.GetCatalogItemsAsync(); 
            }

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