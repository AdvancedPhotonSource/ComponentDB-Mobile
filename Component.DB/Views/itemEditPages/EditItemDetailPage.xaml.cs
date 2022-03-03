/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Component.DB.Services;
using Component.DB.Services.CdbEventArgs;
using Component.DB.ViewModels;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views.itemEditPages
{
    [System.ComponentModel.DesignTimeVisible(true)]
    public partial class EditItemDetailPage : CdbBasePage
    {
        ItemDetailEditViewModel viewModel;
        ItemDetailPage detailPage;

        Picker statusPicker;

        public EditItemDetailPage(Item item, ItemDetailPage detailPage)
        {
            InitializeComponent();

            this.viewModel = new ItemDetailEditViewModel(item);
            this.detailPage = detailPage;
            BindingContext = this.viewModel;

            var domain = item.Domain;

            addEditBindingToEditItemsStackLayout(domain.ItemIdentifier1Label, "Item.ItemIdentifier1");

            if (domain.ItemIdentifier2Label != null && !domain.Name.Equals(Constants.machineDesignDomainName))
            {
                addEditBindingToEditItemsStackLayout(domain.ItemIdentifier2Label, "Item.ItemIdentifier2");
            }

            if (item.Domain.Name.Equals(Constants.inventoryDomainName) || item.Domain.Name.Equals(Constants.cableInventoryDomainName))
            {
                viewModel.LoadItemStatus();
                viewModel.LoadItemLocationInformation();

                addEditBindingToEditItemsStackLayout("QR Id", "QrIdEntry", null, Keyboard.Numeric);

                var propertyApi = CdbApiFactory.Instance.propertyTypeApi;
                PropertyType type = null;
                if (item.Domain.Name.Equals(Constants.inventoryDomainName))
                {
                    type = propertyApi.GetInventoryStatusPropertyType();
                } else if (item.Domain.Name.Equals(Constants.cableInventoryDomainName))
                {
                    type = propertyApi.GetCableInventoryStatusPropertyType(); 
                }
                
                if (type != null)
                {
                    statusPicker = new Picker
                    {
                        Title = "Item Status",
                    };
                    foreach (var allowedValue in type.SortedAllowedPropertyValueList)
                    {
                        statusPicker.Items.Add(allowedValue.Value);
                    }

                    var statusString = viewModel.ItemStatusString;
                    if (statusPicker.Items.Contains(statusString))
                    {
                        statusPicker.SelectedItem = statusString;
                    }

                    addEditBindingToEditItemsStackLayout("Status", null, statusPicker);
                }                

                // Add Location 

                Label itemValueLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };
                itemValueLabel.SetBinding(Label.TextProperty, "ItemLocationInformation.LocationString");
                var buttonChangeLocation = new Button { Text = "Change Location" };
                var buttonClearLocation = new Button { Text = "Clear Location" };
                buttonChangeLocation.Clicked += ChangeLocationEventHandler;
                buttonClearLocation.Clicked += ClearLocationEventHandler;

                addEditBindingToEditItemsStackLayout("Location", null, itemValueLabel);
                EditItemsStackLayout.Children.Add(buttonChangeLocation);
                EditItemsStackLayout.Children.Add(buttonClearLocation);
                addEditBindingToEditItemsStackLayout("Location Details ", "ItemLocationInformation.LocationDetails");
            }
        }

        private void addEditBindingToEditItemsStackLayout(string label, String binding = null, View editObject = null, Keyboard keyboard = null)
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.Default;
            }
            if (binding == null && editObject == null)
            {
                throw new Exception("A Binding or edit object must be specified.");
            }

            Label nameLabel = new Label
            {
                Text = label,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            if (editObject == null)
            {
                editObject = new Entry
                {
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Entry)),
                    Keyboard = keyboard
                };
                editObject.SetBinding(Entry.TextProperty, binding);
            }

            EditItemsStackLayout.Children.Add(nameLabel);
            EditItemsStackLayout.Children.Add(editObject);
        }

        void ChangeLocationEventHandler(Object sender, EventArgs args)
        {

            ItemLocationSelectionPage page = new ItemLocationSelectionPage();
            page.LocationSelected += LocationChangedEventHandler;
            Navigation.PushAsync(page);
        }

        void ClearLocationEventHandler(Object sender, EventArgs args)
        {
            updateLocationInfo(null, "");
        }

        void LocationChangedEventHandler(Object sender, LocationSelectedEventArgs args)
        {
            updateLocationInfo(args.SelectedLocation, args.LocationString);
        }

        void updateLocationInfo(ItemDomainLocation location, String locationString)
        {
            var locationInfo = viewModel.ItemLocationInformation;
            locationInfo.LocationItem = location;
            locationInfo.LocationString = locationString;

            //Fire the value changed event. 
            viewModel.ItemLocationInformation = locationInfo;
        }

        async void HandleSaveClicked(object sender, System.EventArgs e)
        {
            if (viewModel.IsBusy)
            {
                return;
            }

            viewModel.IsBusy = true;

            Item resultItem = null;
            try
            {
                resultItem = await viewModel.UpdateItem();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return;
            }

            var item = viewModel.Item;
            if (item.Domain.Name.Equals(Constants.inventoryDomainName) || item.Domain.Name.Equals(Constants.cableInventoryDomainName))
            {
                //Check if status needs updating
                var selectedItem = statusPicker.SelectedItem;
                if (selectedItem != null)
                {
                    if (!viewModel.ItemStatusString.Equals(selectedItem))
                    {
                        // Need to update. 
                        try
                        {
                            await viewModel.UpdateItemStatusAsync(selectedItem.ToString());
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                            return;
                        }
                    }
                }

                try
                {
                    viewModel.UpdateItemLocation();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                    return;
                }
            }            

            // Pop My page
            detailPage.updateItem(resultItem);

            ClearIsBusy();

            await Navigation.PopAsync();
        }

        protected override void ClearIsBusy()
        {            
            viewModel.IsBusy = false;
        }
    }
}
