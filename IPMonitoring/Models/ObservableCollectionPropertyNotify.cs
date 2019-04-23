using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IPMonitoring.Models
{
    //create a new collection class with a refresh method  
    public class ObservableCollectionPropertyNotify<T> : ObservableCollection<T>
    {
        //OnCollectionChange method is protected, accesible only within a child class in this case. This is why  
        //I made a new Collection class with a public method Refresh.  
        public void Refresh(SynchronizationContext uiContext)
        {
            for (var i = 0; i < this.Count(); i++)
            {
                 uiContext.Send(x => this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)), null);
            }
        }
        public ObservableCollectionPropertyNotify<T> Dates
        {
            get;
            set;
        }

    }
}
