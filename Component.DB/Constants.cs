﻿/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using Xamarin.Forms;
namespace Component.DB
{
    public class Constants
    {
        public const string ImageAddressPath = "/api/Downloads/PropertyValue/Image/";
        public const string ItemViewPath = "/views/item/view?id=";

        // Fetching images from CDB
        public const string ImageExtensionOriginal = "/original";
        public const string ImageExtensionThumbnail = "/thumbnail";
        public const string ImageExtensionScaled = "/scaled";

        public const int locationDomainId = 1;
        public const int catalogDomainId = 2;
        public const int inventoryDomainId = 3;
        public const int maarcDomainId = 5;
        public const int machineDesignDomainId = 6;
        public const int cableCatalogDomainId = 7;
        public const int cableInventoryDomainId = 8;
        public const int cableDesignDomainId = 9;

        public const string inventoryDomainName = "Inventory";
        public const string catalogDomainName = "Catalog";
        public const string cableCatalogDomainName = "Cable Catalog";
        public const string cableInventoryDomainName = "Cable Inventory"; 
        public const string locationDomainName = "Location";
        public const string machineDesignDomainName = "Machine Design"; 

        public const string fontIconGithub = "";
        public const string fontIconEdit = "";
        public const string fontIconCamera = "";
        public const string fontIconPlus = "";
        public const string fontIconPicture = "";
        public const string fontIconQrId = "";
        public const string fontIconConfig = "";
        public const string fontIconQuestion = "";
        public const string fontIconBook = "";
        public const string fontIconLocation = "";
    }
}
