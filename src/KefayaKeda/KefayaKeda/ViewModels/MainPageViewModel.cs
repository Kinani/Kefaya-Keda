using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace KefayaKeda.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private CancellationTokenSource wtoken;
        private Task task;

        #region Properties

        #endregion

        #region Commands
        private RelayCommand captureEleLoaded;
        public RelayCommand CaptureEleLoaded
        {
            get
            {
                captureEleLoaded = new RelayCommand(() =>
                {

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
                       .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                       {
                           //TODO
                       });

                    await Task.Delay(8000, wtoken.Token);
                }
            }, wtoken.Token);
        }
    }
}
