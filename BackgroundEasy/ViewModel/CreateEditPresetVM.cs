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

        public CreateEditPresetVM(MainVM mainVm, StorageHelper sh,Background capturedBg)
        {
            this.SH = sh;
            this.MainVm = mainVm;
            PresetsOptions = mainVm.PresetsVMS;
            this.CapturedBackground = capturedBg;
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


        private PresetVM _SelectedPreset;
        public PresetVM SelectedPreset
        {
            set {
                _SelectedPreset = value;
                notif(nameof(SelectedPreset));
                if (value != null) CurrentName = value.Name;
            }
            get { return _SelectedPreset; }
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


       
        private void hndlCreateCommand()
        {
            try
            {
                string name = CurrentName;
                if (string.IsNullOrWhiteSpace(CurrentName))
                {
                    throw new InvalidOperationException("please specify a preset name");
                }
                var maybeExisting = PresetsOptions.FirstOrDefault(p => p.Name == CurrentName)?.Model;

               if (maybeExisting!=null)
                {
                    //#edit existent mode
                    SH.UpdatePreset(maybeExisting, Model.Preset.FromBackground(CapturedBackground, name) );
                    MainVm.Message($"Updated Preset {name}");
                    OnCloseWindow();
                }
                else
                {
                    //# create new mode
                    var newPreset = Model.Preset.FromBackground(CapturedBackground, name);
                    SH.AddPreset(newPreset);
                    MainVm.Message($"Created Preset {name}");
                    MainVm.OnAddPreset(newPreset);
                    OnCloseWindow();
                }
                
            }
            catch (Exception err)
            {
                MainVm?.ReportErr(err);
            }

        }



      
        private IEnumerable<PresetVM> _PresetsOptions;
        /// <summary>
        /// to overrid a presert
        /// </summary>
        public IEnumerable<PresetVM> PresetsOptions
        {
            set { _PresetsOptions = value; notif(nameof(PresetsOptions)); }
            get { return _PresetsOptions; }
        }





        public ICommand CancelCommand { get { return new MICommand(hndlCancelCommand); } }

        public MainVM MainVm { get; private set; }
        public StorageHelper SH { get; private set; }
        public Background CapturedBackground { get; private set; }

        private void hndlCancelCommand()
        {
            this.OnCloseWindow();
        }

    }
}


