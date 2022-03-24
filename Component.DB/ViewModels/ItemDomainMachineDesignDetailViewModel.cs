using System;
using Gov.ANL.APS.CDB.Model;

namespace Component.DB.ViewModels
{
    public class ItemDomainMachineDesignDetailViewModel : ItemDetailViewModel
    {        
        public ItemDomainMachineDesignDetailViewModel(ItemDomainMachineDesign mdItem) : base(mdItem)
        {
            
        }

        public string FormattedAssignedItemName
        {
            get
            {
                var assignedItem = this.Item.AssignedItem;
                if (assignedItem != null)
                {
                    if (assignedItem.DomainId == Constants.inventoryDomainId)
                    {
                        var catItem = assignedItem.DerivedFromItem;
                        return catItem.Name + " - " + assignedItem.Name; 
                    }
                    return assignedItem.Name;
                }
                return ""; 
            }
        }

        public string FormattedAssignedItemQrId
        {
            get
            {
                var assignedItem = this.Item.AssignedItem;
                if (assignedItem != null)
                {
                    if (assignedItem.DomainId == Constants.catalogDomainId)
                    {
                        return "No inventory specified"; 
                    }

                    var qrId = this.Item.AssignedItem.QrId;
                    return formatQrId(qrId);
                }
                return ""; 
            }
        }

        public new ItemDomainMachineDesign Item
        {
            get
            {
                return (ItemDomainMachineDesign)_Item;
            }
            set
            {
                _Item = value; 
            }
        }


    }
}
