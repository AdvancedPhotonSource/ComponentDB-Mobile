using System;
using System.Collections.Generic;
using Component.DB.Services;
using Component.DB.ViewModels;
using Gov.ANL.APS.CDB.Api;
using Gov.ANL.APS.CDB.Client;
using Xamarin.Forms;

namespace Component.DB.Views.PreferencePages
{
    public partial class CdbConfigurationPage : CdbBasePage
    {
        private CdbMobileAppStorage appStorage;
        private TestApi testApi;
        private CdbApiFactory apiFactory;

        private CdbConfigurationViewModel viewModel; 

        public CdbConfigurationPage()
        {
            InitializeComponent();

            appStorage = CdbMobileAppStorage.Instance;
            apiFactory = CdbApiFactory.Instance;
            testApi = apiFactory.testApi;

            viewModel = new CdbConfigurationViewModel();
            BindingContext = viewModel;
        }

        void PopulateData()
        {
            viewModel.ActiveHosts.Clear();

            viewModel.IsAuthenticated = false;
            viewModel.ActiveAuthUser = "";

            var result = appStorage.getActiveConfiguration();
            if (result != null)
            {
                var cdbAddress = result.CdbAddress;
                viewModel.ActiveHosts.Add(cdbAddress);
                viewModel.ActiveHost = cdbAddress;
            }

            var activeAuth = appStorage.getActiveAuth();

            if (activeAuth != null)
            {
                viewModel.ActiveAuthUser = activeAuth.Username;
                viewModel.IsAuthenticated = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            PopulateData();
        }

        void HandleTestConnectionClicked(object sender, System.EventArgs e)
        {
            try
            {
                testApi.VerifyConnection();
                DisplayAlert("Success", "Connection to server was established", "OK");
            } catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        void HandleTestAuthClicked(object sender, System.EventArgs e)
        {
            try
            {
                testApi.VerifyAuthenticated1();
                DisplayAlert("Success", "User Authentication is established", "OK");
            } catch (ApiException ex)
            {
                HandleException(ex);

                if (ex.ErrorCode == 401)
                {
                    // Clear Active Auth user
                    appStorage.clearActiveAuth();
                    PopulateData();
                }
            }
        }

        void HandleLoginClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new LoginPage());
        }

        void HandleLogoutClicked(object sender, System.EventArgs e)
        {
            apiFactory.LogoutActiveUser();

            PopulateData();
        }
    }
}
