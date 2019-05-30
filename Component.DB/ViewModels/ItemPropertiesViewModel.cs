/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.ViewModels
{
    public class ItemPropertiesViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public Command LoadPropertiesCommand { get; }
        public ObservableCollection<PropertyValue> PropertyValueList { get; }
        public ItemPropertiesViewModel(Item item = null)
        {
            Title = "Properties: " + item.Name;
            Item = item;

            PropertyValueList = new ObservableCollection<PropertyValue>();

            LoadPropertiesCommand = new Command(async () => await ExceuteLoadPropertyValueList());
        }

        async Task ExceuteLoadPropertyValueList()
        {
            try
            {
                 var propertyValues = await itemApi.GetPropertiesForItemAsync(Item.Id);

                foreach (var pv in propertyValues)
                {
                    PropertyValueList.Add(pv);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
