
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using IPMonitor.Models;
using Stylet;
using Screen = Stylet.Screen;

namespace IPMonitoring.Pages
{
    public class NamesWithCollection : ObservableCollection<Names>
    {
        public NamesWithCollection()
        {
            Add(new Names("Hi", "Hello"));
            Add(new Names("World", "Wurld"));
        }
    }
    public class Names
    {
        public Names(string name, string name1)
        {
            this.name1 = name;
            this.name2 = name1;
        }
        public string name1 { get; set; }
        public string name2 { get; set; }
    }

    public class MachineSelectViewModel : Screen
    {
        #region Fields
        private string inputProjectNumber;
        private string inputMachineNumber;
        private ObservableCollection<int> _projectSelectCollection;
        private ObservableCollection<int> _machineSelectCollection;
        public MachineSelectModel _MachineSelectModel;
        private bool machineSelectIsEnabled;
        private bool projectSelectIsEnabled;
        private bool startIPTestIsEnabled;
        private string projectSelectString;
        private string machineSelectString;
        private Visibility deleteMachineVisibility;
        private IEventAggregator eventAggregator;

        #endregion

        #region Constructor

        public MachineSelectViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            _MachineSelectModel = new MachineSelectModel();
            ProjectSelectCollection = new ObservableCollection<int>();
            MachineSelectCollection = new ObservableCollection<int>();
            MachineSelectIsEnabled = false;
            StartIPTestIsEnabled = false;
            InputMachineNumber = string.Empty;
            InputProjectNumber = string.Empty;
            DeleteMachineVisibility = Visibility.Hidden;
            if (_MachineSelectModel.MachineInfoDict.Count > 0)
            {
                UpdateProjectCollections();
                ProjectSelectIsEnabled = true;
            }
            else
            {
                ProjectSelectIsEnabled = false;
            }
            //NamesOC = new NamesWithCollection();
            //NamesOC = new ObservableCollection<string>
            //{
            //    "hello",
            //    "wor;d"
            //};


        } 

        #endregion

        #region Properties

        public string InputProjectNumber
        {
            get => inputProjectNumber;
            set => inputProjectNumber = value;
        }

        public string InputMachineNumber
        {
            get => inputMachineNumber;
            set => inputMachineNumber = value;
        }

        public ObservableCollection<int> ProjectSelectCollection
        {
            get => _projectSelectCollection;
            set => _projectSelectCollection = value;
        }

        public ObservableCollection<int> MachineSelectCollection
        {
            get => _machineSelectCollection;
            set => _machineSelectCollection = value;
        }
        
        public Visibility DeleteMachineVisibility
        {
            get { return this.deleteMachineVisibility; }
            set { SetAndNotify(ref this.deleteMachineVisibility, value); }
        }
        public bool MachineSelectIsEnabled
        {
            get { return this.machineSelectIsEnabled; }
            set { SetAndNotify(ref this.machineSelectIsEnabled, value); }
        }

        public bool ProjectSelectIsEnabled
        {
            get { return this.projectSelectIsEnabled; }
            set { SetAndNotify(ref this.projectSelectIsEnabled, value); }
        }

        public bool StartIPTestIsEnabled
        {
            get { return this.startIPTestIsEnabled; }
            set { SetAndNotify(ref this.startIPTestIsEnabled, value); }
        }

        public string ProjectSelectString
        {
            get { return this.projectSelectString; }
            set { SetAndNotify(ref this.projectSelectString, value); }
        }

        public string MachineSelectString
        {
            get { return this.machineSelectString; }
            set { SetAndNotify(ref this.machineSelectString, value); }
        }
        public string SelectedFilePath { get; set; }

        //public ObservableCollection<Names> NamesOC { get; set; }

        public NamesWithCollection NamesOC { get; set; } = new NamesWithCollection();

        //public ObservableCollection<string> NamesOC { get; set; }
        #endregion

        #region Functions

        public void UpdateProjectCollections()
        {
            ProjectSelectCollection.Clear();
            foreach (var key in _MachineSelectModel.MachineInfoDict.Keys)
            {
                ProjectSelectCollection.Add(key);
            }
        }
        public void UpdateMachineCollections(string projNum)
        {
            MachineSelectCollection.Clear();

            if (projNum == null) return;

            foreach (var key in _MachineSelectModel.MachineInfoDict[Int32.Parse(projNum)].Keys)
            {
                MachineSelectCollection.Add(key);
            }
        } 

        #endregion

        #region ProjectNumberInput

        public void ProjectNumberInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region MachineNumberInput

        public void MachineNumberInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region ProjectSelector

        public void ProjectSelect_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MachineSelectIsEnabled = true;
            UpdateMachineCollections(ProjectSelectString);
        }

        #endregion

        #region MachineSelector

        public void MachineSelect_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(MachineSelectString is null)
            {
                return;
            }
            StartIPTestIsEnabled = true;
            DeleteMachineVisibility = Visibility.Visible;
            SelectedFilePath =
                _MachineSelectModel.MachineInfoDict[Int32.Parse(ProjectSelectString)][Int32.Parse(MachineSelectString)];
        }

        #endregion

        #region BrowseFile

        public void BrowseFile()
        {
            OpenFileDialog _openBrowserDialog = new OpenFileDialog {
                Filter = "html files (*.html)|*.html|(*.*)|*.*"
            };
            DialogResult result = _openBrowserDialog.ShowDialog();
            if (result.ToString() == "OK")
            {
                
                if (Int32.TryParse(inputProjectNumber, out int tempProjectNumber) && Int32.TryParse(inputMachineNumber, out int tempMachineNumber))
                {
                    _MachineSelectModel.AddMachine(tempProjectNumber, tempMachineNumber, _openBrowserDialog.FileName);
                    ProjectSelectIsEnabled = true;
                    SelectedFilePath = _openBrowserDialog.FileName;
                    StartIPTest_Click();
                }
            }
        }

        #endregion

        #region DeleteMachine

        public void DeleteMachine_OnClick()
        {
            _MachineSelectModel.MachineInfoDict[Int32.Parse(ProjectSelectString)].Remove(Int32.Parse(MachineSelectString));
            UpdateMachineCollections(ProjectSelectString);
            DeleteMachineVisibility = Visibility.Hidden;
            StartIPTestIsEnabled = false;
        }

        #endregion

        #region StartIPTest

        public void StartIPTest_Click()
        {
            this.eventAggregator.Publish(new OnStartIP(SelectedFilePath)); 
        }

        #endregion
    }
}
