/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Component.DB.Services;
using Component.DB.Services.CdbEventArgs;
using Gov.ANL.APS.CDB.Api;

namespace Component.DB.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public ItemApi itemApi => CdbApiFactory.Instance.itemApi;
        public LocationItemsApi locationItemsApi => CdbApiFactory.Instance.locationItemApi; 

        public event EventHandler<ViewModelMessageEventArgs> ViewModelMessageEvent;

        protected CdbMobileAppStorage appStore = CdbMobileAppStorage.Instance;        

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected String ActiveUsername
        {
            get
            {
                var auth = appStore.getActiveAuth();
                if (auth != null)
                {
                    return auth.Username;
                }
                return null;                 
            }
        }

        protected String ActiveHostUrl
        {
            get
            {
                var config = appStore.getActiveConfiguration();
                return config.CdbAddress; 
            }
        }

        public String ConnectedToMessage
        {
            get
            {
                return "Connected to " + this.ActiveHostUrl; 
            }
        }

        public String SaveButtonText
        {
            get
            {
                var userName = this.ActiveUsername;
                if (userName != null)
                {
                    return "Save (as user: " + this.ActiveUsername + ")";
                }
                return "Save";                 
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void FireViewModelMessageEvent(String message)
        {
            if (ViewModelMessageEvent != null)
            {
                var args = new ViewModelMessageEventArgs(message);
                ViewModelMessageEvent(this, args); 
            }
         } 

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
