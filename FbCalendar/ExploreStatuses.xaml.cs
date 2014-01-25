using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using FbCalendar.Helper;

namespace FbCalendar
{
    public partial class ExploreStatuses : PhoneApplicationPage
    {
        public ExploreStatuses()
        {
            InitializeComponent();
        }

        List<string> list_years = new List<string>();
        List<string> list_month = new List<string>()
            {
                "January", "February", "March", "April", "May", "June", "July",
                "August", "September", "October", "November", "December"
            };
        // string date;

        Status objStatus = new Status();


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //yearSelectionItemsControl.Items.Clear();

            foreach (var oneStatus in MainPage.oc_statuses)
            {
                objStatus.date = oneStatus.str_updatedTime.Substring(0, 10);

                char[] separator = { '-' };
                string[] separatDate = objStatus.date.Split(separator);

                string year = separatDate[0];
                string month = separatDate[1];

                if (list_years.Count == 0)
                {
                    list_years.Add(year);
                }
                else
                {
                    if (!(year == list_years.Last()))
                    {
                        list_years.Add(year);
                    }
                }
            }

            yearSelection_LongListSelector.ItemsSource = list_years;
            monthSelection_LongListSelector.ItemsSource = list_month;
            //yearSelection.ItemsSource = list_years;

        }

        ObservableCollection<Status> oc_sortedStatuses = new ObservableCollection<Status>();
        public void sortStatuses()
        {
            foreach (var checkStatus in MainPage.oc_statuses)
            {
                string date = checkStatus.str_updatedTime.Substring(0, 10);

                char[] separator = { '-' };
                string[] separatDate = date.Split(separator);

                string year = separatDate[0];
                string month = separatDate[1];

                #region month conversion
                switch (month)
                {
                    case "01":
                        month = "January";
                        break;

                    case "02":
                        month = "February";
                        break;

                    case "03":
                        month = "March";
                        break;

                    case "04":
                        month = "April";
                        break;

                    case "05":
                        month = "May";
                        break;

                    case "06":
                        month = "June";
                        break;

                    case "07":
                        month = "July";
                        break;

                    case "08":
                        month = "Augast";
                        break;

                    case "09":
                        month = "September";
                        break;

                    case "10":
                        month = "October";
                        break;

                    case "11":
                        month = "November";
                        break;

                    case "12":
                        month = "December";
                        break;
                }
                #endregion

                if (year == selectedYear && month == selectedMonth)
                {
                    oc_sortedStatuses.Add(checkStatus);
                }
            }
        }



        public static int numOfYears;
        public static int numOfMonth;
        public static string selectedYear;
        public static string selectedMonth;
        public static bool isYearSelected;
        public static bool isMonthSelected;
        private void yearChanged(object sender, SelectionChangedEventArgs e)
        {
            oc_sortedStatuses = null;
            oc_sortedStatuses = new ObservableCollection<Status>();

            monthSelection_LongListSelector.Visibility = Visibility.Visible;
            sp_noUpdates.Visibility = Visibility.Collapsed;


            numOfYears = list_years.Count();
            selectedYear = yearSelection_LongListSelector.SelectedItem.ToString();

            sortStatuses();


            List<string> list_statusMessages = new List<string>();

            foreach (var singleStatusObject in oc_sortedStatuses)
            {
                list_statusMessages.Add(singleStatusObject.message);
            }

            StatusesList_LongListSelector.Visibility = Visibility.Visible;
            sp_instructions.Visibility = Visibility.Collapsed;
            isYearSelected = true;


            if (oc_sortedStatuses.Count() == 0)
            {
                StatusesList_LongListSelector.Visibility = Visibility.Collapsed;
                sp_noUpdates.Visibility = Visibility.Visible;
            }
            else
            {
                StatusesList_LongListSelector.Visibility = Visibility.Visible;
                sp_noUpdates.Visibility = Visibility.Collapsed;

                StatusesList_LongListSelector.ItemsSource = list_statusMessages;
            }


            #region commented code - copied code
            //if (index_selectedYear == -1)
            //{
            //    gv_appBar_months.Visibility = Visibility.Collapsed;

            //    myListView.Visibility = Visibility.Collapsed;
            //    tBlock_instruction.Visibility = Visibility.Visible;

            //    isYearSelected = false;
            //}
            //else
            //{
            //    myListView.Visibility = Visibility.Visible;
            //    sp_instructions.Visibility = Visibility.Collapsed;

            //    isYearSelected = true;

            //    for (int i = 0; i <= numOfYears; i++)
            //    {
            //        if (index_selectedYear == i)
            //        {
            //            selectedYear = list_years[i];
            //        }
            //    }


            //    sortStatuses();

            //    if (oc_sortedStatuses.Count() == 0)
            //    {
            //        myListView.Visibility = Visibility.Collapsed;
            //        grid_noUpdates.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        myListView.ItemsSource = oc_sortedStatuses;
            //    }
            //}

            //if (!isYearSelected || !isMonthSelected)
            //{
            //    myListView.Visibility = Visibility.Collapsed;
            //    tBlock_instruction.Visibility = Visibility.Visible;
            //    grid_noUpdates.Visibility = Visibility.Collapsed;
            //}

            #endregion
        }

