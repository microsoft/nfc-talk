using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFCTalk
{
    public class Settings : INotifyPropertyChanged
    {
        public void Save()
        {
            IsolatedStorageSettings.ApplicationSettings["Settings.Name"] = _name;
        }

        public void Load()
        {
            _name = IsolatedStorageSettings.ApplicationSettings["Settings.Name"] as string;
        }

        string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (value != _name)
                {
                    _name = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
