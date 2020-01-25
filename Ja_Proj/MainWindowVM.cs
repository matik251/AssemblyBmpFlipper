using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Ja_Proj
{
    class MainWindowVM : BindableBase
    {

        AsmHandler asmLib = new AsmHandler();
        CHandler cLib = new CHandler();

        #region Properties

        private string _chooseDll;
        public string ChooseDll
        {
            get => _chooseDll;
            set
            {
                SetProperty(ref _chooseDll, value);
            }
        }

        private int _awaibleThreads;
        public int AwaibleThreads
        {
            get => _awaibleThreads;
            set
            {
                SetProperty(ref _awaibleThreads, value);
            }
        }

        private string _userImagePath = "C:/obrazek.jpg";
        public string UserImagePath
        {
            get => _userImagePath;
            set
            {
                SetProperty(ref _userImagePath, value);
                OpenFileCommand.CanExecute();
            }
        }

        private byte[] byteArrImage;
        public byte[] ByteArrImage { get => byteArrImage; set => byteArrImage = value; }

        public BitmapSource _inputImage;
        public BitmapSource InputImage
        {
            get => _inputImage;
            set
            {
                SetProperty(ref _inputImage, value);
            }
        }

        public int _coreCount;
        public int CoreCount
        {
            get => _coreCount;
            set
            {
                SetProperty(ref _coreCount, value);
            }
        }

        private int _pixelHeight;
        public int PixelHeight { get => _pixelHeight; set => _pixelHeight = value; }

        private int _pixelWidth;
        public int PixelWidth { get => _pixelWidth; set => _pixelWidth = value; }


        public BitmapSource _resultImage;
        public BitmapSource ResultImage
        {
            get => _resultImage;
            set
            {
                SetProperty(ref _resultImage, value);
            }
        }

        private ObservableCollection<TimeResult> _resultsList = new ObservableCollection<TimeResult>();
        public ObservableCollection<TimeResult> ResultsList
        {
            get => _resultsList;
            set
            {
                SetProperty(ref _resultsList, value);
            }

        }



        #endregion

        public MainWindowVM()
        {
            OpenFileCommand = new DelegateCommand(OpenUserImagePath, CanOpenUserImagePath);// OpenUserImagePath, CanOpenUserImagePath);
            OpenFileWindowCommand = new DelegateCommand(OpenFileDialog, CanPlaceholder);
            FlipImageCommand = new DelegateCommand(Placeholder, CanPlaceholder);
            int worker, io;
            ThreadPool.GetAvailableThreads(out worker, out io);
            AwaibleThreads = worker; // Environment.ProcessorCount;
        }

        #region Methods

        public void OpenFileDialog()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".bmp";
            dlg.Filter = "BMP Files (*.bmp)|*.bmp";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document 
                UserImagePath = dlg.FileName;
            }
        }


        private void OpenUserImagePath()
        {
            InputImage = ImageHandler.LoadBMP(UserImagePath);
            if (InputImage != null)
            {
                var img = ImageHandler.LoadImage(UserImagePath);
                PixelHeight = InputImage.PixelHeight;
                PixelWidth = InputImage.PixelWidth;
                ByteArrImage = ImageHandler.ToByteArray(img, ImageFormat.Bmp);
            }
        }

        private bool CanOpenUserImagePath()
        {
            if (UserImagePath.Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Placeholder()
        {
            if (ChooseDll != null)
            {
                if (ChooseDll.Equals("C"))
                {
                    //NOTE tutaj uruchomienie C
                    //cLib.C();

                    Stopwatch stopWatch = new Stopwatch();
                    BitmapSource resTemp = null;
                    //var temp = ImageHandler.LoadBMP(cLib.CFlip(ImageHandler.LoadImage(UserImagePath), PixelWidth, PixelHeight));
                    if (CoreCount != 0)
                    {
                        stopWatch.Start();
                        var res = cLib.CFlipMultithread(ImageHandler.LoadImage(UserImagePath), PixelWidth, PixelHeight, CoreCount);
                        stopWatch.Stop();
                        var name = ImageHandler.ByteArrayToBitmap(res);
                        resTemp = ImageHandler.LoadBMP(name);
                    }
                    else
                    {
                        stopWatch.Start();
                        var res = cLib.CFlipMultithread(ImageHandler.LoadImage(UserImagePath), PixelWidth, PixelHeight);
                        stopWatch.Stop();
                        var name = ImageHandler.ByteArrayToBitmap(res);
                        resTemp = ImageHandler.LoadBMP(name);

                    }

                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;
                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    ResultImage = resTemp;
                    MessageBox.Show("C RunTime: " + elapsedTime);

                }
                else if (ChooseDll.Equals("Asembler"))
                {
                    //NOTE tutaj asembler

                    Stopwatch stopWatch = new Stopwatch();
                    BitmapSource resTemp = null;
                    stopWatch.Start();
                    if (CoreCount != 0)
                    {
                        stopWatch.Start();
                        var res = asmLib.AsmFlipMultithread(ImageHandler.LoadImage(UserImagePath), PixelWidth, PixelHeight, CoreCount);
                        stopWatch.Stop();
                        var name = ImageHandler.ByteArrayToBitmap(res);
                        resTemp = ImageHandler.LoadBMP(name);

                    }
                    else
                    {
                        stopWatch.Start();
                        var res = asmLib.AsmFlipMultithread(ImageHandler.LoadImage(UserImagePath), PixelWidth, PixelHeight);
                        stopWatch.Stop();
                        var name = ImageHandler.ByteArrayToBitmap(res);
                        resTemp = ImageHandler.LoadBMP(name);
                    }
                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;

                    ResultImage = resTemp;
                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);

                    MessageBox.Show("Asm RunTime: " + elapsedTime);
                }
                else
                {
                    //nic
                }
            }
        }

        private bool CanPlaceholder()
        {
            return true;
        }



        #endregion


        #region Libraries



        #endregion


        #region Commands

        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand OpenFileWindowCommand { get; private set; }
        public DelegateCommand FlipImageCommand { get; private set; }

        #endregion

    }
}
