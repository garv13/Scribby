using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Scribby
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Image_Set : Page
    {
        string url = "";
        MediaCapture _mediaCapture;
        bool _isPreviewing;



        DisplayRequest _displayRequest;
        Compass c;
        Image im;
        int i;
        double yaw;
        double x, y;
        OrientationSensor or;
        
        double wid, h, stepW, stepH, temppitch, tempyaw;
        double yaw5, pitch5;
        public Image_Set()
        {
            i = 0;
            yaw5 = 0; pitch5 = 0;
            or = OrientationSensor.GetDefault();
            c = Compass.GetDefault();
            //mg.ReadingChanged += Mg_ReadingChanged;
            c.ReportInterval = 4;
            c.ReadingChanged += C_ReadingChanged;
            or.ReportInterval = 4;
            or.ReadingChanged += Or_ReadingChanged;
            this.InitializeComponent();
            Geoposition location;
            //tim.Start();
            im = new Image();
            im.Width = 300;
            im.Height = 300;
            Loaded += Image_Set_Loaded;
            Application.Current.Suspending += Application_Suspending;

        }

        private async void Image_Set_Loaded(object sender, RoutedEventArgs e)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // _rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage);

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 2, MovementThreshold = 2 };

                    // Subscribe to the StatusChanged event to get updates of location status changes.
                    geolocator.PositionChanged += Geolocator_PositionChanged;

                    // Carry out the operation.
                    Geoposition pos = await geolocator.GetGeopositionAsync();

                    break;
            }

            await Get_Img_Url();
            im.Source = new BitmapImage(new Uri(url)); // this is the image
            
            lol.Children.Add(im);
        }

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            
        }

        private void C_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            CompassReading reading = args.Reading;
            yaw = reading.HeadingTrueNorth.Value;
            //Debug.WriteLine(lol.ToString());
        }

      
        
        private async void Or_ReadingChanged(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
        {
            //check angle here
            OrientationSensorReading reading = args.Reading;

            // Quaternion values
            SensorQuaternion q = reading.Quaternion;   // get a reference to the object to avoid re-creating it for each access
            double ysqr = q.Y * q.Y;
            // roll (x-axis rotation)
            double t0 = +2.0 * (q.W * q.X + q.Y * q.Z);
            double t1 = +1.0 - 2.0 * (q.X * q.X + ysqr);
            
            // pitch (y-axis rotati)
            double t2 = +2.0 * (q.W * q.Y - q.Z * q.X);
            t2 = t2 > 1.0 ? 1.0 : t2;
            t2 = t2 < -1.0 ? -1.0 : t2;
            double pitch = Math.Asin(t2);
            pitch = pitch * 180 / Math.PI;
            // yaw (z-axis rotation)


            yaw5 += yaw;
            pitch5 += pitch;
            i++;
            if (i == 14)
            {
                yaw = yaw5 / 15;
                pitch = pitch5 / 15;
                yaw5 = pitch5 = i = 0;
                if (yaw < 0)
                    yaw += 360;
                if (pitch < 0)
                    pitch += 360;
                // Debug.WriteLine(yaw.ToString() + "," + pitch.ToString());


                
            }
            
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await StartPreviewAsync();
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
        }
        private async Task StartPreviewAsync()
        {
            try
            {

                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync();
                PreviewControl.Source = _mediaCapture;

                await _mediaCapture.StartPreviewAsync();
                _isPreviewing = true;

                _displayRequest.RequestActive();
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
        private async Task CleanupCameraAsync()
        {
            if (_mediaCapture != null)
            {
                if (_isPreviewing)
                {
                    await _mediaCapture.StopPreviewAsync();
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PreviewControl.Source = null;
                    if (_displayRequest != null)
                    {
                        _displayRequest.RequestRelease();
                    }

                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                });
            }

        }


        public async Task Get_Img_Url()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile imgFile = await localFolder.CreateFileAsync("ImageFile.png", CreationCollisionOption.OpenIfExists); // image to be uploaded
            if (imgFile != null)
                // imgFile.DeleteAsync(); func to delete image put after upload completed
                url = imgFile.Path;
            else
            {
                MessageDialog msgbox = new MessageDialog("Some error occured please re capture the image");
                await msgbox.ShowAsync();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Note n = new Note();
            //must add user id and user access
            n.Like = 0;
            n.Media_Type = "Image";
            n.store = false;
           
            CompassReading reading = c.GetCurrentReading();
            n.Yaw  = reading.HeadingTrueNorth.Value;

            OrientationSensorReading reading2 = or.GetCurrentReading();
            SensorQuaternion q = reading2.Quaternion;   // get a reference to the object to avoid re-creating it for each access
            double ysqr = q.Y * q.Y;
            // roll (x-axis rotation)
            double t0 = +2.0 * (q.W * q.X + q.Y * q.Z);
            double t1 = +1.0 - 2.0 * (q.X * q.X + ysqr);

            // pitch (y-axis rotati)
            double t2 = +2.0 * (q.W * q.Y - q.Z * q.X);
            t2 = t2 > 1.0 ? 1.0 : t2;
            t2 = t2 < -1.0 ? -1.0 : t2;
            double pitch = Math.Asin(t2);
            pitch = pitch * 180 / Math.PI;

            if (pitch < 0)
                pitch += 360;
            var accessStatus = await Geolocator.RequestAccessAsync();
            Geoposition pos = null;
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // _rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage);

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 2, MovementThreshold = 2 };

                    // Subscribe to the StatusChanged event to get updates of location status changes.
                    geolocator.PositionChanged += Geolocator_PositionChanged;

                    // Carry out the operation.
                    pos = await geolocator.GetGeopositionAsync();
                    break;
            }

            var credentials = new StorageCredentials("vrdreamer", "lTD5XmjEhvfUsC/vVTLsl01+8pJOlMdF/ri7W1cNOydXwSdb8KQpDbiveVciOqdIbuDu6gJW8g44YtVjuBzFkQ==");
            var client = new CloudBlobClient(new Uri("https://vrdreamer.blob.core.windows.net/"), credentials);
            var container = client.GetContainerReference("first");
            await container.CreateIfNotExistsAsync();

            var perm = new BlobContainerPermissions();
            perm.PublicAccess = BlobContainerPublicAccessType.Blob;
            await container.SetPermissionsAsync(perm);
            var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".png");
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile imgFile = await localFolder.CreateFileAsync("ImageFile.png", CreationCollisionOption.OpenIfExists); // image to be uploaded

            using (var fileStream = await imgFile.OpenSequentialReadAsync())
            {
                
                //await blockBlob.UploadFromStreamAsync(fileStream);
                await blockBlob.UploadFromFileAsync(imgFile);
            }
            n.Media_Url = blockBlob.StorageUri.PrimaryUri.ToString();
            n.Pitch = pitch;
            n.UserId = "";
            n.gps_coordinate = pos.Coordinate.Point.Position.Latitude.ToString() + "," + pos.Coordinate.Point.Position.Longitude.ToString();
            await App.MobileService.GetTable<Note>().InsertAsync(n);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void MainPage_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Canvas_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CanvasPage));
        }

        private void Store_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StorePage));
        }

        private void Notes_Botton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyNotesPage));
        }

        private void About_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
        private void SignOut_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignUp));
        }
    }
}
