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

    public class CdbConfigurationViewModel : BaseViewModel
    {
        private String _ActiveAuthUser;
        private Boolean _IsAuthenticated;

        public ObservableCollection<String> ActiveHosts { get; set; }
        public List<ScanningActionItem> ScanningActions { get; } 
        private ScanningActionItem _ActiveScanningActionItem { get; set; }
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

            AppStorage = CdbMobileAppStorage.Instance;

            var activeScanningAction = AppStorage.GetScanningAction();
            _ActiveScanningActionItem = ScanningActions[(int)activeScanningAction]; 
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
