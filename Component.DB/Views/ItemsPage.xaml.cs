﻿/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.ComponentModel;

using Xamarin.Forms;

using Component.DB.ViewModels;

namespace Component.DB.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemsPage : ContentPage
    {       

        ItemsViewModel viewModel;

        public ItemsPage()
        {
            setupView(MenuItemType.BrowseCatalog);
        }

        public ItemsPage(MenuItemType itemType)
        {
            setupView(itemType);
        }

        private void setupView(MenuItemType itemType)
        {
            InitializeComponent();


            Boolean isInventory = false; 
            if (itemType == MenuItemType.BrowseCatalog)
            {
                BindingContext = viewModel = new CatalogItemsViewModel();
            }
            else
            {
                isInventory = true; 
                BindingContext = viewModel = new InventoryItemsViewModel();
            }


            // Set up list view template
            var itemDataTemplate = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout();
                stackLayout.Padding = 5; 
                var grid = new Grid();
                stackLayout.Children.Add(grid);

                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var image = new Image();
                image.SetBinding(Image.SourceProperty, "PrimaryImageUrlScaled"); 


                var nameLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))};
                var itemIdentifier1Label = new Label { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };

                nameLabel.SetBinding(Label.TextProperty, "Item.Name");
                itemIdentifier1Label.SetBinding(Label.TextProperty, "Item.ItemIdentifier1");

                grid.Children.Add(image, 0, 0);
                Grid.SetRowSpan(image, 2);

                grid.Children.Add(nameLabel,1, 0);
                grid.Children.Add(itemIdentifier1Label, 1, 1);

                if (isInventory)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });

                    var catalogLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label))};
                    //TODO update the yaml inheritance when done to use the Catalog item. 
                    //catalogLabel.SetBinding(Label.TextProperty, "Item.CatalogItem.Name");
                    catalogLabel.SetBinding(Label.TextProperty, "Item.DerivedFromItem.Name");
                    grid.Children.Add(catalogLabel, 0, 2);
                    Grid.SetColumnSpan(catalogLabel, 2); 
                }
                     

                return new ViewCell { View = stackLayout }; 
            });

            ItemsListView.ItemTemplate = itemDataTemplate; 


        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as ItemDetailViewModel;
            if (item == null)
                return;               

            await Navigation.PushAsync(new ItemDetailPage(item));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            Label CatalogLabel = new Label();
            CatalogLabel.Text = "Catalog: ";
            Console.Write(ItemsListView.Id);

        }

        public void HandleSearchTextChanged(Object sender, TextChangedEventArgs args) 
        {
            viewModel.FilterItemsCommand.Execute(args.NewTextValue); 
        }
    }
}