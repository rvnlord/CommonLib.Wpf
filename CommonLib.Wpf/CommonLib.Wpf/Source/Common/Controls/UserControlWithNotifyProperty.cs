using System.ComponentModel;
using System.Windows.Controls;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Wpf.Source.Common.Controls
{
    public abstract class UserControlWithNotifyProperty : UserControl, INotifyPropertyChanged, INotifyPropertyChangedHelper
    {
        public void SetPropertyAndNotify<T>(ref T field, T propVal, string propName)
        {
            if (Equals(field, propVal)) return;
            field = propVal;
            OnPropertyChanging(propName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanging(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
