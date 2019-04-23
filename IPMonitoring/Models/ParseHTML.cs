using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace IPMonitoring.Models
{
    static class ParseHTML
    {
        const int LINESTOSKIP = 154;

        private static string PullInfoFromTag(string line)
        {
                int pFrom = line.IndexOf(">") + ">".Length;
                int pTo = line.LastIndexOf("<");
                return line = line.Substring(pFrom, pTo - pFrom);


        }


        public static List<IPDataModel> ParseIpHTML(List<IPDataModel> ipDataModel, string filePath)
        {
            if (File.Exists(filePath))
            {
                IPDataModel tempIpDataModel = new IPDataModel();

                string html = File.ReadAllText(filePath);

                var lines = Regex.Split(html, "\r\n|\r|\n").Skip(LINESTOSKIP);
                html = string.Join(Environment.NewLine, lines.ToArray());
                Console.WriteLine(html);

                using (var reader = new StringReader(html))
                {
                    string line = reader.ReadLine();
                    while(line != "</table>" && line != "</body>")
                    {
                        tempIpDataModel.Ip = PullInfoFromTag(line);

                        line = reader.ReadLine();
                        tempIpDataModel.Device = PullInfoFromTag(line);

                        line = reader.ReadLine();
                        tempIpDataModel.Category = PullInfoFromTag(line);

                        line = reader.ReadLine();
                        tempIpDataModel.Device = PullInfoFromTag(line);

                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        tempIpDataModel.EplanCode = PullInfoFromTag(line);

                        line = reader.ReadLine();
                        tempIpDataModel.Name = PullInfoFromTag(line);

                        ipDataModel.Add(tempIpDataModel);


                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                    }
                }
            }
            else
            {
                MessageBox.Show(" ERROR: File at: " + filePath + " no longer exists");
            }
            return ipDataModel;
        }
    }
}
