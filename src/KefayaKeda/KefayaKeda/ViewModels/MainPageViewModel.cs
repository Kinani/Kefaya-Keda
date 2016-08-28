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
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace KefayaKeda.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private CancellationTokenSource wtoken;
        private Task task;

        public CameraMan _cameraMan;
        public CaptureElement captureElement;

        #region Properties
        private StorageFile capturedImage { get; set; }
        private AppServiceConnection appServiceConnection { get; set; }
        public bool isWorking { get; set; } = true;

        #endregion

        #region Commands
        private RelayCommand captureEleLoaded;
        public RelayCommand CaptureEleLoaded
        {
            get
            {
                captureEleLoaded = new RelayCommand(async () =>
                {
                    if (appServiceConnection == null)
                    {
                        await InitAppSvc();
                    }
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

        public MainPageViewModel() { }

        private async Task<AnalysisResult> DoVision(StorageFile file)
        {
            VisionServiceClient visonClient = new VisionServiceClient(Constants.OxfordVision);
            VisualFeature[] visualFeatures = new VisualFeature[] {VisualFeature.Description, VisualFeature.Tags };
            var fileStream = await file.OpenAsync(FileAccessMode.Read);
            return await visonClient.AnalyzeImageAsync(fileStream.AsStream(), visualFeatures);
        }
        private async Task InitAppSvc()
        {
            // Initialize the AppServiceConnection
            appServiceConnection = new AppServiceConnection();
            appServiceConnection.PackageFamilyName = "3794d190-02a2-4c83-b044-74c5304739e9_7fvddcazhy2xa";
            //appServiceConnection.AppServiceName = "BackgroundWebServer-uwp";
            appServiceConnection.AppServiceName = "MySuperServer";
            // Send a initialize request 
            var res = await appServiceConnection.OpenAsync();
            if (res == AppServiceConnectionStatus.Success)
            {
                var message = new ValueSet();
                message.Add("Command", "Initialize");
                var response = await appServiceConnection.SendMessageAsync(message);
                if (response.Status != AppServiceResponseStatus.Success)
                {
                    throw new Exception("Failed to send message");
                }
                appServiceConnection.RequestReceived += OnMessageReceived;
            }
        }

        private async void OnMessageReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            string newState = message["State"] as string;
            switch (newState)
            {
                case "On":
                    {
                        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                       .RunAsync(
                              CoreDispatcherPriority.High,
                             () =>
                             {
                                 if(!isWorking)
                                 {
                                     StartWork();
                                     isWorking = true;
                                 }
                             });
                        break;
                    }
                case "Off":
                    {
                        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                       .RunAsync(
                              CoreDispatcherPriority.High,
                             () =>
                             {
                                 if(isWorking)
                                 {
                                     StopWork();
                                     isWorking = false;
                                 }
                             });
                        break;
                    }
                case "Unspecified":
                default:
                    {
                        // Do nothing 
                        break;
                    }
            }
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
