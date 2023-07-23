using Mi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackgroundEasy.ViewModel
{
    public class BaseSidePanelViewModel : BaseViewModel
    {

        public event EventHandler CloseSidePanelRequested;
        public ICommand CollapseSidePanelCommand { get { return new MICommand(hndlCollapseSidePanelCommand); } }

        private void hndlCollapseSidePanelCommand()
        {
            CloseSidePanelRequested?.Invoke(this, new EventArgs());
        }



    }
}
