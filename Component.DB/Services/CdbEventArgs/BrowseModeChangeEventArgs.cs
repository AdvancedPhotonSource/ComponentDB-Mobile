using System;
using static Component.DB.Services.CdbMobileAppStorage;

namespace Component.DB.Services.CdbEventArgs
{
    public class BrowseModeChangeEventArgs : EventArgs
    {

        BrowseMode BrowseMode { get; }

        public BrowseModeChangeEventArgs(BrowseMode browseMode)
        {
            BrowseMode = browseMode;
        }

    }
}
