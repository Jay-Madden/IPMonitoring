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

namespace IPMonitoring.Pages
{
    public class MonitorIPViewModel : Screen
    {
        private List<IPAddress> IpAddressList;
        private Task<List<PingReply>> Responses;
        private string inputProjectNumber;
        private Thread PingThread;
        private SynchronizationContext uiContext;

        public MonitorIPViewModel(string filePath)
        {
            SelectedFilePath = filePath;
            IpData = new ObservableCollectionPropertyNotify<IPDataModel>();
            IpAddressList = new List<IPAddress>();
            IpDataGrid = new DataGrid();
            uiContext = SynchronizationContext.Current;

            ParseHTML.ParseIpHTML(IpData, filePath);
            for (int i = 0; i < IpData.Count; i++)
            {
                IpAddressList.Add(IpData[i].Ip);
            }

            RefreshIP = true;
            StartIpPingThread();
        }

        public DataGrid IpDataGrid { get; set; }
        public string SelectedFilePath { get; set; }
        public ObservableCollectionPropertyNotify<IPDataModel> IpData { get; set; }
        public bool RefreshIP { get; set; }
        public string InputProjectNumber
        {
            get => inputProjectNumber;
            set => inputProjectNumber = value;
        }

        public void StartIpPingThread()
        {
            PingThread = new Thread(PingIPs) {
                Name = "AsyncPingThread"
            };
            PingThread.Start();
        }

        public void PingIPs()
        {
            while (RefreshIP)
            {
                Responses = PingAsync.AsyncPing(IpAddressList);
                List<IPAddress> tempIpAddresses = new List<IPAddress>();
                foreach (var ip in Responses.Result)
                {
                    tempIpAddresses.Add(ip.Address);
                }

                for (int i = 0; i < IpData.Count; i++)
                {
                    if (tempIpAddresses.Contains(IpData[i].Ip))
                    {
                        uiContext.Send(x => IpData[i].Connected = true, null);
                    }
                    else
                    {
                        uiContext.Send(x => IpData[i].Connected = false, null);

                    }
                }
                IpData.Refresh(uiContext);
            }
            Thread.Sleep(500);
        }
    }
}
