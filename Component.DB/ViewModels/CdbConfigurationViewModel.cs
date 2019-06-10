using System;
using System.Collections.ObjectModel;

namespace Component.DB.ViewModels
{
    public class CdbConfigurationViewModel : BaseViewModel
    {

        private String _ActiveAuthUser;
        private Boolean _IsAuthenticated;

        public ObservableCollection<String> ActiveHosts { get; set; }
        private String _ActiveHost;

        public CdbConfigurationViewModel()
        {
            Title = "Configuration";
            ActiveHosts = new ObservableCollection<string>();
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
