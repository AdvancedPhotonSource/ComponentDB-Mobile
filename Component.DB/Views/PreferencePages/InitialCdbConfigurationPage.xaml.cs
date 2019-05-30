/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Component.DB.Services;
using Xamarin.Forms;

namespace Component.DB.Views.PreferencePages
{
    [System.ComponentModel.DesignTimeVisible(true)]
    public partial class InitialCdbConfigurationPage : ContentPage
    {

        public string cdbHostConfigured { get; set; }

        private CdbMobileAppStorage mobileAppStorage; 
        private App mainApp; 

        public InitialCdbConfigurationPage(App mainApp)
        {
            this.mainApp = mainApp;

            InitializeComponent();

            mobileAppStorage = CdbMobileAppStorage.Instance;

            cdbHostConfigured = mobileAppStorage.CdbHostAddress;

            BindingContext = this;
        }

        async void Save_ClickedAsync(object sender, EventArgs e)
        {
            if (await CdbApiFactory.verifyConnectionAsync(cdbHostConfigured))
            {
                mobileAppStorage.addConnection(cdbHostConfigured);
                mainApp.loadDefaultMainPage();
            } else
            {
                await DisplayAlert("Error Verifying Connection", "Could not connect to server address provided.", "Try Again");
            }
        }
    }
}
