namespace EIADataViewer.Events
{
    public class CloseViewModelEvent
    {
        public object Sender { get; set; }

        public string SenderTypeName { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
