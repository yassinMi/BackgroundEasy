using BackgroundEasy.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Common.ViewModel
{
    public class BaseWindowViewModel : BaseViewModel
    {



        public event EventHandler CloseWindowRequest;
        /// <summary>
        /// call this to close the window (fire CloseWindowRequest)
        /// </summary>
        public void OnCloseWindow()
        {
            this.CloseWindowRequest?.Invoke(this, new EventArgs());
        }

        internal virtual void HandleClosing(CancelEventArgs e)
        {
            
        }
    }
}
