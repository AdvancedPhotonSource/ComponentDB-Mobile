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
using Component.DB.Services.CdbEventArgs;
using Component.DB.Views.ItemDetailPages;

namespace Component.DB.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemDetailPage : CdbBasePage
    {
        protected ItemDetailViewModel viewModel;

        private String CurrentDomainName = null;

        protected ItemDetailPage(ItemDetailViewModel viewModel, Boolean addIdentifiers = true)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;

            if (addIdentifiers)
            {
                CurrentDomainName = viewModel.Item.Domain.Name;

                Item item = viewModel.Item;
                var domain = item.Domain;

                addBindingToDetailsStackLayout(domain.ItemIdentifier1Label, "Item.ItemIdentifier1");

                if (domain.ItemIdentifier2Label != null)
                {
                    addBindingToDetailsStackLayout(domain.ItemIdentifier2Label, "Item.ItemIdentifier2");
                }
            }
        }

        public static ItemDetailPage CreateItemDetailPage(ItemDetailViewModel viewModel)
        {
            var CurrentDomainName = viewModel.Item.Domain.Name;

            if (CurrentDomainName.Equals(Constants.catalogDomainName))
            {
                return new ItemDomainCatalogDetailPage(viewModel);
            } else if (CurrentDomainName.Equals(Constants.inventoryDomainName))
            {
                return new ItemDomainInventoryDetailPage(viewModel);
            } else if (CurrentDomainName.Equals(Constants.machineDesignDomainName))
            {
                return ItemDomainMachineDesignDetailPage.CreateInstance(viewModel); 
            }

            return new ItemDetailPage(viewModel); 
        }

        protected StackLayout ButtonsStackLayout
        {
            get
            {
                return ItemDetailsButtonStackLayout; 
            }
        }

        protected StackLayout DetailsStackLayout
        {
            get
            {
                return _DetailsStackLayout; 
            }
        }

        protected StackLayout MainStackLayout
        {
            get
            {
                return _MainStackLayout; 
            }
        }

        protected Label ItemNameLabel
        {
            get
            {
                return _ItemNameLabel; 
            }
        }



        protected void addBindingToDetailsStackLayout(String label, String bindingValue, int index = -1, StackLayout Stack = null)
        {
            if (Stack == null)
            {
                Stack = DetailsStackLayout; 
            }
            Label itemLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Text = label + ":"
            };
            Label itemValueLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };
            itemValueLabel.SetBinding(Label.TextProperty, bindingValue);

            if (index == -1)
            {
                Stack.Children.Add(itemLabel);
                Stack.Children.Add(itemValueLabel);
            } else
            {
                Stack.Children.Insert(index, itemValueLabel);
                Stack.Children.Insert(index, itemLabel);
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

                MediaFile photo = null;
                try
                {
                    photo = await CrossMedia.Current.TakePhotoAsync(config);
                } catch(Exception ex)
                {
                    HandleException(ex); 
                }
                

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