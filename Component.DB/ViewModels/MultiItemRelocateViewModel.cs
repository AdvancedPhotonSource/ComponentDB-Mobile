﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gov.ANL.APS.CDB.Model;

namespace Component.DB.ViewModels
{
    public class MultiItemRelocateViewModel : BaseViewModel
    {

        private Item _SelectedLocation;
        private String _LocationDetails { get; set; }

        private Boolean _locationDetailsShown;
        private String _locationDetailsPlaceholderText;

        private String _LogEntry { get; set; }
        private String _StatusEntry { get; set; }

        public ObservableCollection<ItemDetailEditViewModel> LocatableItemList { get; set; }

        private const String LOCATION_MODE = "Location";
        private const String STATUS_MODE = "Status";
        private const String LOG_ENTRY_MODE = "Log Entry";

        public List<String> ModePickerList { get; set; }
        private String _ModePickerSelected;

        public MultiItemRelocateViewModel()
        {
            Title = "Update Items";
            LocatableItemList = new ObservableCollection<ItemDetailEditViewModel>();
            LocationDetails = "";
            _locationDetailsShown = true;

            ModePickerList = new List<string>();
            ModePickerList.Add(LOCATION_MODE);
            ModePickerList.Add(STATUS_MODE);
            ModePickerList.Add(LOG_ENTRY_MODE);

            _ModePickerSelected = LOCATION_MODE;
        }

        public async Task AddItemByQrIdAsync(int qrId)
        {
            var itemByQrId = itemApi.GetItemByQrId(qrId);

            bool isNewItemLocation = itemByQrId.Domain.Name.Equals(Constants.locationDomainName);

            if (LocationMode)
            {
                if (isNewItemLocation && this.SelectedLocation == null)
                {
                    this.SelectedLocation = itemByQrId;
                    var message = "Changed location by QrId: " + qrId;
                    FireViewModelMessageEvent(message);
                    return;
                }

                if (this.SelectedLocation != null && this.SelectedLocation.Id == itemByQrId.Id)
                {
                    var message = "The item with QrId " + qrId + " has already been scanned. It is the active location.";
                    FireViewModelMessageEvent(message);
                    return;
                }
            }

            if (StatusMode)
            {
                if (isNewItemLocation)
                {
                    var message = "Status mode is only for inventory items. Location was scanned with qrid: " + qrId;
                    FireViewModelMessageEvent(message);
                    return; 
                }
            }

            // Verify not already in list
            foreach (var item in LocatableItemList)
            {
                var id = item.Item.Id;
                if (itemByQrId.Id == id)
                {
                    var message = "The item with QrId " + qrId + " has already been scanned.";
                    FireViewModelMessageEvent(message);
                    return;

                }
            }

            var hasPemission = await itemApi.VerifyUserPermissionForItemAsync(itemByQrId.Id);

            if (hasPemission != null && (bool)hasPemission)
            {
                //Add
                var editDetailModel = new ItemDetailEditViewModel(itemByQrId);
                addToLocatableItemList(editDetailModel);
                var message = "Added item with QrId: " + qrId;
                FireViewModelMessageEvent(message);
                return;
            }
            else
            {
                var message = "The user does not have sufficient privilages to relocate scanned item. QrId: " + qrId;
                FireViewModelMessageEvent(message);
                return;
            }
        }

        public void addToLocatableItemList(ItemDetailEditViewModel itemDetailModel)
        {
            LocatableItemList.Add(itemDetailModel);
            updateLocationDetailsShownVariables();
        }

        public void removeFromLocatableItemList(ItemDetailEditViewModel itemDetailModel)
        {
            LocatableItemList.Remove(itemDetailModel);
            updateLocationDetailsShownVariables();
        }

        private void updateLocationDetailsShownVariables()
        {
            var containsInventory = false;
            var containsLocation = false;
            foreach (var model in this.LocatableItemList)
            {
                var item = model.Item;
                var domainName = item.Domain.Name;

                switch (domainName)
                {
                    case Constants.locationDomainName:
                        containsLocation = true;
                        break;
                    case Constants.inventoryDomainName:
                        containsInventory = true;
                        break;
                    default:
                        break;
                }
            }
            if (containsInventory && !containsLocation)
            {
                locationDetailsShown = true;
                locationDetailsPlaceholderText = "";
            }
            else if (containsInventory && containsLocation)
            {
                locationDetailsShown = true;
                locationDetailsPlaceholderText = "Applicable to equipment only.";
            }
            else
            {
                locationDetailsShown = false;
            }
        }

        public void ClearItems()
        {
            LocatableItemList.Clear();
            updateLocationDetailsShownVariables();
        }

        public Item SelectedLocation
        {
            get
            {
                return _SelectedLocation;
            }
            set
            {
                _SelectedLocation = value;
                OnPropertyChanged();
            }
        }

        public String LocationDetails
        {
            get
            {
                return _LocationDetails;
            }
            set
            {
                _LocationDetails = value;
                OnPropertyChanged();
            }
        }

        public String locationDetailsPlaceholderText
        {
            get
            {
                return _locationDetailsPlaceholderText;
            }
            set
            {
                _locationDetailsPlaceholderText = value;
                OnPropertyChanged();
            }
        }

        public Boolean locationDetailsShown
        {
            get
            {
                return _locationDetailsShown;
            }
            set
            {
                _locationDetailsShown = value;
                OnPropertyChanged();
            }
        }

        public String LogEntry
        {
            get
            {
                return _LogEntry;
            }
            set
            {
                _LogEntry = value;
                OnPropertyChanged();
            }
        }

        public String StatusEntry
        {
            get
            {
                return _StatusEntry; 
            }
            set
            {
                _StatusEntry = value;
                OnPropertyChanged(); 
            }
        }

        public Boolean LocationMode
        {
            get
            {
                return ModePickerSelected.Equals(LOCATION_MODE);
            }
        }

        public Boolean StatusMode
        {
            get
            {
                return ModePickerSelected.Equals(STATUS_MODE);
            }
        }

        public Boolean LogMode
        {
            get
            {
                return ModePickerSelected.Equals(LOG_ENTRY_MODE);
            }
        }

        public String UpdateItemsButtonText
        {
            get
            {
                switch (ModePickerSelected)
                {
                    case LOCATION_MODE:
                        return "Relocate Items";
                    case LOG_ENTRY_MODE:
                        return "Add Log Entries";
                    case STATUS_MODE:
                        return "Update Status for Items";
                    default:
                        return "Update Items"; 
                }                
            }
        }

        public String ModePickerSelected
        {
            get
            {
                return _ModePickerSelected;
            }
            set
            {
                _ModePickerSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LocationMode));
                OnPropertyChanged(nameof(StatusMode));
                OnPropertyChanged(nameof(LogMode));
                OnPropertyChanged(nameof(UpdateItemsButtonText));
            }
        }
    }
}
