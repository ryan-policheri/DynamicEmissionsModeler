using System;

namespace UnifiedDataExplorer.Events
{
    public class LoadingEvent
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsLoading { get; set; }
    }
}
