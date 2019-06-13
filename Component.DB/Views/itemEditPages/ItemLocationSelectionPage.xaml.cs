using System;
using System.Threading.Tasks;
using Component.DB.Services.CdbEventArgs; 
using Component.DB.ViewModels;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views.itemEditPages
{
    public partial class ItemLocationSelectionPage : ContentPage
    {
        ItemLocationSelectionViewModel viewModel;

        public event EventHandler<LocationSelectedEventArgs> LocationSelected; 

        public ItemLocationSelectionPage(ItemLocationSelectionViewModel viewModel = null)
        {
            InitializeComponent();

            if (viewModel == null)
            {
                viewModel = new ItemLocationSelectionViewModel();
            }

            BindingContext = this.viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.LocationItemList.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
            }
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as ItemDomainLocation;
            if (item == null)
                return;

            var newViewModel = new ItemLocationSelectionViewModel(item);
            var locationSelectionPage = new ItemLocationSelectionPage(newViewModel);

            locationSelectionPage.LocationSelected += LocationSelectedEventHandler; 

            await Navigation.PushAsync(locationSelectionPage);                
        }

        async void LocationSelectedEventHandler(object sender, LocationSelectedEventArgs args)
        {
            // Generate the basic hierarchy string
            var parentItem = viewModel.ParentItem; 
            if (parentItem != null)
            {
                var locStr = args.LocationString;
                locStr = parentItem.Name + "→" + locStr;
                args.LocationString = locStr;
            }

            await Navigation.PopAsync();

            LocationSelected(this, args);
        }

        async void HandleSelectCurrentLocationAsync(object sender, EventArgs e)
        {
        
            await Navigation.PopAsync();

            var selection = viewModel.ParentItem;
            var args = new LocationSelectedEventArgs(selection);

            LocationSelected(this, args); 
        }
    }
}
