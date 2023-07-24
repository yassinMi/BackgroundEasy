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

namespace BackgroundEasy.ViewModel
{
    public class MainVM : BaseViewModel, IDropFilesTarget
    {

        public MainVM()
        {
            MainMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2), Dispatcher.CurrentDispatcher) { DiscardDuplicates = true };

            string connectionString = $"Data Source={ApplicationInfo.DATABASE_FILE};Version=3;";
            SH = new StorageHelper(connectionString);
            //var str = string.Join("\n", Enumerable.Range(0, 900).Select(s => s.ToString()));
            //CurrentSkuInputStr = str;
            //AddSkuFromTextInputCommand.Execute(null);

            foreach (var s in SH.GetItems())
            {
                Images.Add(s);
            }

            Images.CollectionChanged += (s, e) => { notif(nameof(IsEmptyStateVisible)); };
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
        /// the tasks (obsolete we're using a single task design)
        /// </summary>
        public ObservableCollection<STask> Tasks { get; set; }
        public ObservableCollection<string> Images { get; set; } = new ObservableCollection<string>();

        public CollectionViewSource TasksCvs { get; set; } = new CollectionViewSource();

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
            set { _IsImageTabSelected = value; notif(nameof(IsImageTabSelected));
                if (value) SelectedTabIx = 0;
            }
            get { return _IsImageTabSelected; }
        }


        private bool _IsBrushTabSelected;
        public bool IsBrushTabSelected
        {
            set { _IsBrushTabSelected = value; notif(nameof(IsBrushTabSelected));
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
                try
                {
                    UpdatePreview();

                }
                catch (Exception err)
                {


                    MessageBox.Show(err.ToString());
                }

            }
            get { return _SelectedTabIx; }
        }




        public event EventHandler AboutWindowOpenRequested;


        private System.Windows.Media.Color _ColorPickerValue;
        public System.Windows.Media.Color ColorPickerValue
        {
            set
            {
                _ColorPickerValue = value;
                notif(nameof(ColorPickerValue));
                ColorPickerBrushValue.Color = GetBrushColor();
                notif(nameof(ShortStringRep));
                PushUIToCurrentBackground();
            }
            get { return _ColorPickerValue; }
        }
        System.Windows.Media.Color GetBrushColor()
        {

            return new System.Windows.Media.Color()
            {
                R = ColorPickerValue.R,
                G = ColorPickerValue.G,
                B = ColorPickerValue.B,
                A = GetAlpha()
            };
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
        private double _ToleranceSliderValue;
        public double ToleranceSliderValue
        {
            set
            {
                _ToleranceSliderValue = value; notif(nameof(ToleranceSliderValue));
                ColorPickerBrushValue.Color = GetBrushColor();
                notif(nameof(ShortStringRep));
                PushUIToCurrentBackground();
            }
            get { return _ToleranceSliderValue; }
        }
        private byte GetAlpha()
        {
            return (byte)(255 * (1.0 - ToleranceSliderValue));
        }



        private string _CuurentBackgroundImagePath;
        public string CuurentBackgroundImagePath
        {
            set { _CuurentBackgroundImagePath = value; notif(nameof(CuurentBackgroundImagePath)); }
            get { return _CuurentBackgroundImagePath; }
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
            CurrentBackground = new Background()
            {
                BackgroundColor = System.Drawing.Color.FromArgb(GetAlpha(), ColorPickerValue.R, ColorPickerValue.G, ColorPickerValue.B)
            
            };
        }

        private ImageSource _PreviewImageSource;
        public ImageSource PreviewImageSource
        {
            set { _PreviewImageSource = value; notif(nameof(PreviewImageSource)); }
            get { return _PreviewImageSource; }
        }

     
        System.Windows.Media.Brush MiTransparencyBrush
        {
            get; set;
        } = App.Current.MainWindow.TryFindResource("MiTransparencyTiles") as System.Windows.Media.Brush;

        private void UpdatePreview()
        {
            var uri_str = "pack://application:,,,/Media/example-small.png";
            var packUri = new Uri(uri_str);
            var exampleImg = new  BitmapImage(new Uri(uri_str));
            
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            
            drawingContext.DrawRectangle(MiTransparencyBrush, null, new Rect(0, 0, exampleImg.Width, exampleImg.Height));
            drawingContext.DrawRectangle(ColorPickerBrushValue, null, new Rect(0, 0, exampleImg.Width, exampleImg.Height));

            drawingContext.DrawImage(exampleImg, new Rect(0, 0, exampleImg.Width, exampleImg.Height));

            drawingContext.Close();
            
            var renderTarget = new RenderTargetBitmap((int)exampleImg.Width, (int)exampleImg.Height, 96, 96, PixelFormats.Default);
            renderTarget.Render(drawingVisual);
            PreviewImageSource =  renderTarget;

        }


        public ICommand AboutCommand { get { return new MICommand(hndlAboutCommand); } }

        private void hndlAboutCommand()
        {
            AboutWindowOpenRequested?.Invoke(this, new EventArgs());
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




        public ICommand OpenSettingsCommand { get { return new MICommand<string>(hndlOpenSettingsCommand); } }

        private void hndlOpenSettingsCommand(string sect)
        {
            try
            {
                SettingsVM vm = new SettingsVM();
                //push config to vm
                vm.OutputFilenameTemplate = Config.OutputFilenameTemplate;

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
                    Config.OutputFilenameTemplate = vm.OutputFilenameTemplate;
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
                    foreach (var f in files)
                    {
                        var ext = Path.GetExtension(f).ToLower();
                        string[] expectedExtensiosns = new string[] { ".png" };
                        if (expectedExtensiosns.Contains(ext)){
                            hndlAddImageFromFiles_impl(f);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                ReportErr(err);
            }
            
        }

        private void hndlAddImageFromFiles_impl(string f)
        {
            Images.Add(f);
            SH.AddItem(f);

        }

        bool ValidateImagePath(string str)
        {
            //currently the olnly rule is ext being png
            return !string.IsNullOrWhiteSpace(str)
                && Path.GetExtension(str).Equals(".png", StringComparison.InvariantCultureIgnoreCase);
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
                string outputTemplate = Config.OutputFilenameTemplate?.Replace("{SKU}", "{sku}");
                
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
                if(IsImageTabSelected&&!File.Exists(CuurentBackgroundImagePath))
                {
                    throw new InvalidUserOperationException("Please specify the image to use as background");
                }
                if (IsSavedBgTabSelected )
                {
                    throw new InvalidUserOperationException("no background preset selected");
                }


                ScrapingHelper scHelp = new ScrapingHelper();
               

                //# execute app task
                CancellationTokenSource cts = new CancellationTokenSource();

                ViewModel.ScrapingProgressVM vm = new ScrapingProgressVM();
                vm.StepTitle = "-";
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
                    var total = targetImages.Length;
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
                        var source_task = scHelp.ScrapeMessages(targetImages, opts, (p) =>
                        {
                            progressThrottleCc++;
                            bytes += p.ImagesSize;
                            newimages += (p.NewImages );
                            existingImgs += p.ExistingImages;
                            failedImgs += p.FailedImages;
                            imageCC += p.ProcessedImages;
                            if (progressThrottleCc == 0 || progressThrottleCc % progressStep == 0 || progressThrottleCc == targetImages.Length)
                            {
                                vm.DownloadedSize = $"{Utils.BytesToString(bytes)}";
                                vm.SavedImages = (newimages + existingImgs).ToString();
                                vm.FailedImages = (failedImgs).ToString();
                                vm.ProgressPerc = (int)(100f*((double)imageCC) / (double)total);
                            }
                            
                            cts.Token.ThrowIfCancellationRequested();

                        });

                        stagecc++;
                        vm.StepTitle = $"Downloading...";
                        await source_task;

                    }
                    vm.IsIndeterminate = false;
                    vm.ProgressPerc = 100;
                    await Task.Delay(800);



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



        public ICommand ClearSkuCommand { get { return new MICommand(hndlClearSkuCommand, canExecuteClearSkuCommand); } }


        private bool canExecuteClearSkuCommand()
        {
            return Images.Count>0;
        }

        private void hndlClearSkuCommand()
        {
            try
            {
                var res = MessageBox.Show("list will be cleared", ApplicationInfo.APP_TITLE, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
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


    }
}
