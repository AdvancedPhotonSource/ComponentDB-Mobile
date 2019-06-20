using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Component.DB.Services;
using static Component.DB.Services.CdbMobileAppStorage;

namespace Component.DB.ViewModels
{
    public class ScanningActionItem
    {
        public ScanningAction ScanningAction { get; set; }
        public String DisplayText { get; set; }
    }

    public class BrowseModeItem
    {
        public BrowseMode BrowseMode  { get; set; }
        public String DisplayText { get; set; }
    }

    public class CdbConfigurationViewModel : BaseViewModel
    {
        private String _ActiveAuthUser;
        private Boolean _IsAuthenticated;

        public ObservableCollection<String> ActiveHosts { get; set; }
        public List<ScanningActionItem> ScanningActions { get; }
        private ScanningActionItem _ActiveScanningActionItem { get; set; }
        public List<BrowseModeItem> CatalogBrowseModes { get; }
        private BrowseModeItem _SelectedCatalogBrowseMode { get; set; }


        private String _ActiveHost;

        CdbMobileAppStorage AppStorage;

        public CdbConfigurationViewModel()
        {
            Title = "Configuration";
            ActiveHosts = new ObservableCollection<string>();
            ScanningActions = new List<ScanningActionItem> 
            { 
                new ScanningActionItem{ ScanningAction = ScanningAction.DetailsPage, DisplayText = "Show Item Details"},
                new ScanningActionItem{ ScanningAction = ScanningAction.RelocateItem, DisplayText = "Start Relocate Item"}
            };

            CatalogBrowseModes = new List<BrowseModeItem>
            {
                new BrowseModeItem {BrowseMode = BrowseMode.All, DisplayText = "Show All"} ,
                new BrowseModeItem {BrowseMode = BrowseMode.Favorites, DisplayText = "Show Favorites" }
            };
                
            AppStorage = CdbMobileAppStorage.Instance;

            var activeScanningAction = AppStorage.GetScanningAction();
            _ActiveScanningActionItem = ScanningActions[(int)activeScanningAction];
            // Browse modes is generic and can contain different options per domain. 
            var selectedCatalogBrowseMode = AppStorage.GetCatalogBrowseMode(); 
            foreach (var browseItem in CatalogBrowseModes)
            {
                if (browseItem.BrowseMode == selectedCatalogBrowseMode)
                {
                    _SelectedCatalogBrowseMode = browseItem;
                    break;
                }
            }

        }

        public BrowseModeItem SelectedCatalogBrowseMode
        {
            get
            {
                return _SelectedCatalogBrowseMode;
            } set
            {
                if (_SelectedCatalogBrowseMode.BrowseMode != value.BrowseMode)
                {
                    AppStorage.UpdateCatalogBrowseMode(value.BrowseMode);
                }
                _SelectedCatalogBrowseMode = value; 
                OnPropertyChanged();
            }
        }

        public ScanningActionItem ActiveScanningActionItem
        {
            get
            {
                return _ActiveScanningActionItem;
            }
            set
            {
                if (_ActiveScanningActionItem.ScanningAction != value.ScanningAction)
                {
                    AppStorage.UpdateScanningAction(value.ScanningAction);
                }
                _ActiveScanningActionItem = value;
                OnPropertyChanged();
            }
        }

        public String ActiveAuthUser
        {
            get
            {
                return _ActiveAuthUser;
            }
            set
            {
                _ActiveAuthUser = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsAuthenticated
        {
            get
            {
                return _IsAuthenticated;
            }
            set
            {
                _IsAuthenticated = value;
                // Generated property changed;
                IsNotAuthenticated = value;
                OnPropertyChanged();
            }
        }

        public String ActiveHost
        {
            get
            {
                return _ActiveHost;
            }
            set
            {
                _ActiveHost = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsNotAuthenticated
        {
            get
            {
                return !IsAuthenticated; 
            } 
            set
            {
                OnPropertyChanged();
            }
        }
    }
}
