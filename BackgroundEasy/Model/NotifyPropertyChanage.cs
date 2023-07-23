using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mi
{
    public class NotifyPropertyChanage:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void notif(string propertyName)
        {
            var ev = PropertyChanged;
            ev?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
