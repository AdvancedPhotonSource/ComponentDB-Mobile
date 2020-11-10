using System;
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

        public ObservableCollection<ItemDetailEditViewModel> LocatableItemList { get; set; }

        public MultiItemRelocateViewModel()
        {
            Title = "Relocate Items";
            LocatableItemList = new ObservableCollection<ItemDetailEditViewModel>();
            LocationDetails = "";
            _locationDetailsShown = true;
        }

        public async Task AddItemByQrIdAsync(int qrId)
        {
            var itemByQrId = itemApi.GetItemByQrId(qrId);

            if (itemByQrId.Domain.Name.Equals(Constants.locationDomainName) && this.SelectedLocation == null)
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

            OnPropertyChanged();
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
    }
}
