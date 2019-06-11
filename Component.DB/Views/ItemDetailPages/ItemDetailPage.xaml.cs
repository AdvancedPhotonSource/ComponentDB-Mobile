/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.ComponentModel;

using Xamarin.Forms;

using Component.DB.ViewModels;
using Stormlion.PhotoBrowser;
using System.Collections.Generic;
using Gov.ANL.APS.CDB.Model;
using Component.DB.Services;
using Component.DB.Views.PreferencePages;
using Component.DB.Views.itemEditPages;
using System.IO;
using System.Text;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Diagnostics;

namespace Component.DB.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemDetailPage : CdbBasePage
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

            var domainName = item.Domain.Name;
            if (domainName.Equals(Constants.inventoryDomainName))
            {
                ItemNameLabel.Text = "Tag:";
                // Show catalog item information 
                // TODO use catalog item attriibute 
                addBindingToDetailsStackLayout("Catalog Item", "Item.DerivedFromItem.Name", 0);

                addBindingToDetailsStackLayout("QR Id", "FormattedQrId", 0);

                // Show status
                viewModel.LoadItemStatus();
                viewModel.LoadItemLocationInformation();

                addBindingToDetailsStackLayout("Location", "ItemLocationInformation.LocationString");
                addBindingToDetailsStackLayout("Status", "ItemStatusString"); 
            }
            else if (domainName.Equals(Constants.catalogDomainName))
            {
                var inventoryButton = new Button
                {
                    Text = "View Inventory",
                };

                inventoryButton.Clicked += HandleShowInventoryClicked; 


                ItemDetailsButtonStackLayout.Children.Insert(0, inventoryButton);
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
            List<Photo> photos;
            try
            {
                photos = await viewModel.GetPhotosForItem();
            } catch (Exception ex)
            {
                HandleException(ex);
                return;
            }


            if (photos != null)
            {
                PhotoBrowser photoBrowser = new PhotoBrowser();
                photoBrowser.Photos = photos;
                photoBrowser.Show();
            }
            else
            {
                var message = "An error occured when loading list of images.";
                Debug.WriteLine(message);
                await DisplayAlert("Error", message, "OK");
            }
        }

        async void HandleShowInventoryClicked(object sender, System.EventArgs e)
        {
            var itemId = viewModel.Item.Id;

            ItemsPage itemsPage = new ItemsPage(MenuItemType.BrowseInventory, (int)itemId);

            await Navigation.PushAsync(itemsPage); 
        }

        async void HandleShowPropertiesClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ItemPropertiesPage(viewModel.Item));
        }

        async void HandleShowLogsClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ItemLogsPage(viewModel.Item));
        }

        async void HandleUploadImageClicked(object sender, System.EventArgs e)
        {
            if (await VerifyPrivilagesOrPropmtToLogInAsync(viewModel.Item))
            {
                var config = new StoreCameraMediaOptions()
                {
                    DefaultCamera = CameraDevice.Rear
                };

                var photo = await CrossMedia.Current.TakePhotoAsync(config);

                if (photo != null)
                {
                    var s = photo.GetStream();
                    var data = CdbApiFactory.ConvertStreamDataToBase64(s);
                    var file = new FileUploadObject
                    {
                        FileName = "Mobile-Image.jpg",
                        Base64Binary = data
                    };

                    //var stream = new MemoryStream(bytes); 
                    var itemApi = CdbApiFactory.Instance.itemApi;
                    try
                    {
                        var itemId = viewModel.Item.Id;
                        await itemApi.UploadImageForItemAsync(itemId, file);
                        var newItem = itemApi.GetItemById(itemId);
                        updateItem(newItem);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                }
            }
        }

        async void HandleEditItemClicked(object sender, System.EventArgs e)
        {
            if (await VerifyPrivilagesOrPropmtToLogInAsync(viewModel.Item))
            {
                await Navigation.PushAsync(new EditItemDetailPage(viewModel.Item, this));
            }
        }

        public void updateItem(Item item)
        {
            var detailViewModel = new ItemDetailViewModel(item);
            detailViewModel.LoadItemStatus();
            detailViewModel.LoadItemLocationInformation();
            BindingContext = this.viewModel = detailViewModel;
        }
    }
}