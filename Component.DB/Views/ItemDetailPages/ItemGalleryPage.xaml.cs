/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using Component.DB.ViewModels;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views
{
    public partial class ItemGalleryPage : ContentPage
    {

        ItemGalleryViewModel viewModel; 

        public ItemGalleryPage(Item item)
        {
            InitializeComponent();

            BindingContext = this.viewModel = new ItemGalleryViewModel(item); 
        }
    }
}
