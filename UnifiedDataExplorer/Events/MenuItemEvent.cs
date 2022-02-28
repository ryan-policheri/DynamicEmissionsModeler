namespace EIADataViewer.Events
{
    public class MenuItemEvent
    {
        public object Sender { get; set; }

        public string SenderTypeName { get; set; }

        public string MenuItemHeader { get; set; }

        public string Action { get; set; }
    }
}
