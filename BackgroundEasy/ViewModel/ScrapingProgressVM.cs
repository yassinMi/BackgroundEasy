using Mi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BackgroundEasy.ViewModel
{
    public class ScrapingProgressVM: BaseViewModel
    {


        private string _StepTitle;
        public string StepTitle
        {
            set { _StepTitle = value; notif(nameof(StepTitle)); }
            get { return _StepTitle; }
        }


        private string _SavedImages;
        /// <summary>
        /// newly downloaded and previously downloaded 
        /// </summary>
        public string SavedImages
        {
            set { _SavedImages = value; notif(nameof(SavedImages)); }
            get { return _SavedImages; }
        }


       



        private string _FailedImages;
        public string FailedImages
        {
            set { _FailedImages = value; notif(nameof(FailedImages)); }
            get { return _FailedImages; }
        }





        private string _DownloadedSize;
        /// <summary>
        /// total size (inclues previously downloaded)
        /// </summary>
        public string DownloadedSize
        {
            set { _DownloadedSize = value; notif(nameof(DownloadedSize)); }
            get { return _DownloadedSize; }
        }


        private int _ProgressPerc;

        internal bool OnClose()
        {
            if (IsDone) return false;

            if (HasStartedCancelation) return true;
            var res = MessageBox.Show("Abort task?", ApplicationInfo.APP_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(res == MessageBoxResult.Yes)
            {
                hndlStopCommand();
                return true;
            }
            return true;
        }

        public int ProgressPerc
        {
            set { _ProgressPerc = value; notif(nameof(ProgressPerc)); }
            get { return _ProgressPerc; }
        }


        private bool _IsIndeterminate;
        public bool IsIndeterminate
        {
            set { _IsIndeterminate = value; notif(nameof(IsIndeterminate)); }
            get { return _IsIndeterminate; }
        }


        private bool _HasStartedCancelation;
        public bool HasStartedCancelation
        {
            set { _HasStartedCancelation = value; notif(nameof(HasStartedCancelation)); }
            get { return _HasStartedCancelation; }
        }

        public event EventHandler CancellationRequest;

        public ICommand StopCommand { get { return new MICommand(hndlStopCommand, canExecuteStopCommand); } }

        private bool canExecuteStopCommand()
        {
            return !HasStartedCancelation;
        }



        private void hndlStopCommand()
        {
            if (HasStartedCancelation) return;
            HasStartedCancelation = true;
            CancellationRequest?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// closing now works as the tasks has eaither completd or cancelled
        /// </summary>
        public bool IsDone { get; set; }

    }
}
