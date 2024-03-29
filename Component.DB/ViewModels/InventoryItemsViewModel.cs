﻿/*
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
        public InventoryItemsViewModel(ItemsPage itemsPage, int parentItemId = -1) : base(itemsPage, parentItemId)
        {
        }

        public override async Task<List<ConciseItem>> getItems()
        {
            ConciseItemOptions opts = new ConciseItemOptions
            {
                IncludePrimaryImageForItem = true,
                IncludeDerivedFromItemInfo = true
            };

            return await itemApi.GetConciseInventoryItemsAsync(opts);            
        }

        public override string getTitle()
        {
            return "Browse Inventory";
        }
    }
}