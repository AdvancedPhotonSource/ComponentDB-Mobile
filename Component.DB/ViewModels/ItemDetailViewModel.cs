/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Component.DB.Services;
using Component.DB.Utilities;
using Gov.ANL.APS.CDB.Model;
using Stormlion.PhotoBrowser;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Component.DB.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        protected Item _Item;
        protected ConciseItem _conciseItem;
        private PropertyValue _ItemStatus;
        private ItemLocationInformation _ItemLocationInformation;

        public ICommand ViewInPortalCommand { get; }

        public ItemDetailViewModel(Item item = null,
            ConciseItem conciseItem = null)
        {
            this.Item = item;
            this.ConciseItem = conciseItem; 

            updateTitle();

            ViewInPortalCommand = new Command(() => OpenViewInPortalCommand());
        }

        public void OpenViewInPortalCommand()
        {
            var appStorage = CdbMobileAppStorage.Instance;
            var address = appStorage.getActiveConfiguration().CdbAddress;

            var portalUrl = address + Constants.ItemViewPath + Item.Id;

            Launcher.OpenAsync(new Uri(portalUrl));
        }

        public void loadFromQrId(int qrId)
        {
            this.Item = itemApi.GetItemByQrId(qrId);
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

        private Boolean VerifyItemIsInventory()
        {
            return Item.DomainId == Constants.inventoryDomainId
                || Item.DomainId == Constants.cableInventoryDomainId;
        }

        public PropertyValue LoadItemStatus()
        {
            if (!VerifyItemIsInventory())
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

        public ItemLocationInformation LoadItemLocationInformation()
        {
            if (!VerifyItemIsInventory())
            {
                return null;
            }
            if (_ItemLocationInformation == null)
            {
                try
                {
                    _ItemLocationInformation = itemApi.GetItemLocation(Item.Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return _ItemLocationInformation;
        }

        public string PrimaryImageUrlScaled
        {
            get
            {
                String primaryImage = null; 
                if (Item != null)
                {
                    primaryImage = Item.PrimaryImageForItem;
                } else if (ConciseItem != null)
                {
                    primaryImage = ConciseItem.PrimaryImageForItem; 
                }
                
                if (primaryImage == null)
                {
                    return "";
                }

                return ImageUtility.getImageUrl(primaryImage, Constants.ImageExtensionScaled);
            }
        }

        protected string formatQrId(int? qrId)
        {
            if (qrId == null)
            {
                return "";
            }
            return String.Format("{0:000 000 000}", qrId);
        }

        public string FormattedQrId
        {
            get
            {
                var qrId = Item.QrId;
                return formatQrId(qrId);
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

        public ConciseItem ConciseItem
        {
            get
            {
                return _conciseItem;
            }
            set
            {
                this._conciseItem = value; 
            }
        }

        public ItemLocationInformation ItemLocationInformation
        {
            get
            {
                return _ItemLocationInformation; 
            }
            set
            {
                _ItemLocationInformation = value;
                OnPropertyChanged();
            }
        }
    }
}
