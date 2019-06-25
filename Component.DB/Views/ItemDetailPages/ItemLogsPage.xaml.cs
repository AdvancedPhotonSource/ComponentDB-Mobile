/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System; 
using System.ComponentModel;
using Component.DB.ViewModels;
using Component.DB.Views.itemEditPages;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views
{
    [DesignTimeVisible(true)]
    public partial class ItemLogsPage : CdbBasePage
    {
        ItemLogsViewModel viewModel;

        public ItemLogsPage(Item item)
        {
            InitializeComponent();

            BindingContext = this.viewModel = new ItemLogsViewModel(item);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.LoadLogsCommand.Execute(null);
        }

        async void HandleClickedAddLog(object sender, EventArgs e)
        {
            var item = viewModel.Item;
            if (await VerifyPrivilagesOrPropmtToLogInAsync(item)) {
                int itemId = (int)item.Id;
                var addLogPage = new AddLogEntryPage(itemId);
                await Navigation.PushAsync(addLogPage);
            }
        }
    }
}
