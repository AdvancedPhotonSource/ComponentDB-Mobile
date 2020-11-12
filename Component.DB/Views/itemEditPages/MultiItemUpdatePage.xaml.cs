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
    public partial class MultiItemUpdatePage : CdbBasePage
    {

        MultiItemUpdateViewModel viewModel;
        INotificationPopup NotificationPopup;        

        public MultiItemUpdatePage()
        {
            InitializeComponent();

            BindingContext = viewModel = new MultiItemUpdateViewModel();
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

        async void HandleUpdateItemsClicked(object sender, System.EventArgs e)
        {
            var selectedLocation = viewModel.SelectedLocation;
            var locationMode = viewModel.LocationMode;
            var logMode = viewModel.LogMode;
            var statusMode = viewModel.StatusMode;

            if (logMode)
            {
                if (String.IsNullOrEmpty(viewModel.LogEntry))
                {
                    DisplayMissingInputAlert("Log Entry"); 
                    return; 
                }
            } else if (statusMode) {
                if (String.IsNullOrEmpty(viewModel.StatusEntry))
                {
                    DisplayMissingInputAlert("Status");
                    return;
                }
            }

            foreach (var item in viewModel.UpdatableItemList)
            {
                try
                {
                    if (locationMode)
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
                    } else if (logMode)
                    {
                        int itemId = (int)item.Item.Id;
                        AddLogEntryViewModel logEntryModel = new AddLogEntryViewModel(itemId)
                        {
                            LogEntry = viewModel.LogEntry 
                        };
                        await logEntryModel.AddLogEntryForItemAsync(); 
                    } else if (statusMode)
                    {
                        await item.UpdateItemStatusAsync(viewModel.StatusEntry); 
                    }
                    
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                    return;
                }
            }
            
            viewModel.SelectedLocation = null;
            viewModel.LocationDetails = "";
            viewModel.LogEntry = "";
            viewModel.StatusEntry = ""; 

            await DisplayAlert("Update Complete", "Items have been updated", "OK");
        }

        private async void DisplayMissingInputAlert(String inputName)
        {
            var message = "Please fill out " + inputName + " and try again.";
            await DisplayAlert("Missing Input", message, "OK"); 
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

            var prompt = "Options for " + model.MultiItemUpdateListingDisplayText;

            var action = await DisplayActionSheet(prompt, "Cancel", REMOVE_OPT, SHOW_DETAILS_OPT, MAKE_PRIMARY_LOCATION);     

            if (action == REMOVE_OPT || action == MAKE_PRIMARY_LOCATION)
            {
                viewModel.removeFromUpdatableItemList(model);

                if (action == MAKE_PRIMARY_LOCATION)
                {
                    var selectedLocation = viewModel.SelectedLocation;
                    if (selectedLocation != null)
                    {
                        var selectedLocationModel = new ItemDetailEditViewModel(selectedLocation);
                        viewModel.addToUpdatableItemList(selectedLocationModel);
                    }
                    
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

        async void modePicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            // modePicker selected to status
            if (viewModel.StatusMode)
            {
                if (statusPicker.Items.Count == 0)
                {
                    // Prepopulate all valid status values for the drop down. 
                    var propertyApi = CdbApiFactory.Instance.propertyTypeApi;
                    var type = propertyApi.GetInventoryStatusPropertyType();

                    foreach (var allowedValue in type.SortedAllowedPropertyValueList)
                    {
                        statusPicker.Items.Add(allowedValue.Value);
                    }
                }

                // Remove location items from the list
                var ItemsToRemove = new List<ItemDetailEditViewModel>();
                foreach (var Item in viewModel.UpdatableItemList)
                {
                    var dbItem = Item.Item;
                    if (dbItem.Domain.Name.Equals(Constants.locationDomainName))
                    {
                        ItemsToRemove.Add(Item);
                    }
                }

                var countLocations = ItemsToRemove.Count;

                if (countLocations > 0)
                {
                    await DisplayAlert("Status Changed",
                        countLocations + " Location(s) removed. Status is only for inventory items.",
                        "OK");

                    foreach (var ItemToRemove in ItemsToRemove)
                    {
                        viewModel.removeFromUpdatableItemList(ItemToRemove); 
                    }
                }
            }
            else if (viewModel.LocationMode)
            {
                // Verify if the selected location is not in locatable list.
                if (viewModel.SelectedLocation != null)
                {
                    var dbSelectedLocationId = viewModel.SelectedLocation.Id;
                    foreach (var Item in viewModel.UpdatableItemList)
                    {
                        var dbItem = Item.Item;
                        if (dbItem.Id == dbSelectedLocationId)
                        {
                            viewModel.removeFromUpdatableItemList(Item);
                            break;
                        }
                    }
                }
            }
        }
    }
}