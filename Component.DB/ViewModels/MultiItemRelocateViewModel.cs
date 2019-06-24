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

        public async Task<string> AddItemByQrIdAsync(int qrId)
        {
            var itemByQrId = itemApi.GetItemByQrId(qrId);

            if (itemByQrId.Domain.Name.Equals(Constants.locationDomainName))
            {
                this.SelectedLocation = itemByQrId; 
                return "Changed location by QrId: " + qrId; 
            }


            // Verify not already in list
            foreach (var item in LocatableItemList)
            {
                var id = item.Item.Id; 
                if (itemByQrId.Id == id)
                {
                    return "The item with QrId " + qrId + " has already been scanned."; 

                }
            }
            
            var hasPemission = await itemApi.VerifyUserPermissionForItemAsync(itemByQrId.Id);

            if (hasPemission != null && (bool)hasPemission) 
            {
                //Add
                var editDetailModel = new ItemDetailEditViewModel(itemByQrId);
                LocatableItemList.Add(editDetailModel);
                return "Added item with QrId: " + qrId;
            } else
            {
                return "The user does not have sufficient privilages to relocate scanned item. QrId: " + qrId; 
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
