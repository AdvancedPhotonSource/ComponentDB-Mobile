using System;
using System.Threading.Tasks;
using Gov.ANL.APS.CDB.Model;

namespace Component.DB.ViewModels
{
    public class AddLogEntryViewModel : BaseViewModel
    {
        public String LogEntry { get; set; }
        public DateTime EffectiveDate { get; set; }

        private int ItemId { get; }

        public AddLogEntryViewModel(int ItemId)
        {
            Title = "Add Log Entry";
            this.ItemId = ItemId; 

            LogEntry = "";
            EffectiveDate = DateTime.Today;
        }

        public async Task AddLogEntryForItemAsync()
        {
            var logEntryItem = new LogEntryEditInformation
            {
                ItemId = ItemId,
                EffectiveDate = EffectiveDate,
                LogEntry = LogEntry
            };

            await itemApi.AddLogEntryToItemAsync(logEntryItem); 
        }
    }
}
