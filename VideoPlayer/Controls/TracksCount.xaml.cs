using System;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VideoPlayer.Controls
{
    public  sealed partial class TracksCount : UserControl
    {
        private Popup popup;
        private string audioCount_str;
        private string videoCount_str;
        private string timedCount_str;
        private TimeSpan showTime_tmr;
        private TracksCount()
        {
            this.InitializeComponent();
            popup = new Popup();
            MeasurePopupSize();
            popup.Child = this;
            this.Loaded += NotifyPopup_Loaded;
            this.Unloaded += NotifyPopup_Unloaded;
        }

        public TracksCount(string audio_count,string video_count,string timed_count, TimeSpan showTime) : this()
        {
            this.audioCount_str = audio_count;
            this.videoCount_str = video_count;
            this.timedCount_str = timed_count;
            this.showTime_tmr = showTime;
        }

        public TracksCount(string audio_count,string video_count,string timed_count) : this(audio_count,video_count,timed_count, TimeSpan.FromSeconds(2))
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
            if (!string.IsNullOrEmpty(audioCount_str))
            {
                this.audioCount_textBlock.Text = audioCount_str;
            }
            if (!string.IsNullOrEmpty(videoCount_str))
            {
                this.videoCount_textBlock.Text = videoCount_str;
            }
            if (!string.IsNullOrEmpty(timedCount_str))
            {
                this.timedCount_textBlock.Text = timedCount_str;
            }         
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
