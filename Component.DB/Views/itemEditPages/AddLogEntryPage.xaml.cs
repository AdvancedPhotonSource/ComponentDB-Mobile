using System;
using System.Threading.Tasks;
using Component.DB.ViewModels;

namespace Component.DB.Views.itemEditPages
{
    public partial class AddLogEntryPage : CdbBasePage
    {
        AddLogEntryViewModel viewModel;

        public AddLogEntryPage(int ItemId)
        {
            InitializeComponent();

            BindingContext = viewModel = new AddLogEntryViewModel(ItemId); 
        }

        async void HandleSaveClickedAsync(object sender, EventArgs e)
        {
            try
            {
                await viewModel.AddLogEntryForItemAsync();
                await Navigation.PopAsync();
            } catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
