using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FbCalendar.Helper
{
    public static class Util
    {

        public static string LoggedInUsername
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(IsolatedStorageKeys.username))
                {
                    return (string)IsolatedStorageSettings.ApplicationSettings[IsolatedStorageKeys.username];
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(IsolatedStorageKeys.username))
                {
                    IsolatedStorageSettings.ApplicationSettings[IsolatedStorageKeys.username] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(IsolatedStorageKeys.username, value);
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }

        public static bool IsInternetAvailable
        {
            get
            {
                var manualResetEvent = new ManualResetEvent(false);
                bool internetConnectionAvailable = true;
                DeviceNetworkInformation.ResolveHostNameAsync(new DnsEndPoint("microsoft.com", 80),
                networkInfo =>
                {
                    if (networkInfo.NetworkInterface == null)
                    {
                        internetConnectionAvailable = false;
                    }
                    manualResetEvent.Set();
                }, null);
                manualResetEvent.WaitOne(TimeSpan.FromMilliseconds(50));
                return internetConnectionAvailable;
            }
        }
    }
}
