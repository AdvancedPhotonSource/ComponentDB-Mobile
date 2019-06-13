using System;
using Gov.ANL.APS.CDB.Model;

namespace Component.DB.Services.CdbEventArgs
{
    public class LocationSelectedEventArgs : EventArgs
    {

        public ItemDomainLocation SelectedLocation { get; set; }
        public String LocationString { get; set; }

        public LocationSelectedEventArgs(ItemDomainLocation LocationSelection)
        {
            this.SelectedLocation = LocationSelection;

            LocationString = LocationSelection.Name; 
        }
    }
}
