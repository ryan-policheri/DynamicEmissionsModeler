namespace DotNetCommon.MVVM
{
    public class CheckableItemViewModel : ViewModelBase
    {
		private bool _isChecked;
        public bool IsChecked
		{
			get { return _isChecked; }
			set { SetField(ref _isChecked, value);}
		}

        public int ItemId => Item.Id;

        public string ItemName => Item.Name;


        private IHaveIntIdAndName _item;
        public IHaveIntIdAndName Item
        {
            get { return _item; }
            set
            {
                SetField(ref _item, value);
                OnPropertyChanged(nameof(ItemId));
                OnPropertyChanged(nameof(ItemName));
            }
        }
	}
}
