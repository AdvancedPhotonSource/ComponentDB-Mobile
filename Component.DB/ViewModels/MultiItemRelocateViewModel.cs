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
        public ObservableCollection<ItemDetailEditViewModel> LocatableItemList { get; set; }

        public MultiItemRelocateViewModel()
        {
            Title = "Relocate Items";
            LocatableItemList = new ObservableCollection<ItemDetailEditViewModel>();
            LocationDetails = ""; 
        }

        public async Task AddItemByQrIdAsync(int qrId)
        {
            var itemByQrId = itemApi.GetItemByQrId(qrId);

            if (itemByQrId.Domain.Name.Equals(Constants.locationDomainName))
            {
                this.SelectedLocation = itemByQrId; 
                var message = "Changed location by QrId: " + qrId;
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
                LocatableItemList.Add(editDetailModel);
                var message= "Added item with QrId: " + qrId;
                FireViewModelMessageEvent(message);
                return;
            } else
            {
                var message = "The user does not have sufficient privilages to relocate scanned item. QrId: " + qrId;
                FireViewModelMessageEvent(message);
                return;
            }
        }

        public void ClearItems()
        {
            LocatableItemList.Clear(); 
        }

        public Item SelectedLocation
        {
            get
            {
                return _SelectedLocation; 
            } set
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
    }
}
