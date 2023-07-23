using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mi.Common.Attached
{
    public class TextBoxAttachedProperties
    {


        public static bool GetUseUpDownNavigation(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseUpDownNavigationProperty);
        }

        public static void SetUseUpDownNavigation(DependencyObject obj, bool value)
        {
            obj.SetValue(UseUpDownNavigationProperty, value);
        }

        // Using a DependencyProperty as the backing store for UseUpDownNavigation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseUpDownNavigationProperty =
            DependencyProperty.RegisterAttached("UseUpDownNavigation", typeof(bool), typeof(TextBoxAttachedProperties), new PropertyMetadata(false, OnUseUpDownNavigationPropertyChanged));

        private static void OnUseUpDownNavigationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = d as TextBox;
            if (tb == null) return;
            if((bool)e.NewValue ==true && (bool)e.OldValue == false)
            {
                //activate
                tb.PreviewKeyDown += HndlTbPreviewKeyDown;
            }
            else if ((bool)e.NewValue == false && (bool)e.OldValue == true)
            {
                //disable
                //activate
                tb.PreviewKeyDown -= HndlTbPreviewKeyDown;
            }

        }

        private static void HndlTbPreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            if(e.Key== Key.Up)
            {
                tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
            }
            else if (e.Key == Key.Down)
            {
                tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
        }






        public static readonly DependencyProperty EnterCommandProperty =
      DependencyProperty.RegisterAttached("EnterCommand", typeof(ICommand), typeof(TextBoxAttachedProperties), new PropertyMetadata(null, OnEnterCommandChanged));

        public static ICommand GetEnterCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(EnterCommandProperty);
        }

        public static void SetEnterCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EnterCommandProperty, value);
        }

        private static void OnEnterCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox != null)
            {
                textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                if (e.NewValue != null)
                {
                    textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                }
            }
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift)&&!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var textBox = sender as TextBox;
                var command = GetEnterCommand(textBox);
                if (command != null && command.CanExecute(textBox.Text))
                {
                    command.Execute(textBox.Text);
                }
                e.Handled = true;
            }
        }





        public static ICommand GetDblClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DblClickCommandProperty);
        }

        public static void SetDblClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DblClickCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for DblClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DblClickCommandProperty =
            DependencyProperty.RegisterAttached("DblClickCommand", typeof(ICommand), typeof(TextBoxAttachedProperties), new PropertyMetadata(null,DblClickCommandPropertyChae));

        private static void DblClickCommandPropertyChae(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox != null)
            {
                textBox.MouseDoubleClick -= TextBox_DblClk;

                if (e.NewValue != null)
                {
                    textBox.MouseDoubleClick += TextBox_DblClk;
                }
            }
        }

        private static void TextBox_DblClk(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            var command = GetDblClickCommand(textBox);
            if (command != null && command.CanExecute(textBox.Text))
            {
                command.Execute(textBox.Text);
            }
        }

        public static bool GetSelectAllAtFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllAtFocusProperty);
        }

        public static void SetSelectAllAtFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllAtFocusProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectAllAtFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectAllAtFocusProperty =
            DependencyProperty.RegisterAttached("SelectAllAtFocus", typeof(bool), typeof(TextBoxAttachedProperties), new PropertyMetadata(false,SelectAllChae));

        private static void SelectAllChae(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = d as TextBox;
            if (tb == null) return;
            if ((bool)e.NewValue == true && (bool)e.OldValue == false)
            {
                //activate
                tb.GotFocus += HndlTbGotFocus;
            }
            else if ((bool)e.NewValue == false && (bool)e.OldValue == true)
            {
                //disable
                //activate
                tb.GotFocus -= HndlTbGotFocus;
            }
        }

        private static void HndlTbGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox!= null)
            {
                textBox.SelectAll();
            }
        }
    }
}
