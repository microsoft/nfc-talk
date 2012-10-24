/*
 * Copyright © 2012 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace NFCTalk
{
    /// <summary>
    /// XAML template selector to select between current and archived messages as well as
    /// between inbound and outbound messages. See TalkPage.xaml for details on the templates.
    /// </summary>
    public class TalkTemplateSelector : ContentControl
    {
        public DataTemplate InBoundTemplate { get; set; }
        public DataTemplate OutBoundTemplate { get; set; }

        public DataTemplate InBoundArchivedTemplate { get; set; }
        public DataTemplate OutBoundArchivedTemplate { get; set; }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent);
        }

        /// <summary>
        /// If item is an inbound Message to a currently active session InBoundTemplate
        /// is selected. If item is an inbound Message to an already closed session
        /// InBoundArchivedTemplate is selected.
        /// 
        /// If item is an outbound Message to a currently active session OutBoundTemplate
        /// is selected. If item is an outbound Message to an already closed session
        /// OutBoundArchivedTemplate is selected.
        /// </summary>
        /// <param name="item">Message object</param>
        /// <returns></returns>
        public DataTemplate SelectTemplate(object item)
        {
            Message message = item as Message;
            Message.DirectionValue direction;
            bool archived;

            if (message == null)
            {
                Type itemType = item.GetType();

                PropertyInfo info = itemType.GetProperty("Direction");
                direction = (Message.DirectionValue)info.GetValue(item, null);

                info = itemType.GetProperty("Archived");
                archived = (bool)info.GetValue(item, null);
            }
            else
            {
                direction = message.Direction;
                archived = message.Archived;
            }

            if (direction == Message.DirectionValue.In)
            {
                return archived ? InBoundArchivedTemplate : InBoundTemplate;
            }
            else if (direction == Message.DirectionValue.Out)
            {
                return archived ? OutBoundArchivedTemplate : OutBoundTemplate;
            }

            return null;
        }
    }
}
