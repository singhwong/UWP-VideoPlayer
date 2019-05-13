using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoPlayer.Modle;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace VideoPlayer.View
{
    public sealed partial class FoldersViewControl : UserControl
    {
        public VideoLibrary this_library  { get { return this.DataContext as VideoLibrary; } }
        //private SolidColorBrush black = new SolidColorBrush(Colors.Black);
        //private SolidColorBrush whiteSmoke = new SolidColorBrush(Colors.WhiteSmoke);
        public FoldersViewControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }

        //private void UserControl_ActualThemeChanged(FrameworkElement sender, object args)
        //{
        //    if (ActualTheme == ElementTheme.Light)
        //    {
        //        folders_border.Background = whiteSmoke;
        //        //folder_textBlock.Foreground = whiteSmoke;
        //    }
        //    else
        //    {
        //        folders_border.Background = black;
        //        //folder_textBlock.Foreground = black;
        //    }
        //    //switch (this.ActualTheme)
        //    //{
        //    //    case ElementTheme.Default:
        //    //        folders_border.Background = whiteSmoke;
        //    //        break;
        //    //    case ElementTheme.Light:
        //    //        folders_border.Background = whiteSmoke;
        //    //        break;
        //    //    case ElementTheme.Dark:
        //    //        folders_border.Background = black;
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
        //}
    }
}
