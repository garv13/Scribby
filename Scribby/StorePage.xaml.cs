using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class StorePage : Page
    {

        private IMobileServiceTable<Pack> Table = App.MobileService.GetTable<Pack>();
        private MobileServiceCollection<Pack,Pack> items;
        private int page = 0;
        
        public StorePage()
        {
            this.InitializeComponent();
            Loaded += StorePage_Loaded;
        }

        Pack i = new Pack();
        private async void StorePage_Loaded(object sender, RoutedEventArgs e)
        {
            items = await Table.Skip(l * 15).Take(15).Where(Pack
                          => Pack.InStore == true).ToCollectionAsync();
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
