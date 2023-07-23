using Mi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackgroundEasy.ViewModel
{
    public class SettingsVM:BaseViewModel
    {
        public SettingsVM()
        {
            //d time
           
        }






        private int _RequestsIntervalMs;
        public int RequestsIntervalMs
        {
            set { _RequestsIntervalMs = value; notif(nameof(RequestsIntervalMs)); }
            get { return _RequestsIntervalMs; }
        }

        private bool _UseRequestsInterval;
        public bool UseRequestsInterval
        {
            set { _UseRequestsInterval = value; notif(nameof(UseRequestsInterval)); }
            get { return _UseRequestsInterval; }
        }


        private string _URLTemplate;
        public string URLTemplate
        {
            set { _URLTemplate = value; notif(nameof(URLTemplate)); }
            get { return _URLTemplate; }
        }


        private string _HeadersStr;
        public string HeadersStr
        {
            set { _HeadersStr = value; notif(nameof(HeadersStr)); }
            get { return _HeadersStr; }
        }


        private string _OutputFilenameTemplate;
        public string OutputFilenameTemplate
        {
            set { _OutputFilenameTemplate = value; notif(nameof(OutputFilenameTemplate)); }
            get { return _OutputFilenameTemplate; }
        }







        //reserved for cancelation 


        public bool Canceled { get; set; } 
        public event EventHandler CloseRequested;

        public ICommand SaveCommand { get { return new MICommand(hndlSaveCommand); } }

        private void hndlSaveCommand()
        {
            CloseRequested?.Invoke(this, null);
        }


        public ICommand CancelCommand { get { return new MICommand(hndlCancelCommand); } }

        private void hndlCancelCommand()
        {
            Canceled = true;
            CloseRequested?.Invoke(this, null);

        }




    }
}
