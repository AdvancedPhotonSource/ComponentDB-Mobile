/*
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
    public class ItemDetailEditViewModel : ItemDetailViewModel
    {
        public ItemDetailEditViewModel(Item item = null)
        {
            Title = "Edit " + item.Name;
            try {
                Item = itemApi.GetItemById(item.Id);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task<Item> UpdateItem()
        {
            try
            {
                return await itemApi.UpdateItemDetailsAsync(Item);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }
        }

        public async Task<ItemLocationInformation> UpdateItemLocationAsync()
        {
            if (this.ItemLocationInformation == null)
            {
                throw new Exception("The location was never loaded so it cannot be modified"); 
            } 

            var dbLocationInformation = itemApi.GetItemLocation(Item.Id);

            if (ItemLocationInformation.LocationDetails == null)            
                ItemLocationInformation.LocationDetails = "";
            if (dbLocationInformation.LocationDetails == null)            
                dbLocationInformation.LocationDetails = "";

            // -1 means null
            int curLocationd = -1;
            int origLocationId = -1; 
            if (ItemLocationInformation.LocationItem != null)
            {
                curLocationd = (int)ItemLocationInformation.LocationItem.Id;
            }
            if (dbLocationInformation.LocationItem != null)
            {
                origLocationId = (int)dbLocationInformation.LocationItem.Id;
            }

            var updateNeeded = false;
            if (!ItemLocationInformation.LocationDetails.Equals(dbLocationInformation.LocationDetails))
            {
                updateNeeded = true;
            }
            else if (curLocationd != origLocationId)
            {
                updateNeeded = true;
            }

            if (updateNeeded)
            {
                int? locationId = null;
                var locationItem = ItemLocationInformation.LocationItem;

                if (locationItem != null)
                {
                    locationId = locationItem.Id;
                }

                var locationDetails = ItemLocationInformation.LocationDetails;
                var args = new SimpleLocationInformation(Item.Id, locationId, locationDetails);

                itemApi.UpdateItemLocation(args); 
            }

            return this.ItemLocationInformation;
        }

        public async Task<PropertyValue> UpdateItemStatusAsync(String newStatus)
        {
            if (!Item.Domain.Name.Equals(Constants.inventoryDomainName))
            {
                // Can currently only update for inventory. 
                return null; 
            }
            var itemStatus = await itemApi.GetItemStatusAsync(Item.Id);

            PropertyValue submitPropertyValue; 

            if (itemStatus == null)
            {
                var pt = await PropertyApi.GetInventoryStatusPropertyTypeAsync();

                submitPropertyValue = new PropertyValue();
                submitPropertyValue.Value = newStatus;
                submitPropertyValue.PropertyType = pt;
            } else
            {
                if (itemStatus.Value.Equals(newStatus))
                {
                    // Already updated 
                    return itemStatus; 
                }
                submitPropertyValue = itemStatus;
                itemStatus.Value = newStatus; 
            }


            try
            {
                return await itemApi.UpdateItemPropertyValueAsync(Item.Id, submitPropertyValue); 
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }
        }

        public string QrIdEntry
        {
            get
            {
                if (Item.QrId != null)
                {
                    return Item.QrId.ToString();
                }
                return "";
            } set
            {
                try
                {
                    var result = int.Parse(value);
                    Item.QrId = result;
                } catch (Exception ex)
                {
                    Item.QrId = null; 
                }

            }
        }

    }
}
