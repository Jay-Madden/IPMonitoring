using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPMonitoring.Models;
using Stylet;
using IPMonitoring.Pages;
namespace IPMonitoring.Pages
{
    public class MonitorIPViewModel : Screen
    {

        public MonitorIPViewModel(string filePath)
        {
            SelectedFilePath = filePath;
            IpData = new List<IPDataModel>();
            ParseHTML.ParseIpHTML(IpData, filePath);
        }

        public string SelectedFilePath { get; set; }
        public List<IPDataModel> IpData { get; set; }
    }
}
