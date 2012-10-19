/*
 * Copyright © 2012 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using Microsoft.Phone.Controls;
using System.Xml.Linq;

namespace NFCTalk
{
    public partial class AboutPage : PhoneApplicationPage
    {

        public AboutPage()
        {
            InitializeComponent();

            versionTextBox.Text = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
        }
    }
}