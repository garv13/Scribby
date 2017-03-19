using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class Image_Set : Page
    {
        public Image_Set()
        {
            this.InitializeComponent();
            await Get_Img_Url();
            im.Source = new BitmapImage(new Uri(url)); // this is the image
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
    }
}
