﻿namespace UnifiedDataExplorer.Events
{
    public class MenuItemEvent
    {
        public object Sender { get; set; }

        public string SenderTypeName { get; set; }

        public string MenuItemHeader { get; set; }

        public string Action { get; set; }

        public object Data { get; set; }
    }
}
