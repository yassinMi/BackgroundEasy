using BackgroundEasy.Services;
using Mi.Common;
using Mi.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackgroundEasy.ViewModel
{
    public class CreateEditPresetVM: BaseWindowViewModel
    {

        public CreateEditPresetVM()
        {
            //time
        }

        public CreateEditPresetVM(MainVM mainVm, StorageHelper sh)
        {
            this.SH = sh;
            this.MainVm = mainVm;
            NamesOptions = mainVm.PresetsVMS.Select(p => p.Name).ToArray();
            Init();
        }







        private async void Init()
        {

            CurrentName = Services.Utils.GetNextNumberedName(MainVm.PresetsVMS.Select(s => s.Name), "New preset ", 1);
            
        }



      

        private string _CurrentName;
        public string CurrentName
        {
            set { _CurrentName = value; notif(nameof(CurrentName)); }
            get { return _CurrentName; }
        }


        private string _SelectedName;
        public string SelectedName
        {
            set { _SelectedName = value;
                notif(nameof(SelectedName));
                if (value != null) CurrentName = value;
            }
            get { return _SelectedName; }
        }


        private string _Text;
        public string Text
        {
            set { _Text = value;
                notif(nameof(Text));
                if (value != null) CurrentName = value;
            }
            get { return _Text; }
        }















        /// <summary>
        /// </summary>
        public event EventHandler<string> CreatedSuccess;
        /// <summary>
        /// </summary>
        public event EventHandler<Exception> Error;




        public ICommand CreateCommand { get { return new MICommand(hndlCreateCommand, canExecuteCreateCommand); } }

        private bool canExecuteCreateCommand()
        {
            return true;
        }


        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
        private void hndlCreateCommand()
        {
            try
            {
                string name = CurrentName;
                if (string.IsNullOrWhiteSpace(CurrentName))
                {
                    throw new InvalidOperationException("please specify a preset name");
                }
                SH.AddPreset(new Model.Preset() {
                    ImagePath = MainVm.CuurentBackgroundImagePath,
                    Name = name,

                    SolidColorHex = HexConverter(Services.Utils.DrawingColorFromMediaColor(MainVm.ColorPickerValue))
                });
                MainVm.Message($"saved {name}");
                OnCloseWindow(); 
            }
            catch (Exception err)
            {
                MainVm?.ReportErr(err);
            }

        }



      
        private string[] _NamesOptions;
        /// <summary>
        /// to overrid a presert
        /// </summary>
        public string[] NamesOptions
        {
            set { _NamesOptions = value; notif(nameof(NamesOptions)); }
            get { return _NamesOptions; }
        }





        public ICommand CancelCommand { get { return new MICommand(hndlCancelCommand); } }

        public MainVM MainVm { get; private set; }
        public StorageHelper SH { get; private set; }

        private void hndlCancelCommand()
        {
            this.OnCloseWindow();
        }

    }
}


