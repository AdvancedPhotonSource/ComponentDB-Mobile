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
        BrowseCatalog,
        BrowseInventory,
        Settings,
        About,

        // Subpages randered as mainpage
        ItemDetails
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
