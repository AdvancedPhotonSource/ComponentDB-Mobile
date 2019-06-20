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
        public PropertyApi PropertyApi = CdbApiFactory.Instance.propertyApi;

        public event EventHandler<ViewModelMessageEventArgs> ViewModelMessageEvent; 

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
