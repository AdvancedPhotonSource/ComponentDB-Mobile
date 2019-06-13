using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.ViewModels
{
    public class ItemLocationSelectionViewModel : BaseViewModel
    {
        public ObservableCollection<Item> LocationItemList { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ItemDomainLocation ParentItem { get; set; }  

        public ItemLocationSelectionViewModel(ItemDomainLocation ParentItem = null)
        {
            LocationItemList = new ObservableCollection<Item>();

            if (ParentItem == null)
            {
                Title = "Select Location To Begin"; 
                LoadItemsCommand = new Command(async () => await LoadTopLevelLocationsAsync());
            } else
            {
                this.ParentItem = ParentItem;
                Title = ParentItem.Name;

                LoadItemsCommand = new Command(async () => await LoadChildLocationsAync());
            }
        }

        public async Task LoadChildLocationsAync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                var items = await itemApi.GetChildLocationsAsync(ParentItem.Id);
                AddLocationToList(items);
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.Message); 
            }

            IsBusy = false; 
        }

        public async Task LoadTopLevelLocationsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                var locationItems = await itemApi.GetLocationsTopLevelAsync();
                AddLocationToList(locationItems);
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.Message); 
            }

            IsBusy = false;
        }

        void AddLocationToList(List<ItemDomainLocation> locationItems) 
        {
            LocationItemList.Clear();
            foreach (var item in locationItems)
            {
                LocationItemList.Add(item);
            }
        }

        public Boolean IsSelectCurrentLocationEnabled
        {
            get
            {
                return ParentItem != null; 
            }
        }
    }
}
