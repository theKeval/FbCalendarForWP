using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FbCalendar.Resources;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using Facebook.Client;
using System.Threading.Tasks;
using Facebook;
using FbCalendar.Helper;
using System.Collections.ObjectModel;

namespace FbCalendar
{

    public class FBusers
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
    }


    public partial class MainPage : PhoneApplicationPage
    {

        private MobileServiceCollection<FBusers, FBusers> items;
        private IMobileServiceTable<FBusers> var_fbUser = App.MobileService.GetTable<FBusers>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }


        // private FacebookSession session;
        private async Task Authenticate()
        {
            string message = String.Empty;
            try
            {
                FacebookSession session = await App.FacebookSessionClient.LoginAsync("user_about_me,read_stream, email, user_mobile_phone, user_address,user_status");
                App.AccessToken = session.AccessToken;
                App.FacebookId = session.FacebookId;

                //App.FBLoginRecord += 1;
                App.isAuthenticated = true;
            }
            catch (Exception e)
            {
                App.isAuthenticated = false;

                message = "Login failed!";  // Exception details: " + e.Message

                progressBar.IsIndeterminate = false;
                rectangle_black.Visibility = Visibility.Collapsed;

                //MessageDialog dialog = new MessageDialog(message);
                //dialog.ShowAsync();

                //progressRing.IsActive = false;
                //rectDark.Visibility = Visibility.Collapsed;
                //loading_stackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void FBLogin(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Util.IsInternetAvailable)
            {
                //if (!App.isAuthenticated)
                if (!string.IsNullOrEmpty(Util.LoggedInUsername))
                {
                    //App.isAuthenticated = true;

                    progressBar.IsIndeterminate = true;
                    rectangle_black.Visibility = Visibility.Visible;

                    //rectDark.Visibility = Visibility.Visible;
                    //progressRing.IsActive = true;
                    //loading_stackPanel.Visibility = Visibility.Visible;

                    await Authenticate();

                    if (App.isAuthenticated)
                    {
                        //progressBar.IsIndeterminate = true;
                        //rectangle_black.Visibility = Visibility.Visible;

                        // rectDark.Visibility = Visibility.Visible;
                        // progressRing.IsActive = true;

                        await saveUserInfo();
                        await saveStatuses();
                        // sortStatuses();
                        //await saveFriendsInfo();

                        //rectDark.Visibility = Visibility.Collapsed;
                        //progressRing.IsActive = false;
                        //loading_stackPanel.Visibility = Visibility.Collapsed;

                        //MessageDialog msd = new MessageDialog("Login Successfull.");
                        //await msd.ShowAsync();

                        NavigationService.Navigate(new Uri("/ExploreStatuses.xaml", UriKind.Relative));

                        //this.Frame.Navigate(typeof(ExploreStatuses));
                    }
                    //Frame.Navigate(typeof(ShowSlambook));
                }
                else
                {
                    // this.Frame.Navigate(typeof(ExploreStatuses));

                    //customDialog_alreadySignedIn.IsOpen = true;

                    //MessageDialog msd = new MessageDialog("You already signed in to Facebook.");
                    //await msd.ShowAsync();
                }
            }
            else
            {
                MessageBox.Show("Looks like you are not connected to internet..!!\nCheck your internet connectivity and try again.",
                    "FbCalendar", MessageBoxButton.OK);
            }
        }

        public async Task saveUserInfo()
        {

            FacebookClient fbClient = new FacebookClient(App.AccessToken);

            //var me = await fbClient.GetTaskAsync("/me/statuses");
            var me = await fbClient.GetTaskAsync("me");
            var result = (IDictionary<string, object>)me;


            App.user_name = result["name"].ToString();
            App.user_eMail = result["email"].ToString();
            App.user_gender = result["gender"].ToString();

            Util.LoggedInUsername = App.user_name;

            //Constants.user_mobileNumber = result[""].ToString();
            //string lastName = result["last_name"].ToString();

            #region new code

            var userData = new FBusers
            {
                Name = App.user_name,
                Email = App.user_eMail,
                Gender = App.user_gender
            };
            InsertFBuserData(userData);

            #endregion

        }

        private async void InsertFBuserData(FBusers fbUserData)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await var_fbUser.InsertAsync(fbUserData);

            items = await var_fbUser.ToCollectionAsync();
            items.Add(fbUserData);
        }

        public static ObservableCollection<Status> oc_statuses = new ObservableCollection<Status>();
        public async Task saveStatuses()
        {
            FacebookClient fbClient = new FacebookClient(App.AccessToken);

            int limit = 100;
            int offset = 0;
            string statusRequest = "/me/statuses?limit=100&offset="; //+ offset;

            var statusesTask = await fbClient.GetTaskAsync(statusRequest + offset);

            while (statusesTask != null)    // && ((Facebook.JsonArray)statusesTask).Count > 0
            {
                var result = (IDictionary<string, object>)statusesTask;
                var data = (IEnumerable<object>)result["data"];

                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        try
                        {
                            var status = (IDictionary<string, object>)item;

                            oc_statuses.Add(new Status
                            {
                                message = status["message"].ToString(),
                                str_updatedTime = status["updated_time"].ToString(),
                                date = (status["updated_time"].ToString()).Substring(0, 10)
                            }
                                );
                        }
                        catch (KeyNotFoundException exe)
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    break;
                }

                offset += limit;
                statusesTask = await fbClient.GetTaskAsync(statusRequest + offset);

            }

            #region Commented old code
            //var result = (IDictionary<string, object>)statusesTask;
            //var data = (IEnumerable<object>)result["data"];

            //List<Status> friends = new List<Status>();

            //foreach (var item in data)
            //{
            //    try
            //    {
            //        var status = (IDictionary<string, object>)item;

            //        friends.Add(new Status
            //        {
            //            message = status["message"].ToString(),
            //            str_updatedTime = status["updated_time"].ToString()
            //        }
            //            );
            //    }
            //    catch (KeyNotFoundException exe)
            //    {
            //        continue;
            //    }
            //}
            #endregion
        }

        string day, month, year;
        public void sortStatuses()
        {
            foreach (var oneStatus in oc_statuses)
            {
                string date = oneStatus.str_updatedTime.Substring(0, 10);

                char[] separator = { '-' };
                string[] separatDate = date.Split(separator);

                year = separatDate[0];
                month = separatDate[1];
                day = separatDate[2];
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            progressBar.IsIndeterminate = false;
            rectangle_black.Visibility = Visibility.Collapsed;

            //base.OnNavigatedFrom(e);
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}