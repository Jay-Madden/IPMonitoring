using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IPMonitoring.Models
{
    public static class PingAsync
    {
        public static async Task<List<PingReply>> AsyncPing(List<IPAddress> IPList)
        {
            Ping pingSender = new Ping();
            var tasks = IPList.Select(Ip => new Ping().SendPingAsync(Ip, 200));
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }
    }
}
