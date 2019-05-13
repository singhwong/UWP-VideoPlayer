using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace VideoPlayer.Modle
{
    public class VideoLibrary
    {
        public string FolderName { get; set; }
        public string FolderPath { get; set; }
        public BitmapImage FolderImage { get; set; }
    }
}
