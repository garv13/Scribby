using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Scribby
{
    class StoreListing
    {
        public string Title { get; set; }

        public string Price { get; set; }

        public BitmapImage Image { get; set; }

        public string Id { get; set; }
    }
}
