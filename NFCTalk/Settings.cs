/*
 * Copyright © 2012-2014 Microsoft Mobile. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using System.ComponentModel;

namespace NFCTalk
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name = "";

        /// <summary>
        /// Chat name of this device.
        /// </summary>
        public string Name
        {
            get
            {
                if (_name.Length > 0)
                {
                    return _name;
                }
                else
                {
                    return "Anonymous";
                }
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
    }
}
