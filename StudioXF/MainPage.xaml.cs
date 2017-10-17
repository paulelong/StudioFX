using Cubisoft.Winrt.Ftp;
using Cubisoft.Winrt.Ftp.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StudioXF
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Private
        private string currentServerPath = "/";

        private StorageFolder currentFolder;
        private string FTPServerName;
        //private string currentLocalPath = @"C:\";

        private FtpClient ftpClient = new FtpClient();
        #endregion Private

        const int TransferSize = 0x8000;

        #region Init
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Init()
        {
            currentFolder = await PickDefaultStartingLocation();

            if (currentFolder == null)
            {
                var dialog = new MessageDialog("Can't continue wihtout selected directory, exiting...");
                await dialog.ShowAsync();

                App.Current.Exit();
            }


            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Object LocalSettingsPath = localSettings.Values["LocalPath"];
            string LocalPathStr = "";

            if (LocalSettingsPath == null)
            {
                // No data
                LocalPathStr = currentFolder.Path;
            }
            else
            {
                try
                {
                    // Access data in value
                    LocalPathStr = LocalSettingsPath as string;
                    currentFolder = await StorageFolder.GetFolderFromPathAsync(LocalPathStr);
                }
                catch(Exception ex)
                {
                    var dialog = new MessageDialog("Can't access folder " + LocalPathStr + ". Error: " + ex.Message + ".  Using " + currentFolder.Path + " instead.");
                    await dialog.ShowAsync();
                }
            }

            Object value = localSettings.Values["FTPServerName"];
            if (value == null)
            {

            }
            else
            {
                FTPServerName = value as string;
            }

            AddFTPAddresses();

            int FTPIndex = FTPServer.Items.IndexOf(FTPServerName);
            if (FTPIndex >= 0)
            {
                FTPServer.SelectedIndex = FTPIndex;
                //UpdateServerFileList();
                //ServerPath.Text = currentServerPath;

                //await ftpClient.SetWorkingDirectoryAsync(ServerPath.Text);
            }
            LocalPath.Text = LocalPathStr;
//            UpdateLocalFileList();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            Init();
        }

        private void AddFTPAddresses()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Object value = localSettings.Values["FTPAddresses"];

            if (value != null)
            {
                FTPServer.Items.Clear();

                // Access data in value
                string FTPAddresses = value as string;
                string[] addresses = FTPAddresses.Split(',');

                foreach (string s in addresses)
                {
                    FTPServer.Items.Add(s);
                }
            }
        }

        private async Task<StorageFolder> PickDefaultStartingLocation()
        {

            if (StorageApplicationPermissions.FutureAccessList.Entries.Count <= 0)
            //if (rootPage.EnsureUnsnapped())
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
                folderPicker.FileTypeFilter.Add(".wav");

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();

                if (folder.Path != null)
                {
                    // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                    LocalPath.Text = folder.Path;

                }
                else
                {
                    //        OutputTextBlock.Text = "Operation cancelled.";
                    return null;
                }

                return (folder);
            }
            else
            {
                StorageFolder f = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
                return f;
            }
        }

        #endregion Init

        #region ServerUICommands
        private async void FTPServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BusyIndicator.IsActive = true;

            System.Diagnostics.Debug.WriteLine("FTP Server Changed");

            if (ftpClient.IsConnected)
            {
                await ftpClient.DisconnectAsync();
            }

            if(FTPServer.SelectedIndex >= 0)
            {
                string FTPServerName = FTPServer.Items[FTPServer.SelectedIndex].ToString();

                ftpClient.HostName = new HostName(FTPServerName);
                ftpClient.Credentials = new NetworkCredential("anonymous", "user");
                ftpClient.ServiceName = "21";

                try
                {
                    await ftpClient.ConnectAsync();
                }
                catch(Exception ex)
                {
                    var dialog = new MessageDialog(ex.Message);
                    await dialog.ShowAsync();
                }

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                // Create a simple setting

                localSettings.Values["FTPServerName"] = FTPServerName;

                // Is there a default path assocaited?
                object value = localSettings.Values[FTPServerName + "_DefaultDir"];
                if (value == null)
                {
                    currentServerPath = "/";
                }
                else
                {
                    currentServerPath = value as string;
                }
            }

            BusyIndicator.IsActive = false;

            ServerPath.Text = currentServerPath;
        }

        public async Task<List<FtpItem>> GetListing(FtpClient ftpClient)
        {
            System.Diagnostics.Debug.WriteLine("GetListing Start");

            if (!ftpClient.IsConnected)
            {
                var dialog = new MessageDialog("FTP Client is disconnected");
                await dialog.ShowAsync();
            }
            else
            {
                try
                {
                    var result = await ftpClient.GetListingAsync();

                    System.Diagnostics.Debug.WriteLine("GetListing Success Done");

                    return result;
                }
                catch(Exception ex)
                {
                    var dialog = new MessageDialog(ex.Message);
                    await dialog.ShowAsync();

                    return null;
                }
            }

            return null;
        }

        private async void ServerFileList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            string currentString;

            if (!ServerPath.Text.EndsWith("/"))
            {
                currentString = ServerPath.Text + "/" + (ServerFileList.SelectedItem as string) + "/";
            }
            else
            {
                currentString = ServerPath.Text + (ServerFileList.SelectedItem as string) + "/";
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("DoubleTapped - Calling UpdateFileList");

                await ftpClient.SetWorkingDirectoryAsync(currentString);

                System.Diagnostics.Debug.WriteLine("DoubleTapped - Completed call to UpdateFileList");

                //UpdateFileList();
                ServerPath.Text = currentString;

                currentServerPath = currentString;

                // Save last directory as default for this server.
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                string FTPServerName = FTPServer.Items[FTPServer.SelectedIndex].ToString();
                localSettings.Values[FTPServerName + "_DefaultDir"] = currentServerPath;
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

        private async void ServerPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Server Path Changed");
            try
            {
                await ftpClient.SetWorkingDirectoryAsync(ServerPath.Text);

                UpdateServerFileList();

                currentServerPath = ServerPath.Text;

                // Save last directory as default for this server.
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                string FTPServerName = FTPServer.Items[FTPServer.SelectedIndex].ToString();
                localSettings.Values[FTPServerName + "_DefaultDir"] = currentServerPath;
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();

                ServerPath.Text = currentServerPath;
            }

        }

        private async void GoUp_Click(object sender, RoutedEventArgs e)
        {
            string currentString;

            if (!string.IsNullOrEmpty(ServerPath.Text))
            {
                // Find the next directory up.  If it ends with /, then find the next slash.
                if (ServerPath.Text.EndsWith("/"))
                {
                    currentString = ServerPath.Text.Substring(0, ServerPath.Text.Length - 1);
                }
                else
                {
                    currentString = ServerPath.Text;
                }

                int i = currentString.LastIndexOf("/");
                if(i >= 0)
                {
                    currentString = currentString.Substring(0, i+1);

                    try
                    {
                        await ftpClient.SetWorkingDirectoryAsync(currentString);

                        ServerPath.Text = currentString;

                        currentServerPath = currentString;

                        // Save last directory as default for this server.
                        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                        string FTPServerName = FTPServer.Items[FTPServer.SelectedIndex].ToString();
                        localSettings.Values[FTPServerName + "_DefaultDir"] = currentServerPath;
                    }
                    catch (Exception ex)
                    {
                        var dialog = new MessageDialog(ex.Message);
                        await dialog.ShowAsync();
                    }
                }
            }
        }

        private async void UpdateServerFileList()
        {
            System.Diagnostics.Debug.WriteLine("UpdateFileList");

            try
            {
                List<FtpItem> list = await GetListing(ftpClient);

                if (list != null)
                {
                    ServerFileList.Items.Clear();

                    foreach (FtpItem fi in list)
                    {
                        ServerFileList.Items.Add(fi.FullName);
                    }
                }
                else
                {
                    var dialog = new MessageDialog("Error is directory list from FTP server is empty.");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }


        private async void AddFTPServer_Click(object sender, RoutedEventArgs e)
        {
            AddFTPServerDialog d = new AddFTPServerDialog();

            await d.ShowAsync();

            AddFTPAddresses();
        }
        #endregion ServerUICommands

        #region LocalUICommands
        private async void UpdateLocalFileList()
        {
            LocalFiles.Items.Clear();

            try
            {
                IReadOnlyList<IStorageItem> itemList = await currentFolder.GetItemsAsync();

                foreach (IStorageItem f in itemList)
                {
                    if(f.IsOfType(StorageItemTypes.Folder) || 
                        (f.IsOfType(StorageItemTypes.File) && f.Name.EndsWith(".wav")))
                    {
                        LocalFiles.Items.Add(f.Name);

                    }
                }

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                localSettings.Values["LocalPath"] = currentFolder.Path;

                System.Diagnostics.Debug.WriteLine("Local Path List updated.");
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

        }

        private async void GoUpLocal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder f = await currentFolder.GetParentAsync();
                if(f != null)
                {
                    currentFolder = f;
                    LocalPath.Text = currentFolder.Path;
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

        private void LocalPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLocalFileList();
        }

        private async void LocalFiles_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                IReadOnlyList<IStorageItem> itemList = await currentFolder.GetItemsAsync();
                currentFolder = await currentFolder.GetFolderAsync(LocalFiles.SelectedItem as string);

                LocalPath.Text = currentFolder.Path;
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }
        #endregion LocalUICommands

        #region CopyCommands
        private async void CopyLocal_Click(object sender, RoutedEventArgs e)
        {
            if (ServerFileList.SelectedItem != null)
            {
                BusyIndicator.Visibility = Visibility.Visible;

                string ServerFilePath = ServerFileList.SelectedItem as string;
                string LocalFilePath = currentFolder.Path + "\\" + ServerFileList.SelectedItem;

                try
                {
                    if (await ftpClient.FileExistsAsync(ServerFilePath))
                    {
                        using (var stream = await ftpClient.OpenReadAsync(ServerFilePath))
                        {
                            try
                            {
                                var t = Task.Run(async () =>
                                {
                                    // Open the file to read from.
                                    using (FileStream fs = File.Create(LocalFilePath))
                                    {
                                        var buf = new byte[TransferSize].AsBuffer();

                                        do
                                        {
                                            var resultingBuffer = await stream.ReadAsync(buf, TransferSize, Windows.Storage.Streams.InputStreamOptions.Partial);

                                            await fs.WriteAsync(buf.ToArray(), 0, (int)buf.Length);
                                        } while (buf.Length > 0);
                                    }
                                }
                                );

                                t.Wait();

                                FtpResponse reply;
                                if (!(reply = await ftpClient.CloseDataStreamAsync(null)).Success)
                                {
                                    var dialog = new MessageDialog(reply.Message + " Error Closing Data Stream.");

                                    await dialog.ShowAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                var dialog = new MessageDialog("Copy Local, Read: " + ex.Message);
                                await dialog.ShowAsync();
                            }

                            UpdateLocalFileList();
                        }

                    }
                }
                catch(Exception ex)
                {
                    var dialog = new MessageDialog("Copy Local, Exists/Open: " + ex.Message);
                    await dialog.ShowAsync();

                }
                BusyIndicator.Visibility = Visibility.Collapsed;
            }
        }
        private async void CopyToServer_Click(object sender, RoutedEventArgs e)
        {
            if(LocalFiles.SelectedItem != null)
            {
                BusyIndicator.IsActive = true;

                string ServerFilePath = LocalFiles.SelectedItem as string;
                string LocalFilePath = currentFolder.Path + "\\" + LocalFiles.SelectedItem;

                if (await ftpClient.FileExistsAsync(ServerFilePath))
                    await ftpClient.DeleteFileAsync(ServerFilePath);

                using (var stream = await ftpClient.OpenWriteAsync(ServerFilePath))
                {
                    try
                    {
                        var t = Task.Run(async () => 
                        {
                            // Open the file to read from.
                            using (FileStream fs = File.OpenRead(LocalFilePath))
                            {
                                byte[] buf = new byte[TransferSize];
                                int bufsize = 0;

                                do
                                {
                                    bufsize =  await fs.ReadAsync(buf, 0, TransferSize);
                                    if(bufsize >= 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine("writing " + bufsize);
                                        await stream.WriteAsync(buf.AsBuffer());
                                        await stream.FlushAsync();
                                    }
                                    System.Diagnostics.Debug.WriteLine("wrote " + fs.Position + " bufsize is " + bufsize);
                                } while (bufsize > 0);
                                await stream.FlushAsync();
                                System.Diagnostics.Debug.WriteLine("flushing ");
                            }
                        }
                        );

                        t.Wait();

                        FtpResponse reply;
                        if (!(reply = await ftpClient.CloseDataStreamAsync(null)).Success)
                        {
                            var dialog = new MessageDialog(reply.Message + " Error Closing Data Stream.");
                           
                            await dialog.ShowAsync();
                        }

                        UpdateServerFileList();
                    }
                    catch (Exception ex)
                    {
                        var dialog = new MessageDialog(ex.Message);
                        await dialog.ShowAsync();
                    }
                }

                BusyIndicator.IsActive = false;
            }
        }
        #endregion CopyCommands

        internal bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            /*bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
                NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
            }*/

            // return unsnapped;
            return true;
        }

        private void ServerFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateServerFileList();
        }
    }
}
