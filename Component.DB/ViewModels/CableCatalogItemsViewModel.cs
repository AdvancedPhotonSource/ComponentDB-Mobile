using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Component.DB.Views;
using Gov.ANL.APS.CDB.Model;

namespace Component.DB.ViewModels
{
    public class CableCatalogItemsViewModel : ItemsViewModel
    {
        public CableCatalogItemsViewModel(ItemsPage itemsPage, int parentItemId = -1) : base(itemsPage, parentItemId)
        {
        }

        public override async Task<List<ConciseItem>> getItems()
        {
            ConciseItemOptions opts = new ConciseItemOptions
            {
                IncludePrimaryImageForItem = true
            };
            return await itemApi.GetConciseItemsByDomainAsync(Constants.cableCatalogDomainName, opts);
        }

        public override string getTitle()
        {
            return "Browse Cable Catalog";
        }
    }
}
