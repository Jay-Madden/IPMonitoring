
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using IPMonitor.Models;
using Stylet;
using Screen = Stylet.Screen;

namespace IPMonitoring.Pages
{
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
            get => projectSelectString;
            set => projectSelectString = value;
        }

        public string MachineSelectString
        {
            get => machineSelectString;
            set => machineSelectString = value;
        }
        public string SelectedFilePath { get; set; }

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
                if (Int32.Parse(inputProjectNumber) > 0 && Int32.Parse(inputMachineNumber) > 0)
                {
                    _MachineSelectModel.AddMachine(Int32.Parse(inputProjectNumber),Int32.Parse(inputMachineNumber), _openBrowserDialog.FileName);
                    UpdateProjectCollections();
                    ProjectSelectIsEnabled = true;
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
