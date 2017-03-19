﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Scribby
{
    class Pack
    {
        public BitmapImage Image { get; set; }
        public string Title { get; set; }
        public bool InStore { get; set; }
        public string Id { get; set; }

        public string PackId { get; set; }

        public string UserId { get; set; }

        public int No_Of_Purchases { get; set; }

        public float Price { get; set; }

        public string Icon_Url { get; set; }

    }
}
