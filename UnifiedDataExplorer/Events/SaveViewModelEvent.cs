namespace UnifiedDataExplorer.Events
{
    public class SaveViewModelEvent
    {
        public object Sender { get; set; }

        public string SenderTypeName { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}