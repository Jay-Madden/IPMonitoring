using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using IPMonitoring.Models;
using Stylet;
using IPMonitoring.Pages;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace IPMonitoring.Pages
{
    public class MonitorIPViewModel : Screen
    {
        #region Fields
        private List<IPAddress> IpAddressList;
        private Task<List<PingReply>> Responses;
        private string inputProjectNumber;
        private Thread PingThread;
        private SynchronizationContext uiContext;
        private Visibility refreshRateInputVisibilty;
        private IEventAggregator eventAggregator;
        private string refreshRateInput;
        private int internalRefreshRate; 
        #endregion

        #region Constructor
        public MonitorIPViewModel(string filePath, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            SelectedFilePath = filePath;
            IpData = new ObservableCollectionPropertyNotify<IPDataModel>();
            IpAddressList = new List<IPAddress>();
            IpDataGrid = new DataGrid();
            uiContext = SynchronizationContext.Current;
            RefreshRateInputVisibility = Visibility.Hidden;
            //sets default refresh rate at 500 ms
            internalRefreshRate = 500;

            ParseHTML.ParseIpHTML(IpData, filePath);
            for (int i = 0; i < IpData.Count; i++)
            {
                IpAddressList.Add(IpData[i].Ip);
            }

            RefreshIP = true;

            //~~~~~~~~~~~!!DO NOT CHANGE THIS!!~~~~~~~~~~~~~~~~~~~~~~
            //There is a bug in .NET frameworks 4 and up that causes a PROCCESS_HAS_LOCKED_PAGES blue screen of death
            //if you have a debugger attached and stop debugging while the ping is in process
            //If you want to test ping functionality you must build without debugging (CTRL-F5)

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                StartIpPingThread();
            }

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        } 
        #endregion

        #region Properties

        public DataGrid IpDataGrid { get; set; }
        public string SelectedFilePath { get; set; }
        public ObservableCollectionPropertyNotify<IPDataModel> IpData { get; set; }
        public bool RefreshIP { get; set; }

        public string InputProjectNumber
        {
            get => inputProjectNumber;
            set => inputProjectNumber = value;
        }

        public string RefreshRateInput
        {
            get => refreshRateInput;
            set => refreshRateInput = value;
        }


        public Visibility RefreshRateInputVisibility
        {
            get { return this.refreshRateInputVisibilty; }
            set { SetAndNotify(ref this.refreshRateInputVisibilty, value); }
        }

        #endregion

        #region Return To Machine

        public void ReturnToMachine_OnClick()
        {

            //Wait for ping thread to finish before returning to machineselectview
            RefreshIP = false;
            PingThread.Join();

            this.eventAggregator.Publish(new ReturnToMachine());
        }
        #endregion

        #region Refresh Rate
        public void AcceptRefreshRate_OnClick()
        {

            //Wait for ping thread to finish before returning to machineselectview
            RefreshRateInputVisibility = Visibility.Hidden;
            Int32.TryParse(RefreshRateInput, out internalRefreshRate);
        }

        public void SetRefreshRate_OnClick()
        {
            RefreshRateInputVisibility = Visibility.Visible;
        } 

        public void RefreshRateInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region Ping Functions

        public void StartIpPingThread()
        {
            PingThread = new Thread(PingIPs) {
                Name = "AsyncPingThread"
            };
            PingThread.Start();
            PingThread.IsBackground = true;
        }

        public void PingIPs()
        {
            while (RefreshIP)
            {
                Responses = PingAsync.AsyncPing(IpAddressList);
                List<IPAddress> tempIpAddresses = new List<IPAddress>();
                bool UIRefreshNeeded = false;
                foreach (var ip in Responses.Result)
                {
                    tempIpAddresses.Add(ip.Address);
                }

                for (int i = 0; i < IpData.Count; i++)
                {
                    if (tempIpAddresses.Contains(IpData[i].Ip))
                    {
                        if (IpData[i].Connected == false)
                        {
                            uiContext.Send(x => IpData[i].Connected = true, null);
                            UIRefreshNeeded = true;
                        }
                    }
                    else if(IpData[i].Connected == true)
                    {
                        uiContext.Send(x => IpData[i].Connected = false, null);
                        UIRefreshNeeded = true;

                    }
                }
                //Set so that the UI is only refreshed if a change in connection state is detected
                if (UIRefreshNeeded)
                {
                    IpData.Refresh(uiContext);
                    UIRefreshNeeded = false;
                }
                Thread.Sleep(internalRefreshRate);
            }

        }
        #endregion
    }
}
