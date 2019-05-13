﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
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
    public sealed partial class VolumeContentDialog : UserControl
    {
        private Popup popup;
        private string percent_str;
        private string volumeFontIcon_str;
        private TimeSpan showTime_tmr;
        public VolumeContentDialog()
        {
            this.InitializeComponent();
            popup = new Popup();
            popup.Child = this;
            MeasurePopupSize();
            this.Loaded += NotifyPopup_Loaded;
            this.Unloaded += NotifyPopup_Unloaded;
        }

        public VolumeContentDialog(string fontIcon_str, string content, TimeSpan showTime) : this()
        {
            this.percent_str = content;
            this.showTime_tmr = showTime;
            this.volumeFontIcon_str = fontIcon_str;

        }

        public VolumeContentDialog(string icon_str, string content) : this(icon_str, content, TimeSpan.FromSeconds(2))
        {
        }

        public void Show()
        {
            this.popup.IsOpen = true;
        }
       
        public void Hide()
        {
            this.popup.IsOpen = false;
        }

        private void MeasurePopupSize()
        {
            this.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            double marginTop = 0;
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                marginTop = StatusBar.GetForCurrentView().OccludedRect.Height;
            this.Height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            this.Margin = new Thickness(0, marginTop, 0, 0);
        }

        private void NotifyPopup_Loaded(object sender, RoutedEventArgs e)
        {
            this.showContent_textBlock.Text = percent_str;
            this.volume_fontIcon.Glyph = volumeFontIcon_str;
            this.story_Board.BeginTime = this.showTime_tmr;
            this.story_Board.Begin();
            this.story_Board.Completed += storyBoard_Completed;
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += NotifyPopup_VisibleBoundsChanged;
        }
        private void NotifyPopup_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            MeasurePopupSize();
        }

        private void storyBoard_Completed(object sender, object e)
        {
            this.popup.IsOpen = false;
        }

        private void NotifyPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().VisibleBoundsChanged -= NotifyPopup_VisibleBoundsChanged;
        }
    }
}
