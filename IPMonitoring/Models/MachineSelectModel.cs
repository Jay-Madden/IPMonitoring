using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using Newtonsoft.Json;

namespace IPMonitor.Models {
    public class MachineSelectModel : EventArgs
    {

        public Dictionary<int, Dictionary<int, string>> MachineInfoDict;

        #region Constructor
        public MachineSelectModel() {
            MachineInfoDict = new Dictionary<int, Dictionary<int, string>>();
            LoadMachineInfo();
        }

        #endregion

        #region Getters and Setters



        #endregion

        #region Functions

        public void AddMachine(int projNum, int machNum, string filePath) 
        {
            if (MachineInfoDict.ContainsKey(projNum))
            {
                MachineInfoDict[projNum].Add(machNum, filePath);

            }
            else {
                MachineInfoDict.Add(projNum, new Dictionary<int, string>());
                MachineInfoDict[projNum].Add(machNum, filePath);
            }
            WriteMachineInfo(MachineInfoDict);
        }


        #endregion

        #region Functions
        
        private void LoadMachineInfo()
        {

            string loadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"userMachines.json");
            if (File.Exists(loadPath))
            {
                string json = File.ReadAllText(loadPath);
                //MachineInfoDict = JsonConvert.DeserializeObject <Dictionary<int, Dictionary<int, string>>(json);
                 MachineInfoDict = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, string>>>(json);

            }
        }


        private void WriteMachineInfo(Dictionary<int, Dictionary<int, string>> machineInfoDict)
        {
            //open file stream
            string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"userMachines.json");
            Console.WriteLine(destPath);
            //clear old JSON
            File.WriteAllText(destPath ,string.Empty);

            //write new object
            using (StreamWriter file = File.CreateText(destPath))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented
                };
                //serialize object directly into file stream
                serializer.Serialize(file, machineInfoDict);
            }
        }


        #endregion
    }
}