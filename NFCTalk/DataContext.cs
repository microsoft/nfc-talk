﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NFCTalk
{
    class DataContext
    {
        static DataContext _singleton;

        public static NFCTalk.DataContext Singleton()
        {
            if (_singleton == null)
                _singleton = new NFCTalk.DataContext();

            return _singleton;
        }

        public void Save()
        {
            try
            {
                //Settings.Save();
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
                //Settings.Load();
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
    }
}