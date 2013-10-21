NFC Talk
========

NFC Talk application demonstrates connection initiation by tapping devices
together or by searching for nearby devices over Bluetooth, using the
Windows Phone 8 Proximity API, the Windows.Networking.Proximity.PeerFinder
and related classes.

The example has been developed with XAML for Windows Phone devices and tested to
work on Nokia Lumia devices with Windows Phone 8.

This example application is hosted in GitHub:
https://github.com/nokia-developer/nfc-talk

For more information on implementation and porting, visit Nokia Lumia
Developer's Library:
http://developer.nokia.com/Resources/Library/Lumia/#!code-examples/nfc-talk.html


1. Instructions
--------------------------------------------------------------------------------

This is a simple build-and-run solution. Learn about Proximity API usage for
opening communication channels to application instances on other devices by
trying out the application. 

To build the application you need to have Windows 8 and Windows Phone SDK 8.0 or
later installed.

Using the Windows Phone 8 SDK:

1. Open the SLN file: File > Open Project, select the file `NFCTalk.sln`
2. Select the target 'Device'.
3. Press F5 to build the project and run it on the device.

Please see the official documentation for
deploying and testing applications on Windows Phone devices:
http://msdn.microsoft.com/en-us/library/gg588378%28v=vs.92%29.aspx


2. Implementation
--------------------------------------------------------------------------------

**Folders:**

 |                  The root folder contains the project file, the license 
 |                  information and this file (release_notes.txt).
 |
 |- NFCTalk         Root folder for the implementation files.  
 |  |
 |  |- Assets       Graphic assets like icons and tiles.
 |  |
 |  |- Properties   Application property files.
 |  |
 |  |- Resources    Application resources.


**Important files and classes:**

| File                           | Description                                |
|--------------------------------|--------------------------------------------|
| MainPage.xaml(.cs)             | The main page of the application.          |
|                                |                                            |
|--------------------------------|--------------------------------------------|
| TalkPage.xaml(.cs)             | The page that is used to chat with peer.   |
|                                |                                            |
|--------------------------------|--------------------------------------------|
| SettingsPage.cs                | The page that is used to configure chat    |
|                                | name.                                      |
|--------------------------------|--------------------------------------------|
| PeersPage.cs                   | The page that is used to show a list of    |
|                                | detected remote peers.                     |
|--------------------------------|--------------------------------------------|
| Communication.cs               | Class that encapsulates the usage of the   |
|                                | Proximity API.                             |
|--------------------------------|--------------------------------------------|


**Required capabilities:**

* ID_CAP_NETWORKING
* ID_CAP_PROXIMITY


3. License
--------------------------------------------------------------------------------

See the license text file delivered with this project. The license file is also
available online at https://github.com/nokia-developer/nfc-talk/blob/master/Licence.txt


4. Version history
--------------------------------------------------------------------------------

* Version 1.1: Peer browsing support (also for NFC-less devices).
* Version 1.0: The first release.
