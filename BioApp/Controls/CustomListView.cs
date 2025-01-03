using System.Windows.Controls;
using System.Windows.Input;

namespace BioApp
{
    public class CustomListView : ListView
    {
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Override the default, sloppy behavior of key up and down events that are broken in WPF's ListView control.
            if (e.Key == Key.Up)
            {
                e.Handled = true;
                if (SelectedItems.Count > 0)
                {
                    int indexToSelect = Items.IndexOf(SelectedItems[0]) - 1;
                    if (indexToSelect >= 0)
                    {
                        SelectedItem = Items[indexToSelect];
                        ScrollIntoView(SelectedItem);
                    }
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                if (SelectedItems.Count > 0)
                {
                    int indexToSelect = Items.IndexOf(SelectedItems[SelectedItems.Count - 1]) + 1;
                    if (indexToSelect < Items.Count)
                    {
                        SelectedItem = Items[indexToSelect];
                        ScrollIntoView(SelectedItem);
                    }
                }
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
    }
}
