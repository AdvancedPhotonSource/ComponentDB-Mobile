/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Component.DB.ViewModels
{
    public enum MenuItemType
    {
        ScanQRCode,
        RelocateItems, 
        BrowseCatalog,
        BrowseCableCatalog,        
        Settings,
        About,        

        // Subpages randered as mainpage
        ItemDetails,

        BrowseInventory,

        //Keep last
        NoSelection
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; }
    }
}
