﻿/*
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

namespace Component.DB.ViewModels
{
    public abstract class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<ItemDetailViewModel> Items { get; set; }
        public ObservableCollection<ItemDetailViewModel> AllItems { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command FilterItemsCommand { get; }

        public ItemsViewModel()
        {
            Title = getTitle();
            Items = new ObservableCollection<ItemDetailViewModel>();
            AllItems = new ObservableCollection<ItemDetailViewModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            FilterItemsCommand = new Command((filterString) => Filter(filterString));

            //MessagingCenter.Subscribe<NewItemPage, Domain>(this, "AddItem", (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    Items.Add(newItem);
            //    //await DataStore.AddItemAsync(newItem);
            //});
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                AllItems.Clear();
                var items = await getItems(); 
                               
                foreach (var itemObj in items)
                {
                    var item = new ItemDetailViewModel(itemObj);
                    Items.Add(item);
                    AllItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public abstract string getTitle();
        public abstract Task<List<Item>> getItems();

        public void Filter(Object filterStringObj)
        {
            Items.Clear();

            string filterString = filterStringObj.ToString(); 
            filterString = filterString.ToLower(); 

            foreach (var itemViewModel in AllItems)
            {
                var item = itemViewModel.Item;
                if (item.Name.ToLower().Contains(filterString)) {
                    Items.Add(itemViewModel); 
                }
            }
        }
    }
}