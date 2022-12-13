namespace UnifiedDataExplorer.Services.WindowDialog
{
    public class ModalOptions
    {
        public bool ShowSave { get; set; }
        public bool ShowCancel { get; set; }
        public bool ShowDelete { get; set; }

        public static ModalOptions DefaultModalOptions => new ModalOptions { ShowSave = false, ShowDelete = false, ShowCancel = false };
        public static ModalOptions SaveCancelOption => new ModalOptions { ShowSave = true, ShowDelete = false, ShowCancel = true };
        public static ModalOptions SaveCancelDeleteOption => new ModalOptions { ShowSave = true, ShowDelete = true, ShowCancel = true };
    }
}
