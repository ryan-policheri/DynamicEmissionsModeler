using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnifiedDataExplorer.View.ProcessModeling
{
    public partial class DynamicTestingView : UserControl
    {
        public DynamicTestingView()
        {
            InitializeComponent();
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return int.TryParse(text, out int _);
        }

        private void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }
    }
}
