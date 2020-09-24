/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Component.DB.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Launcher.OpenAsync(new Uri("https://xamarin.com/platform")));
            OpenLicenseCommand = new Command(() => Launcher.OpenAsync(new Uri("https://raw.githubusercontent.com/AdvancedPhotonSource/ComponentDB-Mobile/master/LICENSE")));
        }

        public ICommand OpenWebCommand { get; }
        public ICommand OpenLicenseCommand { get; }
    }
}