        private void monthChanged(object sender, SelectionChangedEventArgs e)
        {
            oc_sortedStatuses = null;
            oc_sortedStatuses = new ObservableCollection<Status>();

            numOfMonth = list_month.Count();
            selectedMonth = monthSelection_LongListSelector.SelectedItem.ToString();

            sp_noUpdates.Visibility = Visibility.Collapsed;

            sortStatuses();

            StatusesList_LongListSelector.Visibility = Visibility.Visible;
            sp_instructions.Visibility = Visibility.Collapsed;
            isYearSelected = true;

            List<string> list_statusMessages = new List<string>();

            foreach (var singleStatusObject in oc_sortedStatuses)
            {
                list_statusMessages.Add(singleStatusObject.message);
            }

            if (oc_sortedStatuses.Count() == 0)
            {
                StatusesList_LongListSelector.Visibility = Visibility.Collapsed;
                sp_noUpdates.Visibility = Visibility.Visible;
            }
            else
            {
                StatusesList_LongListSelector.Visibility = Visibility.Visible;
                sp_noUpdates.Visibility = Visibility.Collapsed;

                StatusesList_LongListSelector.ItemsSource = list_statusMessages;
            }

            #region commented -> copied code
            //if (index_selectedMonth == -1)
            //{
            //    myListView.Visibility = Visibility.Collapsed;
            //    tBlock_instruction.Visibility = Visibility.Visible;

            //    isMonthSelected = false;
            //}
            //else
            //{
            //    isMonthSelected = true;

            //    myListView.Visibility = Visibility.Visible;
            //    tBlock_instruction.Visibility = Visibility.Collapsed;

            //    for (int i = 0; i < 12; i++)
            //    {
            //        if (index_selectedMonth == i)
            //        {
            //            selectedMonth = list_month[i];
            //        }
            //    }

            //    AppBar_Top.IsOpen = false;
            //    sortStatuses();

            //    if (oc_sortedStatuses.Count() == 0)
            //    {
            //        myListView.Visibility = Visibility.Collapsed;
            //        grid_noUpdates.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        myListView.ItemsSource = oc_sortedStatuses;
            //    }
            //}

            //if (!isYearSelected || !isMonthSelected)
            //{
            //    myListView.Visibility = Visibility.Collapsed;
            //    tBlock_instruction.Visibility = Visibility.Visible;
            //}

            #endregion

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //App.FacebookSessionClient.Logout();
            App.FacebookSessionClient = null;
            App.FacebookSessionClient = new Facebook.Client.FacebookSessionClient("440989055940618");

            MainPage.oc_statuses = null;
            MainPage.oc_statuses = new ObservableCollection<Status>();

            oc_sortedStatuses = null;
            list_month = null;
            list_years = null;

            App.isAuthenticated = false;

            //base.OnNavigatedFrom(e);
        }

    }
}