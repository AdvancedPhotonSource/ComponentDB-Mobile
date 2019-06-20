using System;
namespace Component.DB.Services.CdbEventArgs
{
    public class ViewModelMessageEventArgs
    {

        public String Message { get; }
        public ViewModelMessageEventArgs(String Message)
        {
            this.Message = Message; 
        }
    }
}
