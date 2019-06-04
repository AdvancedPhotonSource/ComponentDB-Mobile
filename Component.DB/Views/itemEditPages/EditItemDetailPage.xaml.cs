﻿/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Component.DB.Services;
using Component.DB.ViewModels;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views.itemEditPages
{
    [System.ComponentModel.DesignTimeVisible(true)]
    public partial class EditItemDetailPage : ContentPage
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

            if (domain.ItemIdentifier2Label != null)
            {
                addEditBindingToEditItemsStackLayout(domain.ItemIdentifier2Label, "Item.ItemIdentifier2");
            }

            if (item.Domain.Name.Equals(Constants.inventoryDomainName)) 
            {            
                var propertyApi = CdbApiFactory.Instance.propertyApi;
                var type = propertyApi.GetInventoryStatusPropertyType();
                viewModel.loadItemStatus();  

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

        }

        private void addEditBindingToEditItemsStackLayout(string label, String binding = null, View editObject = null)
        {
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
                editObject = new Entry { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Entry)) };
                editObject.SetBinding(Entry.TextProperty, binding); 
            }

            EditItemsStackLayout.Children.Add(nameLabel);
            EditItemsStackLayout.Children.Add(editObject);
        }

        async void HandleSaveClicked(object sender, System.EventArgs e)
        {
            Item resultItem = null;
            try {
                resultItem = await viewModel.UpdateItem();
            } catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                return;
            }

            var item = viewModel.Item;
            if (item.Domain.Name.Equals(Constants.inventoryDomainName))
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
                            await DisplayAlert("Error", ex.Message, "OK");
                            return; 
                        }
                    }
                }
            }

            // Pop My page
            detailPage.updateItem(resultItem);
            await Navigation.PopAsync();
        }
    }
}