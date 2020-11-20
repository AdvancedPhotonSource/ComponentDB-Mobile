/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using Component.DB.Services.CdbEventArgs;
using Component.DB.Services.PlatformDependency;
using Component.DB.ViewModels;
using Component.DB.Views.ItemDetailPages;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views.itemEditPages
{
    public partial class UpdateMdAssignedItemPage : CdbBasePage
    {
        UpdateMdAssignedItemViewModel viewModel;
        private ItemDomainMachineDesignDetailPage detailPage;
        INotificationPopup NotificationPopup;

        public UpdateMdAssignedItemPage(ItemDomainMachineDesign mdItem, ItemDomainMachineDesignDetailPage detailPage)
        {
            InitializeComponent();
            this.viewModel = new UpdateMdAssignedItemViewModel(mdItem);
            this.detailPage = detailPage;

            viewModel.ViewModelMessageEvent += HandleViewModelMessage;

            NotificationPopup = DependencyService.Get<INotificationPopup>();

            BindingContext = this.viewModel; 
        }

        public void UpdatedScannedItem(int qrId)
        {
            this.viewModel.UpdateScannedItem(qrId);            
        }

        async void HandleUpdateAssignedItemClicked(System.Object sender, System.EventArgs e)
        {
            if (viewModel.IsBusy)
            {
                return;
            }

            viewModel.IsBusy = true;

            try
            {
                var mdItem = await viewModel.UpdateAssignedItem();
                detailPage.updateItem(mdItem);
            } catch (Exception ex)
            {
                HandleException(ex);
                return; 
            }                        

            viewModel.IsBusy = false;

            await Navigation.PopAsync(); 
        }

        void HandleViewModelMessage(object sender, ViewModelMessageEventArgs args)
        {
            NotificationPopup.shortPopup(args.Message);
        }
    }
}
