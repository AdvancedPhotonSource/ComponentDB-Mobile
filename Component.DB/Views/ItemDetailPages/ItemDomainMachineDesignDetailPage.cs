/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using Component.DB.Services;
using Component.DB.ViewModels;
using Component.DB.Views.itemEditPages;
using Gov.ANL.APS.CDB.Api;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views.ItemDetailPages
{
    public class ItemDomainMachineDesignDetailPage : ItemDetailPage
    {

        private StackLayout AssignedItemStackLayout; 

        private ItemDomainMachineDesignDetailPage(ItemDomainMachineDesignDetailViewModel viewModel) : base(viewModel, addIdentifiers: false)
        {
            var mdItem = viewModel.Item;             
            var domain = mdItem.Domain;

            addBindingToDetailsStackLayout("QR Id", "FormattedQrId", 0);
            addBindingToDetailsStackLayout(domain.ItemIdentifier1Label, "Item.ItemIdentifier1");

            AssignedItemStackLayout = new StackLayout
            {
                Spacing = 10,
                Padding = 10
            };

            this.MainStackLayout.Children.Add(AssignedItemStackLayout);

            Label AssignedItemSectionLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Text = "Assigned Item"
            };

            var updateAssignedItemBtn = new Button
            {
                Text = "Update Assigned Item",                
            };
            updateAssignedItemBtn.Clicked += HandleUpdateAssignedItemClicked; 

            // inventoryButton.Clicked += HandleShowInventoryClicked;
            AssignedItemStackLayout.Children.Add(AssignedItemSectionLabel);             

            addBindingToDetailsStackLayout("Name", "FormattedAssignedItemName", Stack: AssignedItemStackLayout);
            addBindingToDetailsStackLayout("QR Id", "FormattedAssignedItemQrId", Stack: AssignedItemStackLayout);

            AssignedItemStackLayout.Children.Add(updateAssignedItemBtn);
        }

        public static ItemDomainMachineDesignDetailPage CreateInstance(ItemDetailViewModel viewModel) 
        {
            MachineDesignItemsApi machineApi = CdbApiFactory.Instance.machineApi;

            var item = viewModel.Item;
            var mdItem = machineApi.GetMachineDesignItemById(item.Id);
            var newViewModel = new ItemDomainMachineDesignDetailViewModel(mdItem);

            return new ItemDomainMachineDesignDetailPage(newViewModel); 
        }

        public new ItemDomainMachineDesignDetailViewModel viewModel
        {
            get
            {
                return (ItemDomainMachineDesignDetailViewModel)base.viewModel; 
            }
            set
            {
                base.viewModel = value; 
            }
        }

        async void HandleUpdateAssignedItemClicked(object sender, System.EventArgs e)
        {
            if (await VerifyPrivilagesOrPropmtToLogInAsync(viewModel.Item))
            {
                UpdateMdAssignedItemPage updateAssignedItemPage = new UpdateMdAssignedItemPage(this.viewModel.Item, this);

                await Navigation.PushAsync(updateAssignedItemPage);
            }            
        }

        public void updateItem(ItemDomainMachineDesign item)
        {
            var newViewModel = new ItemDomainMachineDesignDetailViewModel(item);

            BindingContext = this.viewModel = newViewModel;  
        }
    }
}
