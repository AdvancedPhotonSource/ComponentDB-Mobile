/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.ViewModels
{
    public class ItemLogsViewModel : BaseViewModel
    {

        public Item Item { get; set; }
        public Command LoadLogsCommand { get; }
        public ObservableCollection<Log> LogList { get; }

        public ItemLogsViewModel(Item item = null)
        {
            Title = "Logbook: " + item.Name;
            Item = item;

            LogList = new ObservableCollection<Log>();

            LoadLogsCommand = new Command(async () => await ExceuteLoadLogList());
        }

        async Task ExceuteLoadLogList()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LogList.Clear();

            try
            {
                List<Log> logs = await itemApi.GetLogsForItemAsync(Item.Id);

                foreach (var log in logs)
                {
                    LogList.Add(log);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            IsBusy = false;
        }
    }
}
