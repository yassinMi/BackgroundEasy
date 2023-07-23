//v1
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Mi.Common
{

    /// <summary>
    /// ICommand basic implemenation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MICommand<T> : ICommand
    {
        public Action<T> CommandAction { get; set; }
        public Func<T, bool> CanExecuteFunc { get; set; }

        public MICommand(Action<T> _CommandAction)
        {
            CommandAction = _CommandAction;
            CanExecuteFunc = null;

        }

        public MICommand(Action<T> _CommandAction, Func<T, bool> _CanExecuteFunc)
        {
            CommandAction = _CommandAction;
            CanExecuteFunc = _CanExecuteFunc;
        }

        public void Execute(object parameter)
        {
            CommandAction((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }


    public static class UtilCommands
    {

        public static ICommand CopyToClipCommand { get { return new MICommand<object>(hndlCopyToClipCommand, canExecuteCopyToClipCommand); } }

        private static bool canExecuteCopyToClipCommand(object arg)
        {
            return arg!=null;
        }

        private static void hndlCopyToClipCommand(object arg)
        {
            try
            {
                Clipboard.SetText(arg.ToString());
            }
            catch (Exception)
            {
            }
        }


        public static ICommand OpenUrlCommand { get { return new MICommand<object>(hndlOpenUrlCommand, canExecuteOpenUrlCommand); } }

        private static bool canExecuteOpenUrlCommand(object arg)
        {
            return true;
        }

        private static void hndlOpenUrlCommand(object arg)
        {
            try
            {
                if (arg is Uri)
                {
                    Process.Start((arg as Uri).AbsolutePath);
                }
                else if (arg is string)
                {
                    Process.Start((arg as string));
                }
            }
            catch (Exception)
            {

            }
            
        }



    }

    public class MICommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public MICommand(Action _CommandAction)
        {
            CommandAction = _CommandAction;
            CanExecuteFunc = null;

        }

        public MICommand(Action _CommandAction, Func<bool> _CanExecuteFunc)
        {
            CommandAction = _CommandAction;
            CanExecuteFunc = _CanExecuteFunc;
        }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        internal void RaiseCanExecuteChanged()
        {
            
        }
    }

}