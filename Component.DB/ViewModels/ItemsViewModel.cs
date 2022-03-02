/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Component.DB.Views;
using System.Collections.Generic;
using Gov.ANL.APS.CDB.Model;
using Gov.ANL.APS.CDB.Api;
using Gov.ANL.APS.CDB.Client;
using Component.DB.Services;

namespace Component.DB.ViewModels
{
    public abstract class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<ItemDetailViewModel> Items { get; set; }
        public ObservableCollection<ItemDetailViewModel> AllItems { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command FilterItemsCommand { get; }

        protected CdbMobileAppStorage AppStorage; 

        private Exception _LoadItemCommandException;
        private ItemsPage ItemsPage;

        private int parentItemId; 

        public ItemsViewModel(ItemsPage itemsPage, int parentItemId = -1)
        {
            Title = getTitle();
            Items = new ObservableCollection<ItemDetailViewModel>();
            AllItems = new ObservableCollection<ItemDetailViewModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            FilterItemsCommand = new Command((filterString) => Filter(filterString));
            ItemsPage = itemsPage;

            AppStorage = CdbMobileAppStorage.Instance; 

            this.parentItemId = parentItemId;

            //MessagingCenter.Subscribe<NewItemPage, Domain>(this, "AddItem", (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    Items.Add(newItem);
            //    //await DataStore.AddItemAsync(newItem);
            //});
        }

        public void ClearItems()
        {
            AllItems.Clear();
            Items.Clear(); 
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LoadItemCommandException = null;

            try
            {
                ClearItems();

                List<ConciseItem> items; 
                if (parentItemId == -1)
                {
                    items = await getItems();
                } else
                {
                    var derivedItems = await itemApi.GetItemsDerivedFromItemByItemIdAsync(parentItemId);
                    var opts = new ConciseItemOptions
                    {
                        IncludeDerivedFromItemInfo = true,
                        IncludePrimaryImageForItem = true
                    };

                    items = ConvertItemsToConciseItem(derivedItems, opts); 
                }
                               
                foreach (var itemObj in items)
                {
                    var item = new ItemDetailViewModel(null, itemObj);
                    Items.Add(item);
                    AllItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                // TODO convert to event 
                LoadItemCommandException = ex; 
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected List<ConciseItem> ConvertItemsToConciseItem(List<Item> items, ConciseItemOptions opts = null)
        {
            List<ConciseItem> conciseItems = new List<ConciseItem>();

            foreach (Item item in items)
            {
                ConciseItem newItem = new ConciseItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    ItemIdentifier1 = item.ItemIdentifier1,
                    ItemIdentifier2 = item.ItemIdentifier2,
                    PrimaryImageForItem = item.PrimaryImageForItem,
                    QrId = item.QrId,                    
                };

                if (opts != null)
                {
                    if (opts.IncludeDerivedFromItemInfo == true)
                    {
                        if (item.DerivedFromItem != null)
                        {
                            newItem.DerivedFromItemId = item.DerivedFromItem.Id;
                            newItem.DerivedFromItemName = item.DerivedFromItem.Name;
                        }
                    }

                    if (opts.IncludeItemCategoryIdList == true)
                    {
                        newItem.ItemCategoryIdList = new List<int?>();
                        foreach (ItemCategory category in item.ItemCategoryList)
                        {
                            newItem.ItemCategoryIdList.Add(category.Id);
                        }
                    }

                    if (opts.IncludeItemTypeIdList == true)
                    {
                        newItem.ItemTypeIdList = new List<int?>();
                        foreach (ItemType itemType in item.ItemTypeList)
                        {
                            newItem.ItemTypeIdList.Add(itemType.Id);
                        }
                    }

                    if (opts.IncludeItemProjectIdList == true)
                    {
                        newItem.ItemProjectIdList = new List<int?>();
                        foreach (ItemProject itemProject in item.ItemProjectList)
                        {
                            newItem.ItemProjectIdList.Add(itemProject.Id);
                        }
                    }
                }

                conciseItems.Add(newItem);
            }

            return conciseItems; 
        }

        public abstract string getTitle();
        public abstract Task<List<ConciseItem>> getItems();

        public void Filter(Object filterStringObj)
        {
            Items.Clear();

            string filterString = filterStringObj.ToString(); 
            filterString = filterString.ToLower(); 

            foreach (var itemViewModel in AllItems)
            {
                var item = itemViewModel.ConciseItem;
                if (item.Name.ToLower().Contains(filterString)) {
                    Items.Add(itemViewModel); 
                }
            }
        }

        // TODO change to event based
        public Exception LoadItemCommandException
        {
            get
            {
                return _LoadItemCommandException; 
            } set
            {
                _LoadItemCommandException = value;
                ItemsPage.onLoadItemsCommandExceptionChanged();
            }
        }
    }
}