using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Mi.Common;
using System.Diagnostics;
using BackgroundEasy.Services;
using BackgroundEasy.Model;
using System.Threading;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Reflection.Emit;
using Mi.Common.Model;
using System.Reflection;
using Newtonsoft.Json.Linq;
using BackgroundEasy.Common;
//using OfficeOpenXml;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Mi.Common.Attached;

namespace BackgroundEasy.ViewModel
{
    public class MainVM : BaseViewModel, IDropFilesTarget, INativeDropUXVm
    {

        public MainVM()
        {
            MainMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2), Dispatcher.CurrentDispatcher) { DiscardDuplicates = true };

            string connectionString = $"Data Source={ApplicationInfo.DATABASE_FILE};Version=3;";
            SH = new StorageHelper(connectionString);
            SH.PresetUpdated += (s, e) =>
            {
                var affectedVm = PresetsVMS.FirstOrDefault(p => p.Model.Name == e.Name);
                if (affectedVm != null)
                {
                    affectedVm.Model = e;
                }
            };
            ProcessingHelper = new Processor();

            //# load images
            foreach (var s in SH.GetItems())
            {
                Images.Add(s);
            }
            //# loead presets
            foreach (var p in SH.GetPresets())
            {

                OnAddPreset(p);
            }
            if (Config.LastUserSelectedPresetName != null)
            {
                var toBeSeleceted = PresetsVMS.FirstOrDefault(pvm => pvm.Name == Config.LastUserSelectedPresetName);
                if (toBeSeleceted != null) MakeSelected(toBeSeleceted);
            }
            MakeSelectedTabIxdefered(Config.LastUserSelectedTabIx);
            //draw preview
            UpdateCurrentPreviewImage();
            ColorPickerBrushValue.Color = GetBrushColor();
            PushUIToCurrentBackground();
            Images.CollectionChanged += (s, e) => { notif(nameof(IsEmptyStateVisible)); };
        }

        private void MakeSelectedTabIxdefered(int ix)
        {
            if (ix == 0)
            {
                IsImageTabSelected = true; _IsBrushTabSelected = false; _IsSavedBgTabSelected = false;
            }
            else if (ix == 1)
            {
                _IsImageTabSelected = false; IsBrushTabSelected = true; _IsSavedBgTabSelected = false;
            }
            else if (ix == 2)
            {
                _IsImageTabSelected = false; _IsBrushTabSelected = false; IsSavedBgTabSelected = true;
            }
            _SelectedTabIx = ix;
            CoreUtils.WriteLine($"exiting MakeSelectedTabIxdefered {ix} -> {SelectedTabIx}");
        }

        private void SubPresetVMEvents(PresetVM p)
        {
            p.SelectRequest += h_PresetVM_SelectRequest;
            p.DeleteRequest += h_PresetVM_DeleteRequest;
        }

        private void h_PresetVM_DeleteRequest(object sender, EventArgs e)
        {
            PresetVM pvm = sender as PresetVM;
            if (CurrentSelectedPresetVM == pvm)
            {
                MakeSelected(null);
            }
            SH.DeletePreset(pvm.Name);
            PresetsVMS.Remove(pvm);
            Message($"Deleted preset '{pvm.Name}'");
        }

        private void h_PresetVM_SelectRequest(object sender, EventArgs e)
        {
            PresetVM pvm = sender as PresetVM;
            if(CurrentSelectedPresetVM!= pvm)
            {
                MakeSelected(pvm);
            }
        }

        StorageHelper SH { get; set; }

        #region app_state


        

   


        #endregion

        bool deferUpdateUserConfig = false;


       



        private string _AppStatusString;
        public string AppStatusString
        {
            set { _AppStatusString = value; notif(nameof(AppStatusString)); }
            get { return _AppStatusString; }
        }


        private bool _AppStatusHasProgress;
        public bool AppStatusHasProgress
        {
            set { _AppStatusHasProgress = value; notif(nameof(AppStatusHasProgress)); }
            get { return _AppStatusHasProgress; }
        }


        private int? _AppStatusProgressValue;
        /// <summary>
        /// null for indeterminate
        /// </summary>
        public int? AppStatusProgressValue
        {
            set { _AppStatusProgressValue = value; notif(nameof(AppStatusProgressValue)); }
            get { return _AppStatusProgressValue; }
        }



        private bool _IsLoggingIn;
        public bool IsLoggingIn
        {
            set { _IsLoggingIn = value; notif(nameof(IsLoggingIn)); }
            get { return _IsLoggingIn; }
        }





        public ObservableCollection<string> Profiles { get; set; } = new ObservableCollection<string>();
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<PresetVM> PresetsVMS { get; set; } = new ObservableCollection<PresetVM>();
        public ObservableCollection<string> Images { get; set; } = new ObservableCollection<string>();

        public CollectionViewSource TasksCvs { get; set; } = new CollectionViewSource();

        internal void OnAddPreset(Preset p)
        {
            var pvm = new PresetVM(p, this);
            PresetsVMS.Add(pvm);
            SubPresetVMEvents(pvm);
        }

        public SnackbarMessageQueue MainMessageQueue { get; set; }

        public ConfigService Config { get { return ConfigService.Instance; } }

        public void StatusIndeterminate(string lbl)
        {
            AppStatusHasProgress = true;
            AppStatusProgressValue = null;
            AppStatusString = lbl;
        }
        public void StatusDeterminate(int progressPerc)
        {
            AppStatusHasProgress = true;
            AppStatusProgressValue = progressPerc;
        }
        public void StatusClear()
        {
            AppStatusHasProgress = false;
            AppStatusString = null;
        }
        public void StatusLbl(string lbl)
        {
            AppStatusHasProgress = false;
            AppStatusString = lbl;
        }






        private string _CurrentInputDestinationFolder  = ConfigService.Instance.LastUserOutputDirectory;
        public string CurrentInputDestinationFolder
        {
            set { _CurrentInputDestinationFolder = value;
                notif(nameof(CurrentInputDestinationFolder));
                if (!deferUpdateUserConfig)
                    Config.LastUserOutputDirectory = value;
            }
            get { return _CurrentInputDestinationFolder; }
        }


        


        private bool _InputShouldSkipExisting = ConfigService.Instance.LastUserShouldSkipExisting;
        public bool InputShouldSkipExisting
        {
            set { _InputShouldSkipExisting = value;
                notif(nameof(InputShouldSkipExisting));
                if (!deferUpdateUserConfig)
                    Config.LastUserShouldSkipExisting = value;
            }
            get { return _InputShouldSkipExisting; }
        }







       






        private string _InputOutputFilenameTemplateStr="{ImageName}-bg.png";
        public string InputOutputFilenameTemplateStr
        {
            set { _InputOutputFilenameTemplateStr = value; notif(nameof(InputOutputFilenameTemplateStr)); }
            get { return _InputOutputFilenameTemplateStr; }
        }



       



        private bool _IsInDropFileState;
        public bool IsInDropFileState
        {
            set { _IsInDropFileState = value;
                notif(nameof(IsInDropFileState));
                notif(nameof(IsEmptyStateVisible));
            }
            get { return _IsInDropFileState; }
        }


        public bool IsEmptyStateVisible
        {
            
            get { return Images.Count==0 && !IsInDropFileState; }
        }





        public string WindowTitle
        {
            get
            {
                return
                 $"{ApplicationInfo.APP_TITLE} {ApplicationInfo.APP_VERSION_PART}"
                  ;
            }
        }



        private bool _IsImageTabSelected = true;
        public bool IsImageTabSelected
        {
            set { _IsImageTabSelected = value;
                notif(nameof(IsImageTabSelected));
                if (value) SelectedTabIx = 0;
            }
            get { return _IsImageTabSelected; }
        }


        private bool _IsBrushTabSelected;
        public bool IsBrushTabSelected
        {
            set { _IsBrushTabSelected = value;
                notif(nameof(IsBrushTabSelected));
                if (value) SelectedTabIx = 1;
            }
            get { return _IsBrushTabSelected; }
        }


        private bool _IsSavedBgTabSelected;
        public bool IsSavedBgTabSelected
        {
            set { _IsSavedBgTabSelected = 
                    value; notif(nameof(IsSavedBgTabSelected));
                if (value) SelectedTabIx = 2;
            }
            get { return _IsSavedBgTabSelected; }
        }


        private int _SelectedTabIx;
        public int SelectedTabIx
        {
            set { _SelectedTabIx = value;
                notif(nameof(SelectedTabIx));
                Config.LastUserSelectedTabIx = value;
                try
                {
                    PushUIToCurrentBackground();
                    //Message("push");
                }
                catch (Exception err)
                {


                    MessageBox.Show(err.ToString());
                }

            }
            get { return _SelectedTabIx; }
        }




        public event EventHandler AboutWindowOpenRequested;


        private System.Windows.Media.Color _ColorPickerValue=ConfigService.Instance.LastUserColorPickerValue;
        public System.Windows.Media.Color ColorPickerValue
        {
            set
            {
                _ColorPickerValue = value;
                notif(nameof(ColorPickerValue));
                ColorPickerBrushValue.Color = GetBrushColor();
                notif(nameof(ShortStringRep));
                if (!deferUpdateUserConfig)
                    Config.LastUserColorPickerValue = value;
                PushUIToCurrentBackground();
            }
            get { return _ColorPickerValue; }
        }
        System.Windows.Media.Color GetBrushColor()
        {
            var c =   new System.Windows.Media.Color()
            {
                R = ColorPickerValue.R,
                G = ColorPickerValue.G,
                B = ColorPickerValue.B,
                A = GetAlpha()
            };
            var hex = Utils.HexConverter(Utils.DrawingColorFromMediaColor(c));
            CurentBackgroundHexColor = c.ToString();
            return c;

        }
        public string ShortStringRep
        {
            get
            {
                return
                  "#" + ColorPickerValue.R.ToString("X2") + ColorPickerValue.G.ToString("X2") + ColorPickerValue.B.ToString("X2")
                  + (ToleranceSliderValue == 0 ? "" : $" ±{(ToleranceSliderValue * 100).ToString("0")}%")
                    ;
            }
        }
        private double _ToleranceSliderValue= ConfigService.Instance.LastUserToleranceSliderValue;
        public double ToleranceSliderValue
        {
            set
            {
                _ToleranceSliderValue = value; notif(nameof(ToleranceSliderValue));
                ColorPickerBrushValue.Color = GetBrushColor();
                notif(nameof(ShortStringRep));
                if (!deferUpdateUserConfig)
                    Config.LastUserToleranceSliderValue = value;
                PushUIToCurrentBackground();
            }
            get { return _ToleranceSliderValue; }
        }
        private byte GetAlpha()
        {
            return (byte)(255 * (1.0 - ToleranceSliderValue));
        }



        private string _CuurentBackgroundImagePath = ConfigService.Instance.LastUserBackgroundImagePath;
        public string CuurentBackgroundImagePath
        {
            set { _CuurentBackgroundImagePath = value;
                notif(nameof(CuurentBackgroundImagePath));
                if (!deferUpdateUserConfig)
                    Config.LastUserBackgroundImagePath = value;
                PushUIToCurrentBackground();
                notif(nameof(CurrentBgImgImageSource));
                notif(nameof(IsMultipleBgMode));
                notif(nameof(IsNavigatorVisible));
                notif(nameof(CuurentBackgroundImagePathUI));

            }
            get { return _CuurentBackgroundImagePath; }
        }


        private int _CurrentBgIx=0;
        public int CurrentBgIx
        {
            set { _CurrentBgIx = value;
                notif(nameof(CurrentBgIx));
                notif(nameof(CurrentBgIxUI));
                notif(nameof(CurrentBgImgImageSource));
                notif(nameof(CuurentBackgroundImagePathUI));
            }
            get { return _CurrentBgIx; }
        }

        /// <summary>
        /// <see cref="CurrentBgIx"/> +1
        /// </summary>
        public int CurrentBgIxUI
        {
            get { return CurrentBgIx+1; }
        }


        /// <summary>
        /// whether a single or a selected ic from the muliple imgs, this shows the path
        /// </summary>
        public string CuurentBackgroundImagePathUI
        {
            get {
                try
                {
                    return IsMultipleBgMode? CurrentBackgroundImagesPaths?.ElementAt(CurrentBgIx): CuurentBackgroundImagePath;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }




        private bool _IsBgInDropFileState;
        public bool IsBgInDropFileState
        {
            set { _IsBgInDropFileState = value; notif(nameof(IsBgInDropFileState)); }
            get { return _IsBgInDropFileState; }
        }



        private string[] _CurentBackgroundImagesPaths = ConfigService.Instance.LastUserBackgroundImagesPaths;
        public string[] CurrentBackgroundImagesPaths
        {
            set
            {
                _CurentBackgroundImagesPaths = value;
                notif(nameof(CurrentBackgroundImagesPaths));
                if (!deferUpdateUserConfig)
                    Config.LastUserBackgroundImagesPaths = value;
                PushUIToCurrentBackground();
                notif(nameof(CurrentBgImgImageSource));
                notif(nameof(IsClearBgImgsVisible));
                notif(nameof(IsMultipleBgMode));
                notif(nameof(IsNavigatorVisible));
                notif(nameof(CuurentBackgroundImagePathUI));
                if (value != null)
                {
                    CurrentBgIx = Math.Min(value.Length - 1, CurrentBgIx);
                }


            }
            get { return _CurentBackgroundImagesPaths; }
        }


        public ImageSource CurrentBgImgImageSource
        {
            get
            {
                try
                {
                    if (IsMultipleBgMode)
                    {
                        return new BitmapImage(new Uri(CurrentBackgroundImagesPaths[CurrentBgIx]));
                    }
                    else
                    {
                        return new BitmapImage(new Uri(CuurentBackgroundImagePath));
                    }
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public string[] PreviewExampleOptions { get; set; } = new string[] { "Portrait - small", "Portrait - large", "Landscape - small", "Landscape - large" };


        private string _SelectedPreviewExample = ConfigService.Instance.LastUserSelectedPreviewExample;
        public string SelectedPreviewExample
        {
            set { _SelectedPreviewExample = value;
                if (!deferUpdateUserConfig)
                    Config.LastUserSelectedPreviewExample = value;
                notif(nameof(SelectedPreviewExample));
                UpdateCurrentPreviewImage();
            }
            get { return _SelectedPreviewExample; }
        }



        public System.Windows.Media.SolidColorBrush ColorPickerBrushValue { get; set; } = new System.Windows.Media.SolidColorBrush();

        private Background _CurrentBackground;
        public Background CurrentBackground
        {
            set { _CurrentBackground = value;
                notif(nameof(CurrentBackground));
                try
                {
                    UpdatePreview();
                }
                catch (Exception err)
                {

                    MessageBox.Show(err.ToString());
                }
            }
            get { return _CurrentBackground; }
        }

        private void PushUIToCurrentBackground()
        {
            CoreUtils.WriteLine($"PushUIToCurrentBackground SelectedTabIx = {SelectedTabIx}");

            if (SelectedTabIx == 1)
            {
                CurrentBackground = new Background()
                {
                    BackgroundColor = System.Drawing.Color.FromArgb(GetAlpha(), ColorPickerValue.R, ColorPickerValue.G, ColorPickerValue.B)
                };
            }
            else if (SelectedTabIx == 0)
            {
                if (IsMultipleBgMode)
                {
                    CurrentBackground = new Background() { BackgroundImagePaths= CurrentBackgroundImagesPaths };
                }
                else if(CuurentBackgroundImagePath!=null)
                {
                    CurrentBackground = new Background() { BackgroundImagePath = CuurentBackgroundImagePath };
                }
                else
                {
                    CurrentBackground = null;
                }
            }
            else if (SelectedTabIx == 2)
            {
                if (CurrentSelectedPresetVM != null)
                {
                    var p = CurrentSelectedPresetVM.Model;
                    CurrentBackground = new Background()
                    {
                        BackgroundImagePath = p.ImagePath,
                        BackgroundImagePaths = p.ImagePaths,
                        BackgroundColor = p.IsImageType? default(System.Drawing.Color): p.TryGetColor()
                    };
                }
                else
                {
                    CurrentBackground = null;
                }
                
            }

        }

        private ImageSource _PreviewImageSource;
        public ImageSource PreviewImageSource
        {
            set { _PreviewImageSource = value; notif(nameof(PreviewImageSource)); }
            get { return _PreviewImageSource; }
        }


        private BitmapImage _CurrentPreviewImage;
        public BitmapImage CurrentPreviewImage
        {
            set { _CurrentPreviewImage = value;
                notif(nameof(CurrentPreviewImage));
                UpdatePreview();
            }
            get { return _CurrentPreviewImage; }
        }


       

        private void UpdateCurrentPreviewImage()
        {
            CoreUtils.WriteLine($"UpdateCurrentPreviewImage");

            BitmapImage res = null;
            if (SelectedPreviewExample == "Portrait - small")
            {
                var uri_str = "pack://application:,,,/Media/example-small.png";
                var exampleImg = new BitmapImage(new Uri(uri_str));
                res = exampleImg;
            }
            else if (SelectedPreviewExample == "Portrait - large")
            {
                var uri_str = "pack://application:,,,/Media/example-large.png";
                var exampleImg = new BitmapImage(new Uri(uri_str));
                res = exampleImg;
            }
            else if (SelectedPreviewExample == "Landscape - small")
            {
                var uri_str = "pack://application:,,,/Media/example-small.png";
                var exampleImg = new BitmapImage(new Uri(uri_str));
                res = flipImage(exampleImg);
            }
            else if (SelectedPreviewExample == "Landscape - large")
            {
                var uri_str = "pack://application:,,,/Media/example-large.png";
                var exampleImg = new BitmapImage(new Uri(uri_str));
                res = flipImage(exampleImg);
            }
            else
            {
                //the default is similar to Portrait - small
                var uri_str = "pack://application:,,,/Media/example-small.png";
                var exampleImg = new BitmapImage(new Uri(uri_str));
                res = exampleImg;
            }
            CurrentPreviewImage = res;
        }
        /// <summary>
        /// rotates the image 90 then flips the result horuzontally 
        /// </summary>
        /// <param name="exampleImg"></param>
        /// <returns></returns>
        private BitmapImage flipImage(BitmapImage exampleImg)
        {
            return ProcessingUtils.FlipImage(exampleImg);
        }

        System.Windows.Media.Brush MiTransparencyBrush
        {
            get; set;
        } = App.Current.MainWindow.TryFindResource("MiTransparencyTiles") as System.Windows.Media.Brush;

        private void UpdatePreview()
        {
            CoreUtils.WriteLine($"UpdatePreview");
            var exampleImg = CurrentPreviewImage;
            if (exampleImg == null) 
            {
                CoreUtils.WriteLine($"exampleImg null, exiting UpdatePreview");
                PreviewImageSource = null;
                return;
            }
            ImageSource preview_raw = null;
            Background bg = CurrentBackground;
            int width, height;
            if (bg == null)
            {
                CoreUtils.WriteLine($"CurrentBackground null, exiting UpdatePreview");
                preview_raw = CurrentPreviewImage;
                width = CurrentPreviewImage.PixelWidth;
                height = CurrentPreviewImage.PixelHeight;
            }
            else
            {
                try
                {
                    preview_raw = ProcessingHelper.AddBackgroundToImagePreview(exampleImg, bg, new BackgroundLayeringOptions() { Fit = ConfigService.Instance.BgPlacement == "Fit" }, CurrentBgIx);
                    width = (int)preview_raw.Width;
                    height = (int)preview_raw.Height;
                }
                catch (Exception err)
                {
                    ReportErr(err);
                    preview_raw = CurrentPreviewImage;
                    width = CurrentPreviewImage.PixelWidth;
                    height = CurrentPreviewImage.PixelHeight;
                }
                
            }


            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            //draw trancparency tiles
            drawingContext.DrawRectangle(MiTransparencyBrush, null, new Rect(0, 0, exampleImg.PixelWidth, exampleImg.PixelHeight));            
            //draw the combined image
            drawingContext.DrawImage(preview_raw, new Rect(0, 0, width, height));

            drawingContext.Close();
            var renderTarget = new RenderTargetBitmap((int)exampleImg.PixelWidth, (int)exampleImg.PixelHeight, 96, 96, PixelFormats.Default);
            renderTarget.Render(drawingVisual);
            PreviewImageSource =  renderTarget;

        }



        private string _CurentBackgroundHexColor;
        /// <summary>
        /// one way binding to UI, it only affects the state when triggering the PushHexCommand (via enter)
        /// </summary>
        public string CurentBackgroundHexColor
        {
            set { _CurentBackgroundHexColor = value; notif(nameof(CurentBackgroundHexColor)); }
            get { return _CurentBackgroundHexColor; }
        }



        private PresetVM _CurrentSelectedPresetVM;
        public PresetVM CurrentSelectedPresetVM
        {
            set { _CurrentSelectedPresetVM = value;
                notif(nameof(CurrentSelectedPresetVM));
                try
                {
                    PushUIToCurrentBackground();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
            get { return _CurrentSelectedPresetVM; }
        }




        public void MakeSelected(PresetVM pvm)
        {
            
            if (CurrentSelectedPresetVM != null)
            {
                CurrentSelectedPresetVM.IsSelected = false;
            }
            CurrentSelectedPresetVM = pvm;
            Config.LastUserSelectedPresetName = pvm?.Name;
            if (pvm != null)
            {
                pvm.IsSelected = true;
            }
            
        }



        public ICommand AboutCommand { get { return new MICommand(hndlAboutCommand); } }

        private void hndlAboutCommand()
        {
            AboutWindowOpenRequested?.Invoke(this, new EventArgs());
        }


        public bool IsNavigatorVisible
        {
            get { return IsMultipleBgMode&& CurrentBackgroundImagesPaths.Length>1; }
        }
        /// <summary>
        /// CurrentBackgroundImagesPaths != null && CuurentBackgroundImagePath == null
        /// </summary>
        public bool IsMultipleBgMode
        {
            get
            {
                return CurrentBackgroundImagesPaths != null && CuurentBackgroundImagePath == null;
            }
        }







        public ICommand DestinationFolderCommand { get { return new MICommand(hndlDestinationFolderCommand, canExecuteDestinationFolderCommand); } }

        private bool canExecuteDestinationFolderCommand()
        {
            return true;
        }

        private void hndlDestinationFolderCommand()
        {
            IOUtils.PromptPickingDirectory((s, c) => {
                if (c == true) return;
                CurrentInputDestinationFolder = s;
            }, "Output Directory");
        }

        public ICommand SelectBgImageCommand { get { return new MICommand(hndlSelectBgImageCommand, canExecuteSelectBgImageCommand); } }

        private bool canExecuteSelectBgImageCommand()
        {
            return true;
        }

        private void hndlSelectBgImageCommand()
        {
            var f = IOUtils.PromptOpeningPath(".png", "Select Background Image", "Image Files|*.*") ;
            CurrentBackgroundImagesPaths = null;
            CuurentBackgroundImagePath = f;
        }




        public ICommand OpenSettingsCommand { get { return new MICommand<string>(hndlOpenSettingsCommand); } }

        private void hndlOpenSettingsCommand(string sect)
        {
            try
            {
                SettingsVM vm = new SettingsVM();
                //push config to vm
                vm.OutputFilenameTemplate = Config.OutputFilenameTemplate;
                vm.CurrentBgPlacement = vm.BgPlacements.FirstOrDefault(p => Config.BgPlacement == p.Name) ?? vm.BgPlacements.First();
                View.SettingsWindow w = new View.SettingsWindow();
                w.DataContext = vm;
                if (!string.IsNullOrEmpty(sect))
                {
                    if (sect == "output-sect")
                    {

                    }
                }
                w.Owner = App.Current.MainWindow;
                w.ShowDialog();
                //# if not canceled, push setting from vm to app global variables and local storage 
                if (vm.Canceled == false)
                {
                    var old_pgPlc = Config.BgPlacement;
                    Config.OutputFilenameTemplate = vm.OutputFilenameTemplate;
                    Config.BgPlacement = vm.CurrentBgPlacement?.Name ?? "Contain";
                    if (Config.BgPlacement != old_pgPlc)
                    {
                        PushUIToCurrentBackground();
                    }
                }

            }
            catch (Exception err)
            {
                ReportErr(err);
            }
        }



        public void Message(string msg)
        {
            MainMessageQueue.Enqueue(msg);
        }

        public void ReportErr(Exception err, string context = null)
        {
            //# special expections
            if (err is InvalidUserOperationException)
            {
                ReportFriendlyErr(err);
                return;
            }
            if (err is NotImplementedException)
            {
                ReportFriendlyErr(err, false);
                return;
            }
            if (err is AggregateException)
            {
                var ag = (AggregateException)err;
                var inner = ag.InnerExceptions?.FirstOrDefault();
                ReportFriendlyErr(inner ?? err, false);
                return;
            }
            //# other
            if (context != null)
            {
                MessageBox.Show($"{context}:{err}", ApplicationInfo.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show($"Error:{err}", ApplicationInfo.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// shows only the message of the exception
        /// </summary>
        public void ReportFriendlyErr(Exception err, bool isWarning = true)
        {
            MessageBox.Show($"{err.Message}", ApplicationInfo.APP_TITLE, MessageBoxButton.OK, isWarning ? MessageBoxImage.Warning : MessageBoxImage.Error);
        }
        /// <summary>
        /// shows only the message of the exception
        /// </summary>
        public void ReportFriendlyErr(string msg, bool isWarning = true)
        {
            MessageBox.Show($"{msg}", ApplicationInfo.APP_TITLE, MessageBoxButton.OK, isWarning ? MessageBoxImage.Warning : MessageBoxImage.Error);
        }



        public bool OnClose()
        {
            return false;
        }

        public void HndlDropFiles(string senderKey, string[] files)
        {
            try
            {
                if (senderKey == "importDropOnImagesCollectionView")
                {
                     hndlAddImageFromFiles_impl_multiple(files);
                }
                else if (senderKey == "bg")
                {
                    hndlAddBgImageFromFiles_impl_multiple(files);
                }
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
            
        }

        private void hndlAddImageFromFiles_impl_single(string f)
        {
            if(Path.GetExtension(f).Equals(".png", StringComparison.OrdinalIgnoreCase) == false)
            {
                return;
            }
            var added = SH.AddItem(f);
            if (added)
            {
                Message("added image");
                Images.Add(f);
            }
            else
            {
                Message("skiped duplicate");
            }
           
        }
        /// <summary>
        /// does filtering as well, exception unsafe
        /// </summary>
        /// <param name="f"></param>
        private void hndlAddImageFromFiles_impl_multiple(IEnumerable<string> f)
        {
            var toBeAdded = f.Where(ValidateImagePath).ToArray();
            if (toBeAdded.Length == 0)
            {
                Message("no png images");
                return;
            }
            var added = SH.AddItems(toBeAdded);
            
            foreach (var item in toBeAdded)
            {
                if(!Images.Contains(item))
                Images.Add(item);
            }
            var dups = toBeAdded.Length - added;
            Message($"added {added} images"+(dups>0?$", duplicates:{toBeAdded.Length- added}":""));

        }

        bool ValidateImagePath(string str)
        {
            try
            {
                //currently the olnly rule is ext being png
                return !string.IsNullOrWhiteSpace(str)
                    && Path.GetExtension(str).Equals(".png", StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }
        bool ValidateBgImagePath(string str)
        {
            try
            {
                //currently the olnly rule is ext being png
                return !string.IsNullOrWhiteSpace(str)
                    && (
                    Path.GetExtension(str).Equals(".png", StringComparison.InvariantCultureIgnoreCase)
                    || Path.GetExtension(str).Equals(".jpg", StringComparison.InvariantCultureIgnoreCase)
                    || Path.GetExtension(str).Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase)
                    )
                    ;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// must call<see cref="IsTextMultipleImages(string)"/> before this
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        IEnumerable<string> GetImagesFromFolder(string dir)
        {
            throw new NotImplementedException();
        }
       

       


        








        public ICommand StartProcessingCommand { get { return new MICommand(hndlStartProcessingCommand, canStartProcessingCommand); } }

        private bool canStartProcessingCommand()
        {
            return true;
        }

        private void hndlStartProcessingCommand()
        {
            try
            {
                //# consolidate app task input

                string[] targetImages = SH.GetItems().ToArray();
                string outputFolder = CurrentInputDestinationFolder;
                bool shouldSkipExisting = InputShouldSkipExisting;
                string outputTemplate = Config.OutputFilenameTemplate?.Replace("{imagename}", "{ImageName}");
                
                //# validate app task input


                if (string.IsNullOrWhiteSpace(outputFolder))
                {
                    throw new InvalidUserOperationException("please specify an output folder");
                }
                Uri _;//unused
                if (!Uri.TryCreate(outputFolder, UriKind.Absolute, out _))
                {
                    throw new InvalidUserOperationException("please specify a valid output folder");
                }
                if (!targetImages.Any())
                {
                    throw new InvalidUserOperationException("Please add one or more Images");
                }
                if (string.IsNullOrWhiteSpace(outputTemplate))
                {
                    throw new InvalidUserOperationException("Please specify a Filename Template at settings screen");
                }
                
                
                if (!outputTemplate.Contains("{ImageName}"))
                {
                    throw new InvalidUserOperationException("Output Filename Template must contain {ImageName}");
                }
                if(IsImageTabSelected&&(!File.Exists(CuurentBackgroundImagePath) && CurrentBackgroundImagesPaths == null))
                {
                    throw new InvalidUserOperationException("Please specify the image to use as background");
                }
                if (CurrentBackground==null )
                {
                    throw new InvalidUserOperationException("no background selected");
                }

                /*if (CurrentBackground.BackgroundImagePath != null && CurrentBackground.BackgroundImage == null)
                {
                    CurrentBackground.BackgroundImage = File.ReadAllBytes(CurrentBackground.BackgroundImagePath);
                }*/

                Processor scHelp = new Processor();
               

                //# execute app task
                CancellationTokenSource cts = new CancellationTokenSource();

                ViewModel.ScrapingProgressVM vm = new ScrapingProgressVM();
                vm.StepTitle = "Processing...";
                vm.IsIndeterminate = false;
                vm.SavedImages = "0";
                vm.FailedImages = "0";
                vm.DownloadedSize = "0 B";
                vm.CancellationRequest += (s, e) => cts.Cancel();

                long bytes = 0;
                long newimages = 0;
                long existingImgs = 0;
                long failedImgs = 0;
                View.ScrapingProgressWindow w = new View.ScrapingProgressWindow();
                var t = Task.Run(async () =>
                {

                    var stagecc = 0;
                    var total = targetImages.Length* (CurrentBackground.IsMultiImageType?CurrentBackground.BackgroundImagePaths.Length:1);
                    var progressThrottleCc = 0;
                    for (int i = 0; i < 1; i++)
                    {
                        var imageCC = 0;
                        cts.Token.ThrowIfCancellationRequested();


                        //# create tas
                        ProcessingOptions opts = new ProcessingOptions()
                        {
                            DumpDir = outputFolder,
                            SkipExisting = shouldSkipExisting,
                            OutputFilenameTemplate = outputTemplate,
                            Background = CurrentBackground

                        };
                        Directory.CreateDirectory(opts.DumpDir);
                        int progressStep = Math.Max(1, total / 5000);
                        Action<ProcessingProgressReport> prog_cb = (p) =>
                        {
                            progressThrottleCc++;
                            bytes += p.ImagesSize;
                            newimages += (p.NewImages);
                            existingImgs += p.ExistingImages;
                            failedImgs += p.FailedImages;
                            imageCC += p.ProcessedImages;
                            if (progressThrottleCc == 0 || progressThrottleCc % progressStep == 0 || progressThrottleCc == targetImages.Length)
                            {
                                vm.DownloadedSize = $"{Utils.BytesToString(bytes)}";
                                vm.SavedImages = (newimages + existingImgs).ToString();
                                vm.FailedImages = (failedImgs).ToString();
                                vm.ProgressPerc = (int)(100f * ((double)imageCC) / (double)total);
                            }

                            cts.Token.ThrowIfCancellationRequested();

                        };
                        Task<int> source_task = null;
                        if (CurrentBackground.IsMultiImageType)
                        {
                            source_task = scHelp.ProcessImagesMultiple(targetImages, opts, prog_cb);
                        }
                        else
                        {
                            source_task = scHelp.ProcessImages(targetImages, opts, prog_cb);
                        }

                        stagecc++;
                        vm.StepTitle = $"Processing...";
                        await source_task;

                    }
                    vm.IsIndeterminate = false;
                    vm.ProgressPerc = 100;
                    await Task.Delay(400);



                }, cts.Token);

                t.ContinueWith(tt => {
                    vm.IsDone = true;

                    if (tt.Status == TaskStatus.RanToCompletion)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            w.Close();
                            OnTaskEnded(outputFolder, newimages);
                            StatusLbl($"Task completed (new: {newimages}, existing: {existingImgs}, failed: {failedImgs}, Total Size: {Utils.BytesToString(bytes)})");
                        });
                    }
                    else if (tt.Status == TaskStatus.Faulted)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            w.Close();
                            ReportErr(tt.Exception);
                        });
                    }
                    else if (tt.Status == TaskStatus.Canceled)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            w.Close();
                            OnTaskEnded(outputFolder, newimages);
                            StatusLbl($"Task aborted (new: {newimages}, existing: {existingImgs}, failed: {failedImgs}, Total Size: {Utils.BytesToString(bytes)})");
                        });
                    }

                });


                //# set up progress view


                w.DataContext = vm;
                w.Owner = App.Current.MainWindow;
                w.ShowDialog();
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
        }


        private void OnTaskEnded(string outputFolder, long msgs)
        {
            Config.LastRun = DateTime.Now;
            MainMessageQueue.Enqueue($"Done.", "Open", (e) => {
                Task.Run(() =>
                {
                    try
                    {
                        if (Directory.Exists(outputFolder)) ;
                        Process.Start(outputFolder);
                    }
                    catch (Exception)
                    {

                    }
                });
            },null,true,true,TimeSpan.FromSeconds(4));
        }



        public ICommand ClearImagesCommand { get { return new MICommand(hndlClearImagesCommand, canExecuteClearImagesCommand); } }


        private bool canExecuteClearImagesCommand()
        {
            return Images.Count>0;
        }

        private void hndlClearImagesCommand()
        {
            try
            {
                var res = MessageBox.Show("Images list will be cleared", ApplicationInfo.APP_TITLE, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (res != MessageBoxResult.OK) return;
                SH.ClearTable();
                Images.Clear();
                Message("Cleared List");
            }
            catch ( Exception err)
            {
                ReportErr(err);
            }
           
        }



        public ICommand SaveCurrentBackgroundAsPresetCommand { get { return new MICommand(hndlSaveCurrentBackgroundAsPresetCommand, canExecuteSaveCurrentBackgroundAsPresetCommand); } }

        public Processor ProcessingHelper { get; private set; }

        private bool canExecuteSaveCurrentBackgroundAsPresetCommand()
        {
            return IsBrushTabSelected|| IsImageTabSelected;
        }

        private void hndlSaveCurrentBackgroundAsPresetCommand()
        {
            try
            {
                if (CurrentBackground == null) throw new InvalidUserOperationException("no background");
                var vm = new CreateEditPresetVM(this, SH, CurrentBackground);
                var w = new View.CreateEditPresetWindow();
                w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                w.Owner = App.Current.MainWindow;
                w.DataContext = vm;
                vm.CloseWindowRequest += (s, e) =>
                {
                    w.Close();
                };
                w.ShowDialog();
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
           

        }



        public ICommand AddImageCommand { get { return new MICommand(hndlAddImageCommand, canExecuteAddImageCommand); } }

        private bool canExecuteAddImageCommand()
        {
            return true;
        }

        private void hndlAddImageCommand()
        {
            try
            {
                var f = IOUtils.PromptOpeningPath(".png", "Open Image", "PNG image|*.png");
                if (f == "") return;
                hndlAddImageFromFiles_impl_single(f);
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
        }


        public ICommand AddBgImageCommand { get { return new MICommand(hndlAddBgImageCommand, canExecuteAddBgImageCommand); } }

        private bool canExecuteAddBgImageCommand()
        {
            return true;
        }

        private void hndlAddBgImageCommand()
        {
            try
            {
                var f = IOUtils.PromptOpeningPath(".png", "Add Background Image", "All Files|*.*");
                if (f == "") return;
                hndlAddBgImageFromFiles_impl_single(f);
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
        }

        /// <summary>
        /// not optimised for bulk adding (ceates too many arrays)
        /// </summary>
        /// <param name="f"></param>
        private void hndlAddBgImageFromFiles_impl_single(string f)
        {
            //# case current mode is multiple
            if (CurrentBackgroundImagesPaths != null)
            {
                CurrentBackgroundImagesPaths = CurrentBackgroundImagesPaths.Append(f).ToArray();
                return;
            }
            //# case current mode is single
            else if (CurrentBackgroundImagesPaths==null && CuurentBackgroundImagePath != null)
            {
                var keepValue = CuurentBackgroundImagePath;
                CuurentBackgroundImagePath = null;
                CurrentBackgroundImagesPaths = new string[] { keepValue, f };
            }
            //# casell empty
            else
            {
                CurrentBackgroundImagesPaths = new string[] {  f };
            }

        }

        public ICommand AddImagesFromFolderCommand { get { return new MICommand(hndlAddImagesFromFolderCommand, canExecuteAddImagesFromFolderCommand); } }

        private bool canExecuteAddImagesFromFolderCommand()
        {
            return true;
        }

        private void hndlAddImagesFromFolderCommand()
        {
            try
            {
                IOUtils.PromptPickingDirectory((s, c) => {
                    if (c == true) return;
                    var pngFiles = Directory.GetFiles(s, "*.png");
                    hndlAddImageFromFiles_impl_multiple(pngFiles);
                }, "Input Directory");
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
        }


        public ICommand AddBgImagesFromFolderCommand { get { return new MICommand(hndlAddBgImagesFromFolderCommand, canExecuteAddBgImagesFromFolderCommand); } }

        private bool canExecuteAddBgImagesFromFolderCommand()
        {
            return true;
        }

        private void hndlAddBgImagesFromFolderCommand()
        {
            try
            {
                IOUtils.PromptPickingDirectory((s, c) => {
                    if (c == true) return;
                    var bgImgFiles = Directory.GetFiles(s, "*.png")
                    .Concat(Directory.GetFiles(s, "*.jpg"))
                    .Concat(Directory.GetFiles(s, "*.jpeg")).ToArray()
                    ;
                    hndlAddBgImageFromFiles_impl_multiple(bgImgFiles);
                }, "Add Backgound Images");
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
        }

        private void hndlAddBgImageFromFiles_impl_multiple(string[] bgImgFiles)
        {
            var validImgs = bgImgFiles.Where(ValidateBgImagePath);
            var validCC = validImgs.Count();
            if (validCC == 0)
            {
                Message("no valid backgound images");
                return;
            }
            //# case current mode is multiple
            if (IsMultipleBgMode)
            {
                CurrentBackgroundImagesPaths = CurrentBackgroundImagesPaths.Concat(validImgs).ToArray();
                
            }
            //# case current mode is single
            else if (CurrentBackgroundImagesPaths == null && CuurentBackgroundImagePath != null)
            {
                var keepValue = CuurentBackgroundImagePath;
                CuurentBackgroundImagePath = null;
                CurrentBackgroundImagesPaths = new string[] { keepValue }.Concat(validImgs).ToArray();
            }
            //# casell empty
            else
            {
                CurrentBackgroundImagesPaths = validImgs.ToArray();
            }
            Message($"added {validCC} backgound images");
        }

        public ICommand PushHexCommand { get { return new MICommand(hndlPushHexCommand, canExecutePushHexCommand); } }

        private bool canExecutePushHexCommand()
        {
            return true;
        }

        private void hndlPushHexCommand()
        {
            try
            {
                var obj = Utils.cc.ConvertFromString(CurentBackgroundHexColor);
                System.Drawing.Color c = (System.Drawing.Color)obj;
                ColorPickerValue = Utils.MediaColorFromDrawingColor( c);
                ToleranceSliderValue = ((double)ColorPickerValue.A) / 255f;
            }
            catch (Exception err)
            {
                
            }
            
        }


        public bool IsClearBgImgsVisible
        {
            get { return CuurentBackgroundImagePath != null || (CurrentBackgroundImagesPaths != null && CurrentBackgroundImagesPaths.Any()); }
        }



        public ICommand ClearBgImagesCommand { get { return new MICommand(hndlClearBgImagesCommand, canExecuteClearBgImagesCommand); } }

        private bool canExecuteClearBgImagesCommand()
        {
            return IsClearBgImgsVisible;
        }

        private void hndlClearBgImagesCommand()
        {
            try
            {
                CuurentBackgroundImagePath = null;
                CurrentBackgroundImagesPaths = null;
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
        }


        public ICommand NavigatePreviousBgImageCommand { get { return new MICommand(hndlNavigatePreviousBgImageCommand, canExecuteNavigatePreviousBgImageCommand); } }

        private bool canExecuteNavigatePreviousBgImageCommand()
        {
            return IsMultipleBgMode && (CurrentBgIx >0);
        }

        private void hndlNavigatePreviousBgImageCommand()
        {
            if (CurrentBgIx > 0)
            {
                CurrentBgIx--;
                UpdatePreview();
            }
        }


        public ICommand NavigateNextBgImageCommand { get { return new MICommand(hndlNavigateNextBgImageCommand, canExecuteNavigateNextBgImageCommand); } }

        private bool canExecuteNavigateNextBgImageCommand()
        {
            return IsMultipleBgMode&& (CurrentBgIx < CurrentBackgroundImagesPaths.Length - 1);
        }

        private void hndlNavigateNextBgImageCommand()
        {
            if (CurrentBgIx < CurrentBackgroundImagesPaths.Length - 1)
            {
                CurrentBgIx++;
                UpdatePreview();
            }
        }

        public void OnIsInDropFileStateChanged(string channel, bool value)
        {
            if (channel == "bg")
            {
                IsBgInDropFileState = value;
            }
        }
    }
}
