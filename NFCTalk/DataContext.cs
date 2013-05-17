/*
 * Copyright © 2012-2013 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Windows.Networking.Proximity;

namespace NFCTalk
{
    /// <summary>
    /// DataContext holds application settings, messages and objects that
    /// are used from multiple pages.
    /// </summary>
    class DataContext
    {
        private static DataContext _singleton;
        private Settings _settings = null;
        private ObservableCollection<Message> _messages = null;
        private Communication _communication = null;
        private PeerInformation _peerInformation = null;

        /// <summary>
        /// DataContext singleton instance.
        /// </summary>
        public static NFCTalk.DataContext Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new NFCTalk.DataContext();
                }

                return _singleton;
            }
        }

        public PeerInformation PeerInformation
        {
            get
            {
                return _peerInformation;
            }

            set
            {
                if (_peerInformation != value)
                {
                    _peerInformation = value;
                }
            }
        }

        /// <summary>
        /// Save settings and messages to persistent storage.
        /// </summary>
        public void Save()
        {
            try
            {
                IsolatedStorageSettings.ApplicationSettings["Settings"] = _settings;
                IsolatedStorageSettings.ApplicationSettings["Messages"] = _messages;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Load settings and messages from persistent storage.
        /// </summary>
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

        /// <summary>
        /// Application settings.
        /// </summary>
        public Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings();
                }

                return _settings;
            }
        }

        /// <summary>
        /// Chat messages.
        /// </summary>
        public ObservableCollection<Message> Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new ObservableCollection<Message>();
                }

                return _messages;
            }
        }

        /// <summary>
        /// Communication object.
        /// </summary>
        public Communication Communication
        {
            get
            {
                if (_communication == null)
                {
                    _communication = new Communication();
                }

                return _communication;
            }
        }
    }
}
