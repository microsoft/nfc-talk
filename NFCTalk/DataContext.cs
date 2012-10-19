/*
 * Copyright © 2012 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace NFCTalk
{
    class DataContext
    {
        static DataContext _singleton;

        public static NFCTalk.DataContext Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new NFCTalk.DataContext();

                return _singleton;
            }
        }

        public void Save()
        {
            try
            {
                IsolatedStorageSettings.ApplicationSettings["Settings"] = _settings;
                IsolatedStorageSettings.ApplicationSettings["Messages"] = _messages;
            }
            catch (Exception)
            {
            }
        }

        public void Load()
        {
            try
            {
                _settings = IsolatedStorageSettings.ApplicationSettings["Settings"] as Settings;
                _messages = IsolatedStorageSettings.ApplicationSettings["Messages"] as ObservableCollection<Message>;
            }
            catch (Exception)
            {
            }
        }

        Settings _settings = null;
        public Settings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = new Settings();

                return _settings;
            }
        }

        ObservableCollection<Message> _messages = null;
        public ObservableCollection<Message> Messages
        {
            get
            {
                if (_messages == null)
                    _messages = new ObservableCollection<Message>();

                return _messages;
            }
        }

        Communication _communication = null;
        public Communication Communication
        {
            get
            {
                if (_communication == null)
                    _communication = new Communication();

                return _communication;
            }
        }
    }
}
