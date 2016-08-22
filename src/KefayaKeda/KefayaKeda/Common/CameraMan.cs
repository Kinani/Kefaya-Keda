using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KefayaKeda.Common
{
    public class CameraMan
    {
        MediaCapture _mediaCapture = null;
        CaptureElement _captureElement = null;
        
        public CameraMan( CaptureElement captureElement)
        {   
            _captureElement = captureElement;
            
            Application.Current.Suspending += Application_Suspending;
        }

        public async Task StartPreviewAsync()
        {
            try
            {

                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync();

                _captureElement.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();
                Constants.IsPreviewing = true;

               
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                System.Diagnostics.Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed. {0}", ex.Message);
            }
        }

        public async Task CleanupCameraAsync()
        {
            if (_mediaCapture != null)
            {
                if (Constants.IsPreviewing)
                {
                    await _mediaCapture.StopPreviewAsync();
                }

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _captureElement.Source = null;
                    
                });
                
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }

        }
        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {

            var deferral = e.SuspendingOperation.GetDeferral();
            await CleanupCameraAsync();
            deferral.Complete();

        }

    }
}
