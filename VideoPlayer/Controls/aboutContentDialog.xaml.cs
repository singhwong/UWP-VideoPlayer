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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace VideoPlayer.Controls
{
    public sealed partial class aboutContentDialog : UserControl
    {
        private Popup popup;
        public aboutContentDialog()
        {
            this.InitializeComponent();
            popup = new Popup();
            popup.Child = this;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await about_contentDialog.ShowAsync();
        }

        public void Show()
        {
            popup.IsOpen = true;
        }

        private void Close_button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            about_contentDialog.Hide();
        }
    }
}
