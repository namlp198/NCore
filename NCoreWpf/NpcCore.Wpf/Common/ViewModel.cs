using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace ValidationToolkit
{
    public abstract class ViewModel : ValidationErrorContainer, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected new void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
