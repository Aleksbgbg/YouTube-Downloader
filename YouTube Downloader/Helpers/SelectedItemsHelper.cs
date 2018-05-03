namespace YouTube.Downloader.Helpers
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    internal static class SelectedItemsHelper
    {
        private static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached("SelectedItems",
                                                                                                               typeof(IList),
                                                                                                               typeof(SelectedItemsHelper),
                                                                                                               new FrameworkPropertyMetadata(SelectedItemsProperty_Changed));

        internal static IList GetSelectedItems(DependencyObject dependencyObject)
        {
            return (IList)dependencyObject.GetValue(SelectedItemsProperty);
        }

        internal static void SetSelectedItems(DependencyObject dependencyObject, IList value)
        {
            dependencyObject.SetValue(SelectedItemsProperty, value);
        }

        private static void SelectedItemsProperty_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (!(dependencyObject is Selector selector))
            {
                throw new InvalidOperationException($"Cannot assign {nameof(SelectedItemsProperty)} to non-{nameof(Selector)} object.");
            }

            IList selectedItems = (IList)e.NewValue;
            selector.SelectionChanged += SelectorSelectionChanged;

            void SelectorSelectionChanged(object sender, SelectionChangedEventArgs args)
            {
                foreach (object item in args.AddedItems)
                {
                    selectedItems.Add(item);
                }

                foreach (object item in args.RemovedItems)
                {
                    selectedItems.Remove(item);
                }
            }
        }
    }
}