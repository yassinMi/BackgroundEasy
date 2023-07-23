using Mi.Common;
using Mi.UI;
using Mi.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mi.ViewModel
{
    public class PromptWindowVM: INotifyPropertyChanged
    {
        public PromptWindowVM()
        {
            //design data
        }


        public event PropertyChangedEventHandler PropertyChanged;

        internal void notif(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public PromptWindowVM(PromptContent content)
        {
            PromptContent = content;
            Buttons = content.Buttons;
            notif(nameof(Buttons));
        }
        private string[] _buttons;
        public string[] Buttons
        {
            set { _buttons = value; }
            get { return PromptContent.Buttons; }
        }


        public string Result { get; set; } 


        private  PromptContent _PromptContent ;
        public PromptContent PromptContent
        {
            set { _PromptContent = value; }//todo remove, temporary for design data
            get { return _PromptContent; }
        }


        public ICommand ClickButtonCommand { get { return new MICommand<string>(hndlClickButtonCommand, canExecuteClickButtonCommand); } }

        private bool canExecuteClickButtonCommand(string arg)
        {
            return true;
        }

        private void hndlClickButtonCommand(string arg)
        {
            Result = arg;
            WindowObj?.Close();
        }



        public PromptWindow WindowObj { get; set; }
    }
}
