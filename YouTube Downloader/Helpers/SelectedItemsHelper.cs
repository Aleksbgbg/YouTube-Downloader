namespace YouTube.Downloader.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;

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
            IList listSelectedItems = (IList)e.NewValue;

            if (!(listSelectedItems is INotifyCollectionChanged observableCollection))
            {
                throw new ArgumentOutOfRangeException(nameof(e.NewValue), e.NewValue, $"Collection must be of {nameof(INotifyCollectionChanged)} type.");
            }

            if (!(dependencyObject is ListBox listBox))
            {
                throw new InvalidOperationException($"Cannot assign {nameof(SelectedItemsProperty)} to non-{nameof(ListBox)} object.");
            }

            IList listBoxSelectedItems = listBox.SelectedItems;

            void Add(IEnumerable items, IList list)
            {
                foreach (object item in items)
                {
                    list.Add(item);
                }
            }

            void Remove(IEnumerable items, IList list)
            {
                foreach (object item in items)
                {
                    list.Remove(item);
                }
            }

            listBox.SelectionChanged += (sender, args) =>
            {
                Add(args.AddedItems, listSelectedItems);
                Remove(args.RemovedItems, listSelectedItems);
            };

            observableCollection.CollectionChanged += (sender, args) =>
            {
                void AddNew()
                {
                    Add(args.NewItems, listBoxSelectedItems);
                }

                void RemoveOld()
                {
                    Remove(args.OldItems, listBoxSelectedItems);
                }

                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddNew();
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        RemoveOld();
                        break;

                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Move:
                        RemoveOld();
                        AddNew();
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        listBoxSelectedItems.Clear();
                        break;
                }
            };
        }
    }
}