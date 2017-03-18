using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Scribby
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CanvasPage : Page
    {
        public CanvasPage()
        {
            this.InitializeComponent();
            DrawingArea.InkPresenter.InputDeviceTypes =
       Windows.UI.Core.CoreInputDeviceTypes.Mouse |
       Windows.UI.Core.CoreInputDeviceTypes.Pen;
        }


        private async void NextBar_Click(object sender, RoutedEventArgs e)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, (int)DrawingArea.ActualWidth, (int)DrawingArea.ActualHeight, 96);

            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawInk(DrawingArea.InkPresenter.StrokeContainer.GetStrokes());
            }




            //var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            //savePicker.SuggestedStartLocation =
            //    Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //// Dropdown of file types the user can save the file as
            //savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".png" });
            //// Default file name if the user does not type one in or select a file to replace
            //savePicker.SuggestedFileName = "New Image";

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile imgFile = await localFolder.CreateFileAsync("ImageFile.png", CreationCollisionOption.ReplaceExisting);


            if (imgFile != null)
            {
                CachedFileManager.DeferUpdates(imgFile);
                using (var fileStream = await imgFile.OpenAsync(FileAccessMode.ReadWrite))
                    await renderTarget.SaveAsync(fileStream, CanvasBitmapFileFormat.Png, 1f);
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(imgFile);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    MessageDialog msgbox = new MessageDialog("Image couldn't be saved");
                    await msgbox.ShowAsync();
                }
            }
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
