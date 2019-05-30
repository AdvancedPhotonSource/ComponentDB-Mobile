﻿/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Component.DB.Utilities;
using Gov.ANL.APS.CDB.Model;
using Stormlion.PhotoBrowser;

namespace Component.DB.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        private Item _Item;
        private PropertyValue _ItemStatus; 

        public ItemDetailViewModel(Item item = null)
        {
            this.Item = item;

            updateTitle();
        }

        public void updateTitle()
        {
            if (Item != null) 
            {
                Title = Item.Name;
            }

        }

        public async Task<List<Photo>> GetPhotosForItem()
        {
            try
            {
                List<PropertyValue> images = await itemApi.GetImagePropertiesForItemAsync(Item.Id);

                List<Photo> photos = new List<Photo>();

                foreach (PropertyValue image in images)
                {
                    string url = ImageUtility.getImageUrl(image.Value, Constants.ImageExtensionOriginal);

                    Photo photo = new Photo();
                    photo.URL = url;
                    photos.Add(photo);
                }

                return photos;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null; 
            }
        }

        public string ItemStatusString
        { 
            get
            {
                if (_ItemStatus != null)
                {
                    return _ItemStatus.Value; 
                }
                return "";
            }
        }

        public PropertyValue loadItemStatus()
        {
            if (!Item.Domain.Name.Equals(Constants.inventoryDomainName))
            {
                return null; 
            }
            if (_ItemStatus == null)
            {
                try
                {
                    _ItemStatus = itemApi.GetItemStatus(Item.Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return _ItemStatus;
        }

        public string PrimaryImageUrlScaled
        {
            get
            {
                var primaryImage = Item.PrimaryImageForItem;
                if (primaryImage == null)
                {
                    return "";
                }

                return ImageUtility.getImageUrl(primaryImage, Constants.ImageExtensionScaled);
            }
        }

        public Item Item
        {
            get
            {
                return _Item; 
            }
            set
            {
                this._Item = value;
            }
        }
    }
}
