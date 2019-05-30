/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System; 
using System.ComponentModel;
using Component.DB.ViewModels;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views
{
    [DesignTimeVisible(true)]
    public partial class ItemLogsPage : ContentPage
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
    }
}
