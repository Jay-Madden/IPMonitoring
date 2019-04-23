using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;

namespace IPMonitoring.Pages {
    public class TaskbarViewModel : Screen {

        public string Subreddit { get; set; }


        public TaskbarViewModel() {
        }

        public bool CanOpen {
            get { return !String.IsNullOrWhiteSpace(this.Subreddit); }
        }
        public void Open() {
            // TODO
        }
    }
}
