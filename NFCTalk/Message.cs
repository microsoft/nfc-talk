/*
 * Copyright © 2012 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

namespace NFCTalk
{
    public class Message
    {
        public enum DirectionValue
        {
            In = 0,
            Out = 1
        }

        public DirectionValue Direction { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Archived { get; set; }
    }
}
