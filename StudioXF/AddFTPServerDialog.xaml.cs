using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StudioXF
{
    public sealed partial class AddFTPServerDialog : ContentDialog
    {
        public AddFTPServerDialog()
        {
            this.InitializeComponent();
        }

        private void OK(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Add the address or name into the list.
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Object value = localSettings.Values["FTPAddresses"];

            if (value == null)
            {
                // No data
                localSettings.Values["FTPAddresses"] = FTPAddress.Text;
            }
            else
            {
                // Access data in value
                string FTPAddresses = value as string;
                FTPAddresses += "," + FTPAddress.Text;
                localSettings.Values["FTPAddresses"] = FTPAddresses;
            }            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }
    }
}
