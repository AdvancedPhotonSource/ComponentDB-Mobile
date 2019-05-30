/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */  
using System;
using Component.DB.Utilities;
using Gov.ANL.APS.CDB.Model;

namespace Component.DB.ViewModels
{
    public class ItemGalleryViewModel : BaseViewModel
    {
        public string Image { get; set; }
        public ItemGalleryViewModel(Item item)
        {
            Title = "Gallery: " + item.Name;

            Image = ImageUtility.getImageUrl(item.PrimaryImageForItem, Constants.ImageExtensionOriginal);
            Console.WriteLine(Image);
        }
    }
}
