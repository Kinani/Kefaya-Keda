using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KefayaKeda.Common;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace KefayaKeda.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private CancellationTokenSource wtoken;
        private Task task;
        private HttpServer server;


        public CameraMan _cameraMan;
        public CaptureElement captureElement;

        #region Properties
        private StorageFile capturedImage { get; set; }
        #endregion

        #region Commands
        private RelayCommand captureEleLoaded;
        public RelayCommand CaptureEleLoaded
        {
            get
            {
                captureEleLoaded = new RelayCommand(async () =>
                {
                    _cameraMan = new CameraMan(captureElement);
                    await _cameraMan.StartPreviewAsync();
                    StartWork();
                });
                return captureEleLoaded;
            }
            set
            {
                captureEleLoaded = value;
            }
        }
        #endregion

        public MainPageViewModel()
        {
            // Posible bug? should we start it from a ThreadPool
            server = new HttpServer(8000);
            server.StartServer();
            server.OnStateChanged += Server_OnStateChanged;
        }

        private void Server_OnStateChanged(object source, string e)
        {
            if (e == "On")
            {
                // TODO
            }
            else
            {

            }
        }

        private async Task<AnalysisResult> DoVision(StorageFile file)
        {
            VisionServiceClient visonClient = new VisionServiceClient(Constants.OxfordVision);
            VisualFeature[] visualFeatures = new VisualFeature[] {VisualFeature.Description, VisualFeature.Tags };
            var fileStream = await file.OpenAsync(FileAccessMode.Read);
            return await visonClient.AnalyzeImageAsync(fileStream.AsStream(), visualFeatures);
        }

        void StopWork()
        {
            wtoken.Cancel();

            try
            {
                task.Wait();
            }
            catch (AggregateException) { }
        }
        void StartWork()
        {
            wtoken = new CancellationTokenSource();

            task = Task.Run(async () =>
            {
                while (true)
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                       .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                       {
                           capturedImage = await _cameraMan.CaptureImage();
                           AnalysisResult result = await DoVision(capturedImage);
                           Debug.WriteLine(result.Description.Captions[0].Text);
                       });

                    await Task.Delay(300000, wtoken.Token);
                }
            }, wtoken.Token);
        }
    }
}
