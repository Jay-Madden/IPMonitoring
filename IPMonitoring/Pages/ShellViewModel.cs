using System;
using ControlzEx;
using Stylet;
using IPMonitoring;
using IPMonitoring.Models;

namespace IPMonitoring.Pages
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<OnStartIP>, IHandle<ReturnToMachine>
    {
        public MachineSelectViewModel MachineSelect { get; private set; }
        public MonitorIPViewModel MonitorIp { get; private set; }
        private IEventAggregator eventAggregator;

        public ShellViewModel(MachineSelectViewModel machineSelect, IEventAggregator eventAggregator)
        {

            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);

            MachineSelect = machineSelect;

            this.Items.Add(MachineSelect);
            this.ActiveItem = MachineSelect;
        }
        
        public void Handle(OnStartIP message)
        {
            MonitorIp = new MonitorIPViewModel(message.SelectedFilePath, eventAggregator);
            this.Items.Add(MonitorIp);
            this.ActiveItem = MonitorIp;
        }

        public void Handle(ReturnToMachine message)
        {
            MachineSelect = new MachineSelectViewModel(eventAggregator);
            this.ActiveItem = MachineSelect;
        }
    }
}
