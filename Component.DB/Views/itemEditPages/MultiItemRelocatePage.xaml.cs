using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public MultiItemRelocatePage()
        {
            InitializeComponent();

            BindingContext = viewModel = new MultiItemRelocateViewModel();
            viewModel.ViewModelMessageEvent += HandleViewModelMessage;

            NotificationPopup = DependencyService.Get<INotificationPopup>();
        }

        public void AddQrId(int qrId)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {                
                    await viewModel.AddItemByQrIdAsync(qrId);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            });
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

        void HandleViewModelMessage(object sender, ViewModelMessageEventArgs args)
        {
            NotificationPopup.shortPopup(args.Message);
        }

        async void HandleRelocateItemsClicked(object sender, System.EventArgs e)
        {
            var selectedLocation = viewModel.SelectedLocation; 
            foreach (var item in viewModel.LocatableItemList)
            {
                try
                {
                    if (item.Item.Domain.Name.Equals(Constants.locationDomainName))
                    {
                        item.UpdateLocationParent(selectedLocation); 
                    }
                    else
                    {
                        var locInfo = item.LoadItemLocationInformation();
                        ItemDomainLocation locationItem = null;

                        if (selectedLocation != null)
                        {
                            locationItem = new ItemDomainLocation { Id = selectedLocation.Id };
                        }

                        locInfo.LocationItem = locationItem;
                        locInfo.LocationDetails = viewModel.LocationDetails;
                        item.UpdateItemLocation();
                    }
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

        async void ItemsListView_ItemTapped(ListView sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            ItemDetailEditViewModel model = (ItemDetailEditViewModel) sender.SelectedItem;

            var REMOVE_OPT = "Remove";
            var SHOW_DETAILS_OPT = "View Details"; 
            string MAKE_PRIMARY_LOCATION = null;

            if (model.Item.Domain.Name.Equals(Constants.locationDomainName))
            {
                MAKE_PRIMARY_LOCATION = "Replace with Selected Location";
            }

            var prompt = "Options for " + model.ItemRelocateListingDisplayText;

            var action = await DisplayActionSheet(prompt, "Cancel", REMOVE_OPT, SHOW_DETAILS_OPT, MAKE_PRIMARY_LOCATION);     

            if (action == REMOVE_OPT || action == MAKE_PRIMARY_LOCATION)
            {
                viewModel.removeFromLocatableItemList(model);

                if (action == MAKE_PRIMARY_LOCATION)
                {
                    var selectedLocation = viewModel.SelectedLocation;
                    var selectedLocationModel = new ItemDetailEditViewModel(selectedLocation);
                    viewModel.addToLocatableItemList(selectedLocationModel);
                    viewModel.SelectedLocation = model.Item; 
                }
            } else if (action == SHOW_DETAILS_OPT)
            {
                var item = model.Item;
                var viewModel = new ItemDetailViewModel(item);
                var detailsPage = new ItemDetailPage(viewModel);

                await Navigation.PushAsync(detailsPage);
            }

            sender.SelectedItem = null; 
        }
    }
}