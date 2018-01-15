using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Financier.Desktop.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected BaseViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Use of CallerMemberName here allows sub-class code in property setters to simply call OnPropertyChanged()
        // public string MyProperty
        // {
        // set
        // {
        //     if (value != _myProperty)
        //     {
        //         _myProperty = value;
        //         OnPropertyChanged();
        //     }
        // }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
