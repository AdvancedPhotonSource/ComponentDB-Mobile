/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using Xamarin.Forms;
using Component.DB.Services;
using Component.DB.Views;
using Component.DB.Views.PreferencePages;

namespace Component.DB
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            CdbMobileAppStorage store = CdbMobileAppStorage.Instance;
            var config = store.getActiveConfiguration();

            if (config != null)
            {
                this.loadDefaultMainPage();
            } else
            {
                MainPage = new InitialCdbConfigurationPage(this);
            }
        }

        public void loadDefaultMainPage()
        {
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
