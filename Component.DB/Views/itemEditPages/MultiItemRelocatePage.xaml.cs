using System;
using System.Collections.Generic;
using Component.DB.Services;
using Component.DB.Services.CdbEventArgs;
using Component.DB.Services.PlatformDependency;
using Component.DB.ViewModels;
using Component.DB.Views.PreferencePages;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views.itemEditPages
{
    public partial class MultiItemRelocatePage : CdbBasePage
    {

        MultiItemRelocateViewModel viewModel;
        INotificationPopup NotificationPopup;

        bool loginChecked;

        public MultiItemRelocatePage()
        {
            InitializeComponent();

            BindingContext = viewModel = new MultiItemRelocateViewModel();

            NotificationPopup = DependencyService.Get<INotificationPopup>();
            loginChecked = false;
        }

        public void addQrId(int qrId)
        {           
            try
            {
                var result = viewModel.addItemByQrId(qrId);
                Device.BeginInvokeOnMainThread(() =>
                {
                    NotificationPopup.shortPopup(result);
                });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        void HandleClearItemsClicked(object sender, System.EventArgs e)
        {
            viewModel.ClearItems();
        }

        async void HandleScanClicked(object sender, System.EventArgs e)
        {
            var scanPage = new QrScannerPage();
            await Navigation.PushAsync(scanPage);
        }

        void HandleSelectLocationManually(object sender, System.EventArgs e)
        {
            ItemLocationSelectionPage locationSelectionPage = new ItemLocationSelectionPage();
            locationSelectionPage.LocationSelected += LocationChangedEventHandler;

            Navigation.PushAsync(locationSelectionPage);
        }

        void LocationChangedEventHandler(Object sender, LocationSelectedEventArgs args)
        {
            var location = args.SelectedLocation;
            viewModel.SelectedLocation = location;
        }

        async void HandleRelocateItemsClicked(object sender, System.EventArgs e)
        {
            foreach (var item in viewModel.LocatableItemList)
            {
                try
                {
                    var locInfo = item.LoadItemLocationInformation();
                    ItemDomainLocation locationItem = null;
                    var selectedLocation = viewModel.SelectedLocation;

                    if (selectedLocation != null)
                    {
                        locationItem = new ItemDomainLocation { Id = selectedLocation.Id };
                    }

                    locInfo.LocationItem = locationItem;
                    locInfo.LocationDetails = viewModel.LocationDetails; 
                    await item.UpdateItemLocationAsync();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                    return;
                }
            }

            viewModel.ClearItems();
            viewModel.SelectedLocation = null;
            viewModel.LocationDetails = ""; 
            await DisplayAlert("Update Complete", "Locations have been updated", "OK");
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var factory = CdbApiFactory.Instance;
            var authenticated = await factory.verifyUserAuthenticated();
            if (authenticated == null || (bool)!authenticated)
            {
                var proceedToLogin = await DisplayAlert("Login Needed", "Proceed to login?", "Yes", "No");
                if (proceedToLogin)
                {
                    await Navigation.PushAsync(new LoginPage());
                } else
                {
                    var mainPage = Application.Current.MainPage as MainPage;
                    await mainPage.NavigateFromMenu((int)MenuItemType.BrowseCatalog); 
                }
            }
        }
    }
}