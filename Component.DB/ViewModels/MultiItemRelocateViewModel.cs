using System;
using System.Collections.ObjectModel;
using Gov.ANL.APS.CDB.Model;

namespace Component.DB.ViewModels
{
    public class MultiItemRelocateViewModel : BaseViewModel
    {

        private Item _SelectedLocation;
        public ObservableCollection<ItemDetailEditViewModel> LocatableItemList { get; set; }

        public MultiItemRelocateViewModel()
        {
            Title = "Relocate Items";
            LocatableItemList = new ObservableCollection<ItemDetailEditViewModel>(); 
        }

        public String addItemByQrId(int qrId)
        {
            var result = itemApi.GetItemByQrId(qrId);

            if (result.Domain.Name.Equals(Constants.locationDomainName))
            {
                this.SelectedLocation = result; 
                return "Changed location by QrId: " + qrId; 
            }


            // Verify not already in list
            foreach (var item in LocatableItemList)
            {
                var id = item.Item.Id; 
                if (result.Id == id)
                {
                    return "The item with QrId " + qrId + " has already been scanned."; 

                }
            }
            
            // TODO Verify permission

            //Add
            var editDetailModel = new ItemDetailEditViewModel(result);
            LocatableItemList.Add(editDetailModel);
            return "Added item with QrId: " + qrId;
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
    }
}
