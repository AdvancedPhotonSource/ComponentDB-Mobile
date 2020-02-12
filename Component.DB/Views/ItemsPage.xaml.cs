/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.ComponentModel;

using Xamarin.Forms;

using Component.DB.ViewModels;
using System.Collections.Generic;
using Gov.ANL.APS.CDB.Model;
using Component.DB.Services.CdbEventArgs;
using Component.DB.Services;
using Component.DB.Services.PlatformDependency;
using Gov.ANL.APS.CDB.Api;

namespace Component.DB.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemsPage : CdbBasePage
    {

        ItemApi itemApi = CdbApiFactory.Instance.itemApi;

        ItemsViewModel viewModel;
        INotificationPopup NotificationPopup;

        public ItemsPage()
        {
            SetupView(MenuItemType.BrowseCatalog);
        }

        public ItemsPage(MenuItemType itemType)
        {
            SetupView(itemType);
        }

        public ItemsPage(MenuItemType itemType, int parentItemId = -1)
        {
            SetupView(itemType, parentItemId);
        }

        private void SetupView(MenuItemType itemType, int parentItemId = -1)
        {
            InitializeComponent();
            NotificationPopup = DependencyService.Get<INotificationPopup>();

            Boolean isInventory = false; 
            if (itemType == MenuItemType.BrowseCatalog)
            {
                BindingContext = viewModel = new CatalogItemsViewModel(this, parentItemId);
                CdbMobileAppStorage.Instance.CatalogBrowseModeChangedEvent += OnBrowseModeChanged;
            }
            else
            {
                isInventory = true; 
                BindingContext = viewModel = new InventoryItemsViewModel(this, parentItemId);
            }

            viewModel.ViewModelMessageEvent += OnViewModelMessage; 

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

            // Load the latest version of the item.            
            var itemObj = itemApi.GetItemById(item.Item.Id);
            var newItem = new ItemDetailViewModel(itemObj);

            await Navigation.PushAsync(new ItemDetailPage(newItem));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }        

        private void OnBrowseModeChanged(object sender, BrowseModeChangeEventArgs args)
        {
            viewModel.ClearItems(); 
        }

        private void OnViewModelMessage(object sender, ViewModelMessageEventArgs args)
        {
            NotificationPopup.longPopup(args.Message); 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
            }
        }

        public void onLoadItemsCommandExceptionChanged()
        {
            if (viewModel.LoadItemCommandException != null)
            {
                HandleException(viewModel.LoadItemCommandException);
                viewModel.LoadItemCommandException = null;
            }
        }

        public void HandleSearchTextChanged(Object sender, TextChangedEventArgs args) 
        {
            viewModel.FilterItemsCommand.Execute(args.NewTextValue); 
        }
    }
}