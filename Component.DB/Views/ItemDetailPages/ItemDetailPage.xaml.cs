/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Component.DB.ViewModels;
using Stormlion.PhotoBrowser;
using System.Collections.Generic;
using Gov.ANL.APS.CDB.Model;
using Component.DB.Services;
using Component.DB.Views.PreferencePages;
using Component.DB.Views.itemEditPages;

namespace Component.DB.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;

            Item item = viewModel.Item;
            var domain = item.Domain; 

            addBindingToDetailsStackLayout(domain.ItemIdentifier1Label, "Item.ItemIdentifier1");                   

            if (domain.ItemIdentifier2Label != null)
            {
                addBindingToDetailsStackLayout(domain.ItemIdentifier2Label, "Item.ItemIdentifier2");
            }

            if (item.Domain.Name.Equals(Constants.inventoryDomainName))
            {
                ItemNameLabel.Text = "Tag:";
                // Show catalog item information 
                // TODO use catalog item attriibute 
                addBindingToDetailsStackLayout("Catalog Item", "Item.DerivedFromItem.Name", 0);

                // Show status
                viewModel.loadItemStatus();
                addBindingToDetailsStackLayout("Status", "ItemStatusString"); 
            }
        }

        private void addBindingToDetailsStackLayout(String label, String bindingValue, int index = -1)
        {
            Label itemLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Text = label + ":"
            };
            Label itemValueLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };
            itemValueLabel.SetBinding(Label.TextProperty, bindingValue);

            if (index == -1)
            {
                DetailsStackLayout.Children.Add(itemLabel);
                DetailsStackLayout.Children.Add(itemValueLabel);
            } else
            {
                DetailsStackLayout.Children.Insert(index, itemValueLabel);
                DetailsStackLayout.Children.Insert(index, itemLabel);
            }


        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new Item
            {
                Name = "Item 1",
                Description = "This is an item description."
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }

        async void HandleItemImageTap(object sender, System.EventArgs e)
        {
            List<Photo> photos = await viewModel.GetPhotosForItem();

            if (photos != null)
            {
                PhotoBrowser photoBrowser = new PhotoBrowser();
                photoBrowser.Photos = photos;
                photoBrowser.Show();
            }
            else
            {
                // TODO: popup error
            }
        }

        async void HandleShowPropertiesClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ItemPropertiesPage(viewModel.Item));
        }

        async void HandleShowLogsClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ItemLogsPage(viewModel.Item));
        }

        async void HandleEditItemClicked(object sender, System.EventArgs e)
        {
            var factory = CdbApiFactory.Instance;
            var authenticated = await factory.verifyUserAuthenticated();
            if (authenticated == null || (bool)!authenticated)
            {
                await DisplayAlert("Login Needed", "Login is required for this feature", "OK");
                await Navigation.PushAsync(new LoginPage());
            } else
            {
                var result = factory.itemApi.VerifyUserPermissionForItemAsync(viewModel.Item.Id);
                if (result != null && (bool)await result)
                {
                    await Navigation.PushAsync(new EditItemDetailPage(viewModel.Item, this));
                }
                else
                {
                    await DisplayAlert("Error", "You don't have sufficient privilages to edit the item", result.ToString());
                }

            }
        }

        public void updateItem(Item item)
        {
            var detailViewModel = new ItemDetailViewModel(item);
            detailViewModel.loadItemStatus();
            BindingContext = this.viewModel = detailViewModel;
        }
    }
}