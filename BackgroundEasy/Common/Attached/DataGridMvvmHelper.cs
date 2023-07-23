using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mi.Common.Attached
{
    public static class DataGridMvvmHelper
    {



        public static IItemsGridState GetState(DependencyObject obj)
        {
            return (IItemsGridState)obj.GetValue(StateProperty);
        }

        public static void SetState(DependencyObject obj, IItemsGridState value)
        {
            obj.SetValue(StateProperty, value);
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State", typeof(IItemsGridState), typeof(DataGridMvvmHelper), new PropertyMetadata(null, onStatePropChanged));




       

        
        private static void onStatePropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid g = d as DataGrid;
            if (d == null) return;
            object state = e.NewValue;
            Type objType = state.GetType();
            if(!(objType.IsGenericType &&objType.GetGenericTypeDefinition() == typeof(ItemsGridState<>)))
            {
                return;
            }

            UnsubscribeGridEvents(g);//just in case they alreaey subbed
            SubscribeGridEvents(g);
        }

        private static void SubscribeGridEvents(DataGrid g)
        {
            g.PreviewKeyDown += h_PreviewKeyDown;
            g.SelectionChanged += h_SelectionChanged;
        }
        private static void UnsubscribeGridEvents(DataGrid g)
        {
            g.PreviewKeyDown -= h_PreviewKeyDown;
            g.SelectionChanged -= h_SelectionChanged;
        }

        private static void h_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid g = sender as DataGrid;
            IItemsGridState state = GetState(g);
            if (state != null)
            {
                state.OnSelectionChanged(e);
            }

        }

        private static void h_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid g = sender as DataGrid;
            IItemsGridState state = GetState(g);
            if (state == null) return;
            if(Key.Delete == e.Key)
            {
                state.OnDelete();
                e.Handled = true;
            }
        }
    }
}

namespace Mi.Common
{
    /// <summary>
    /// represents a VM that can be used with the DataGridMvvmHelper to achieve selection binding etc
    /// </summary>
    public interface IDataGridsHostVM
    {
        void OnSelectedItemsChanged();
    }

    public interface IItemsGridState
    {
        void OnDelete();
        void OnSelectionChanged(SelectionChangedEventArgs e);
    }









    /// <summary>
    /// worsk best when you're binding a datagrid to a CollectionViewSource backekkd with an <see cref="ObservableCollection{T}"/> and want a property on your view model to hold user selection and provide hooks for user actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemsGridState<T> : IItemsGridState where T : class
    {
        public ItemsGridState()
        {
            Selection = new ObservableCollection<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SelectionChangedAction">these actions are equivalent to subscribing to the events <see cref="ItemsGridState{T}.SelectionChanged". passed as ctor args for convinience/></param>
        /// <param name="DeleteAction"></param>
        public ItemsGridState(Action SelectionChangedAction,Action DeleteAction=null)
        {
            Selection = new ObservableCollection<T>();

            if (SelectionChangedAction != null)
                SelectionChanged += (s, e) => { SelectionChangedAction(); };

            if (DeleteAction != null)
                DeletePressed += (s, e) => { DeleteAction(); };
        }
        /// <summary>
        /// all items currently selected, this is never null
        /// </summary>
        public ObservableCollection<T> Selection { get; set; }
        /// <summary>
        /// the selected item. or null if the selected items count is not 1
        /// </summary>
        public T SingleSelection { get; set; }
        /// <summary>
        /// the first item to be selected in the current selection (time wise)
        /// </summary>
        public T FirstSelection { get; set; }
       
        public T LastSelection { get; set; }
        /// <summary>
        /// the first item in the selection (position wise)
        /// NOTE: this is not set automatically, u must call <see cref="ItemsGridState{T}.ComputeTopAndBottomSelection(IEnumerable{T})"/> at <see cref="ItemsGridState{T}.SelectionChanged"/> before accessing this.
        /// </summary>
        public T TopSelection { get; set; }
        public T BottomSelection { get; set; }
        /// <summary>
        /// fired on any user action that causes the selection state to change; this will be fired only once per user action regardless of the number of items added or removed by that action (ctrl+z for example)
        /// 
        /// NOTE: this is fired after the state of the selection is updated.
        /// </summary>
        public event EventHandler SelectionChanged;
        /// <summary>
        /// fired when the datagrid recieves a deletion action (tipically when pressing del on the selected row(s))
        /// the passe dcollection is the same as <see cref=" DataGridProxy{T}.Selection"/> which holsd the selected items that you usually would want to delete from your backing collection 
        /// </summary>
        public event EventHandler<IEnumerable<T>> DeletePressed;

        public void OnDelete()
        {
            DeletePressed?.Invoke(this, Selection);
        }
        /// <summary>
        /// if you're binding the state in xaml via <see cref="Mi.Common.Attached.DataGridMvvmHelper.StateProperty"/> then you don't need to call this, otherwise it is to be called from the datagrid's <see cref="System.Windows.Controls.Primitives.Selector.SelectionChanged"/> event handler
        /// </summary>
        /// <param name="e"></param>
        public void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                Selection.Add((T)item);
            }
            foreach (var item in e.RemovedItems)
            {
                Selection.Remove((T)item);
            }
            if (Selection.Count == 1)
            {
                SingleSelection = Selection.FirstOrDefault();
            }
            else
            {
                SingleSelection = null;
            }
            FirstSelection = Selection.FirstOrDefault();
            LastSelection = Selection.LastOrDefault();
            
            SelectionChanged?.Invoke(this, new EventArgs());
        }
        [Obsolete("this is not implemented (an implementation can be added)",true)]
        /// <summary>
        /// the TopSelected is not automatically populated unless you call this from the SelectionChanged event
        /// </summary>
        /// <param name="Order">usually the view (order matters)</param>
        public void ComputeTopAndBottomSelection(IEnumerable<T> Order)
        {
            if (Selection.Any())
            {
                //todo: determin the top and bottom items here
            }
            else
            {
                TopSelection = null;
                BottomSelection = null;
            }
        }
    }
}
