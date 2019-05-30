/*
 * Copyright (c) UChicago Argonne, LLC. All rights reserved.
 * See LICENSE file.
 */
using System;
using System.Collections.Generic;
using Component.DB.Services;
using Xamarin.Forms;

namespace Component.DB.Views.PreferencePages
{
    public partial class LoginPage : ContentPage
    {

        public string username { get; set; }
        public string password { get; set; }

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public async void Login_Clicked(object sender, EventArgs e)
        {
            var apiFactory = CdbApiFactory.Instance;
            var result = false;  
            try
            {
                result = await apiFactory.authenticateUserAsync(username, password);
            } catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                return; 
            }


            if (result)
            {
                await Navigation.PopAsync();
            } else
            {
                await DisplayAlert("Login Failed", "The username or password entered is incorrect", "Try Again"); 
            }
        }
    }
}
