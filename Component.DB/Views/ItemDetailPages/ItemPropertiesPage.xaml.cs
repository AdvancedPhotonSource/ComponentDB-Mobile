/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System; 
using System.ComponentModel;
using Component.DB.ViewModels;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Component.DB.Views
{
    [DesignTimeVisible(true)]
    public partial class ItemPropertiesPage : ContentPage
    {
        ItemPropertiesViewModel viewModel;

        public ItemPropertiesPage(Item item)
        {
            InitializeComponent();

            BindingContext = this.viewModel = new ItemPropertiesViewModel(item);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.LoadPropertiesCommand.Execute(null);
        }
    }
}
