using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPMonitoring.Models
{
    public class IPDataModel
    {
        private bool connected;
        private IPAddress ip;
        private string device;
        private string category;
        private string eplanCode;
        private string name;

        public IPDataModel()
        {

        }
        public bool Connected
        {
            get => connected;
            set => connected = value;
        }
        public IPAddress Ip
        {
            get => ip;
            set => ip = value;
        }

        public string Device
        {
            get => device;
            set => device = value;
        }

        public string Category
        {
            get => category;
            set => category = value;
        }

        public string EplanCode
        {
            get => eplanCode;
            set => eplanCode = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

 
    }
}
