using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Component.DB.Services;
using Gov.ANL.APS.CDB.Api;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.ViewModels
{
    public class MultiItemUpdateViewModel : BaseViewModel
    {

        private PropertyTypeApi propertyTypeApi = CdbApiFactory.Instance.propertyTypeApi; 

        private Item _SelectedLocation;
        private String _LocationDetails { get; set; }

        private Boolean _locationDetailsShown;
        private String _locationDetailsPlaceholderText;

        private String _LogEntry { get; set; }
        private String _StatusEntry { get; set; }

        public ObservableCollection<ItemDetailEditViewModel> UpdatableItemList { get; set; }

        private enum StatusListModeEnum
        {
            InventoryOnly,
            CableInventoryOnly,
            Combined,
            Blank
        }
        private StatusListModeEnum statusListMode = StatusListModeEnum.Blank; 
        private List<String> InventoryStatusList;
        private List<String> CableInventoryStatusList;
        private List<String> CombinedStatusList; 

        private const String LOCATION_MODE = "Location";
        private const String STATUS_MODE = "Status";
        private const String LOG_ENTRY_MODE = "Log Entry";

        public List<String> ModePickerList { get; set; }
        private String _ModePickerSelected;

        public MultiItemUpdateViewModel()
        {
            Title = "Update Items";
            UpdatableItemList = new ObservableCollection<ItemDetailEditViewModel>();
            LocationDetails = "";
            _locationDetailsShown = true;

            ModePickerList = new List<string>();
            ModePickerList.Add(LOCATION_MODE);
            ModePickerList.Add(STATUS_MODE);
            ModePickerList.Add(LOG_ENTRY_MODE);

            _ModePickerSelected = LOCATION_MODE;
        }

        public async Task AddItemByIdAsync(int? Id)
        {
            var itemById = itemApi.GetItemById(Id);
            await AddItemByAsync(itemById);
        }

        public async Task AddItemByQrIdAsync(int qrId)
        {
            var itemByQrId = itemApi.GetItemByQrId(qrId);
            await AddItemByAsync(itemByQrId);
        }

        private async Task AddItemByAsync(Item item)
        {
            var qrId = item.QrId;
            bool isNewItemLocation = item.Domain.Name.Equals(Constants.locationDomainName);

            if (LocationMode)
            {
                if (isNewItemLocation && this.SelectedLocation == null)
                {
                    this.SelectedLocation = item;
                    var message = "Changed location by QrId: " + qrId;
                    FireViewModelMessageEvent(message);
                    return;
                }

                if (this.SelectedLocation != null && this.SelectedLocation.Id == item.Id)
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
            foreach (var ittrItem in UpdatableItemList)
            {
                var id = ittrItem.Item.Id;
                if (item.Id == id)
                {
                    var message = "The item with QrId " + qrId + " has already been scanned.";
                    FireViewModelMessageEvent(message);
                    return;

                }
            }

            var hasPemission = await itemApi.VerifyUserPermissionForItemAsync(item.Id);

            if (hasPemission != null && (bool)hasPemission)
            {
                //Add
                var editDetailModel = new ItemDetailEditViewModel(item);
                addToUpdatableItemList(editDetailModel);
                var message = "Added item with QrId: " + qrId;
                FireViewModelMessageEvent(message);
                return;
            }
            else
            {
                var message = "The user does not have sufficient privilages to update scanned item. QrId: " + qrId;
                FireViewModelMessageEvent(message);
                return;
            }
        }

        public void addToUpdatableItemList(ItemDetailEditViewModel itemDetailModel)
        {
            foreach (var idm in UpdatableItemList)
            {
                var item = idm.Item;
                var current = itemDetailModel.Item;

                if (item.Id == current.Id)
                {
                    // No need to add. Item is already in list. 
                    return;
                }
            }

            UpdatableItemList.Add(itemDetailModel);
            updateLocationDetailsShownVariables();

        }

        public void removeFromUpdatableItemList(ItemDetailEditViewModel itemDetailModel)
        {
            UpdatableItemList.Remove(itemDetailModel);
            updateLocationDetailsShownVariables();
        }

        public void UpdateStatusPickerAllowedValues(Picker statusPicker) {
            if (!StatusMode)
            {
                // no need to process. status is not selected
                return;
            }

            StatusListModeEnum currentStatusMode = StatusListModeEnum.Blank;

            if (UpdatableItemList.Count > 0)
            {
                foreach (ItemDetailEditViewModel model in UpdatableItemList)
                {
                    if (model.Item.Domain.Name == Constants.inventoryDomainName)
                    {
                        if (currentStatusMode == StatusListModeEnum.CableInventoryOnly)
                        {
                            currentStatusMode = StatusListModeEnum.Combined;
                            break;
                        }
                        currentStatusMode = StatusListModeEnum.InventoryOnly;
                    } else if (model.Item.Domain.Name == Constants.cableInventoryDomainName)
                    { 
                        if (currentStatusMode == StatusListModeEnum.InventoryOnly)
                        {
                            currentStatusMode = StatusListModeEnum.Combined;
                            break;
                        }
                        currentStatusMode = StatusListModeEnum.CableInventoryOnly;
                    }
                }                
            }            

            if (currentStatusMode == statusListMode)
            {
                 // No change 
                return; 
            }

            statusListMode = currentStatusMode;
            var selection = statusPicker.SelectedItem; 
            statusPicker.Items.Clear();

            List<String> statusList = null;

            switch (statusListMode)
            {
                case StatusListModeEnum.InventoryOnly:
                    LoadInventoryStatusListIfNeeded();
                    statusList = InventoryStatusList;                     
                    break;
                case StatusListModeEnum.CableInventoryOnly:                    
                    LoadCableInventoryStatusListIfNeeded(); 
                    statusList = CableInventoryStatusList;                     
                    break;
                case StatusListModeEnum.Combined:
                    LoadCombinedStatusListIfNeeded(); 
                    statusList = CombinedStatusList; 
                    break;
                default:
                    return;
            }

            // Prepopulate all valid status values for the drop down.
            
            foreach (String status in statusList)
            {                
                statusPicker.Items.Add(status);

                if (selection != null && selection.Equals(status))
                {
                    selection = null; 
                    statusPicker.SelectedItem = status;
                }
            }
        }

        private void PopulateAllowedValuesToStringList(List<String> stringList, PropertyType type)
        {
            foreach (var allowedValue in type.SortedAllowedPropertyValueList)
            {
                stringList.Add(allowedValue.Value); 
            }
        }

        private void LoadInventoryStatusListIfNeeded()
        {
            if (InventoryStatusList == null)
            {
                InventoryStatusList = new List<string>();

                var type = propertyTypeApi.GetInventoryStatusPropertyType();
                PopulateAllowedValuesToStringList(InventoryStatusList, type);
            }
        }

        private void LoadCableInventoryStatusListIfNeeded()
        {
            if (CableInventoryStatusList == null)
            {
                CableInventoryStatusList = new List<string>();

                var type = propertyTypeApi.GetCableInventoryStatusPropertyType();
                PopulateAllowedValuesToStringList(CableInventoryStatusList, type); 
            }
        }

        private void LoadCombinedStatusListIfNeeded()
        {
            if (CombinedStatusList == null)
            {
                LoadInventoryStatusListIfNeeded();
                LoadCableInventoryStatusListIfNeeded();

                CombinedStatusList = new List<string>();

                foreach (String statusVal in InventoryStatusList)
                {
                    if (CableInventoryStatusList.Contains(statusVal))
                    {
                        CombinedStatusList.Add(statusVal); 
                    }
                }
            }
        }

        private void updateLocationDetailsShownVariables()
        {
            var containsInventory = false;
            var containsLocation = false;
            foreach (var model in this.UpdatableItemList)
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
            UpdatableItemList.Clear();
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
                OnPropertyChanged(nameof(SelectedLocationSpecified));
                OnPropertyChanged(nameof(SelectedLocationUnspecified));
                OnPropertyChanged();
            }
        }

        public Boolean SelectedLocationSpecified
        {
            get
            {
                return SelectedLocation != null;
            }
        }

        public Boolean SelectedLocationUnspecified
        {
            get
            {
                return SelectedLocation == null;
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
                String buttonText = "";

                switch (ModePickerSelected)
                {
                    case LOCATION_MODE:
                        buttonText="Relocate Items";
                        break;
                    case LOG_ENTRY_MODE:
                        buttonText="Add Log Entries";
                        break;
                    case STATUS_MODE:
                        buttonText="Update Status for Items";
                        break;
                    default:
                        buttonText="Update Items";
                        break;                    
                }

                var username = this.ActiveUsername;

                if (username != null)
                {
                    return buttonText + " (as user: " + username + ")";
                }
                return buttonText; 
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
