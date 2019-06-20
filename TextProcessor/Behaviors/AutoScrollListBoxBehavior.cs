using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace TextProcessor.Behaviors
{
    class AutoScrollListBoxBehavior : Behavior<ListBox>
    {
        INotifyCollectionChanged observableItems;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
        }

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            // this handler is called when the window loads, and when the expander is expanded.  Prevent multiple subscriptions to CollectionChanged below
            if (observableItems != null)
                observableItems.CollectionChanged -= observableItems_CollectionChanged;

            // try to resolve an INotifyCollectionChanged instance
            ListCollectionView lcv = AssociatedObject.ItemsSource as ListCollectionView;
            observableItems = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (lcv != null)
                observableItems = lcv.SourceCollection as INotifyCollectionChanged;
            if (observableItems == null)
                return;

            observableItems.CollectionChanged += observableItems_CollectionChanged;
        }

        void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            object item = AssociatedObject.Items.GetItemAt(AssociatedObject.Items.Count - 1);
            AssociatedObject.ScrollIntoView(item);
        }

        void observableItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            object item = e.NewItems[e.NewItems.Count - 1];
            AssociatedObject.ScrollIntoView(item);
        }
    }
}
