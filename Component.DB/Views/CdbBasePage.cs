using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Component.DB.Services;
using Component.DB.Views.PreferencePages;
using Gov.ANL.APS.CDB.Model;
using Xamarin.Forms;

namespace Component.DB.Views
{
    public abstract class CdbBasePage : ContentPage
    {

        /// <summary>
        /// Verifies the privilages or propmt to log in async.
        /// </summary>
        /// <returns>True when user has sufficient privilages, False when user either does not or needs to log in (Login page opens automatically).</returns>
        /// <param name="item">Item.</param>
        protected async Task<bool> VerifyPrivilagesOrPropmtToLogInAsync(Item item)
        {
            var factory = CdbApiFactory.Instance;
            var authenticated = await factory.verifyUserAuthenticated();
            if (authenticated == null || (bool)!authenticated)
            {
                await DisplayAlert("Login Needed", "Login is required for this feature", "OK");
                await Navigation.PushAsync(new LoginPage());
                return false;
            }
            else
            {
                var result = factory.itemApi.VerifyUserPermissionForItemAsync(item.Id);
                if (result != null && (bool)await result)
                {
                    return true; 
                }
                else
                {
                    await DisplayAlert("Error", "You don't have sufficient privilages to edit the item", "OK");
                    return false;
                }
            }
        }

        protected void HandleException(Exception ex)
        {            
            var exMessage = CdbApiFactory.ParseApiException(ex);
            Debug.WriteLine(exMessage);
            DisplayAlert(exMessage.SimpleName, exMessage.Message, "OK");

            ClearIsBusy();
        }

        protected virtual void ClearIsBusy()
        {
            //Override this function when needed.
        }
    }
}
