/*
 * Copyright © 2012-2014 Microsoft Mobile. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

namespace NFCTalk
{
    /// <summary>
    /// Representation for a single chat message.
    /// </summary>
    public class Message
    {
        public enum DirectionValue
        {
            In = 0,
            Out = 1
        }

        /// <summary>
        /// Direction of message, in to this device, or out to the other device.
        /// </summary>
        public DirectionValue Direction { get; set; }

        /// <summary>
        /// Sender's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Is this message archived.
        /// </summary>
        public bool Archived { get; set; }
    }
}
