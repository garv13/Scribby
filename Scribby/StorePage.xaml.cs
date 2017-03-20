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
        List<StoreListing> StoreList = new List<StoreListing>();
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
            StoreListing temp;
            items = await Table.Take(10).ToCollectionAsync();
            foreach (Pack lol in items)
            {
                temp = new StoreListing();
                temp.Id = lol.Id;
                temp.Title = lol.Title;
                temp.Image = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(lol.Icon_Url));
                temp.Price = "Price: " + lol.Price.ToString();
                StoreList.Add(temp);
            }
            StoreListView.DataContext = StoreList;
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void StoreListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            StoreListing sent = e.ClickedItem as StoreListing;
            Frame.Navigate(typeof(StoreDetailPage), sent);
        }
    }
}
