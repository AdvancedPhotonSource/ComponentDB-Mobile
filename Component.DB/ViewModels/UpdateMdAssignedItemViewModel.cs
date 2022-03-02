using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Component.DB.Services;
using Gov.ANL.APS.CDB.Api;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.ViewModels
{
    public class UpdateMdAssignedItemViewModel : BaseViewModel
    {
        MachineDesignItemsApi machineApi => CdbApiFactory.Instance.machineApi; 

        public ObservableCollection<ItemDetailViewModel> InventoryItems { get; set; }
        private ItemDetailViewModel _SelectedInventoryItem;
        private ItemDomainMachineDesign mdItem;
        private Item CatalogItem; 

        public UpdateMdAssignedItemViewModel(ItemDomainMachineDesign mdItem)
        {
            Item assignedItem = mdItem.AssignedItem;             
            Title = "Update Assigned Item";
            this.mdItem = mdItem; 

            
            if (assignedItem != null)
            {
                LoadItemInventoryList(assignedItem); 
            }
        }

        private void LoadItemInventoryList(Item assignedItem)
        {            
            Boolean selectAssignedItemInventory = false;

            if (assignedItem.Domain.Name.Equals(Constants.inventoryDomainName))
            {
                selectAssignedItemInventory = true;
                CatalogItem = assignedItem.DerivedFromItem;
            }
            else if (assignedItem.Domain.Name.Equals(Constants.catalogDomainName))
            {
                CatalogItem = assignedItem;
            }

            if (CatalogItem != null)
            {
                InventoryItems = new ObservableCollection<ItemDetailViewModel>();

                List<Item> inventoryItems = itemApi.GetItemsDerivedFromItemByItemId(CatalogItem.Id);

                foreach (var item in inventoryItems)
                {
                    var itemDetail = new ItemDetailViewModel(item);

                    if (selectAssignedItemInventory)
                    {
                        if (item.Id == assignedItem.Id)
                        {
                            SelectedInventoryItem = itemDetail;
                            selectAssignedItemInventory = false;
                        }
                    }
                    InventoryItems.Add(itemDetail);
                }
            }
        }

        public async Task<ItemDomainMachineDesign> UpdateAssignedItem()
        {
            try
            {
                var mdItemId = mdItem.Id;
                var assignedItemId = SelectedInventoryItem.Item.Id;
                UpdateMachineAssignedItemInformation info = new UpdateMachineAssignedItemInformation
                {
                    AssignedItemId = assignedItemId,
                    MdItemId = mdItemId
                }; 
                return await machineApi.UpdateAssignedItemAsync(info); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }
        }

        public void UpdateScannedItem(int qrid)
        {             
            if (InventoryItems == null)
            {
                var newAssignedItem = itemApi.GetItemByQrId(qrid);                

                if (newAssignedItem != null)
                {
                    var newDomainName = newAssignedItem.Domain.Name;
                    if (newDomainName.Equals(Constants.catalogDomainName) ||
                        newDomainName.Equals(Constants.inventoryDomainName))
                    {
                        LoadItemInventoryList(newAssignedItem);
                        OnPropertyChanged(nameof(PromptText));
                        OnPropertyChanged(nameof(IsVisibleInventoryList));
                        OnPropertyChanged(nameof(InventoryItems));

                        // Trigger update of selection.
                        var temp = SelectedInventoryItem;
                        SelectedInventoryItem = null;
                        SelectedInventoryItem = temp;
                        return; 
                    } else
                    {
                        FireViewModelMessageEvent("Scanned Item must be catalog or inventory");
                        return;
                    }
                } else
                {
                    FireViewModelMessageEvent("Error loading qrid: " + qrid);
                    return;
                }

            }
            bool itemFound = false; 
            foreach (var itemDetail in InventoryItems)
            {
                var item = itemDetail.Item;
                if (item.QrId == qrid)
                {
                    SelectedInventoryItem = itemDetail;
                    itemFound = true; 
                    break; 
                }
            }

            if (itemFound)
            {
                FireViewModelMessageEvent("Selected item: " + SelectedInventoryItem.FormattedQrId);
            } else
            {
                FireViewModelMessageEvent("Item must be of catalog: " + CatalogItem.Name); 
            }

            
        }

        public String PromptText
        {
            get
            {
                if (IsVisibleInventoryList)
                {
                    return "Scan or Select Item To Proceed"; 
                }
                return "Scan Item to Proceed"; 
            }
        }

        public Boolean IsVisibleInventoryList
        {
            get
            {
                return InventoryItems != null; 
            }
        }

        public ItemDetailViewModel SelectedInventoryItem
        {
            get
            {
                return _SelectedInventoryItem;    
            }
            set
            {
                _SelectedInventoryItem = value; 
                OnPropertyChanged(); 
            }
        }
    }
}

