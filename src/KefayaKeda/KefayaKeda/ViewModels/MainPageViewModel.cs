using GalaSoft.MvvmLight;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace KefayaKeda.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Properties

        #endregion

        #region Commands

        #endregion

        public MainPageViewModel() { }

        private async Task<AnalysisResult> DoVision(StorageFile file)
        {
            VisionServiceClient visonClient = new VisionServiceClient(Constants.OxfordVision);
            VisualFeature[] visualFeatures = new VisualFeature[] {VisualFeature.Description, VisualFeature.Tags };
            var fileStream = await file.OpenAsync(FileAccessMode.Read);
            return await visonClient.AnalyzeImageAsync(fileStream.AsStream(), visualFeatures);
        }


    }
}
